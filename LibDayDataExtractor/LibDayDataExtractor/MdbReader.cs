using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace LibDayDataExtractor
{
    public class MdbReader
    {
        public void ExtractToTsv(string mdbFilePath, string outputPath)
        {
            using (OleDbConnection connectionToMdb = ConnectToMdbFile(mdbFilePath))
            {
                foreach (string tableName in GetTableNames(connectionToMdb))
                {
                    ExportTableToTsv(mdbFilePath, outputPath, connectionToMdb, tableName);
                }
            }
        }

        private static void ExportTableToTsv(string mdbPath, string outputPath,
            OleDbConnection connectionToMdb, string tableName)
        {
            string query = string.Format("Select * from [{0}]", tableName);

            OleDbCommand cmd = new OleDbCommand(query, connectionToMdb);
            cmd.CommandType = CommandType.Text;

            string outputFileName = GenerateOutputPath(mdbPath, outputPath, tableName);

            using (StreamWriter streamWriter = new StreamWriter(outputFileName))
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        streamWriter.WriteLine(string.Join("\t", GetRowValues(dataReader)));
                    }
                }
            }
        }

        private static IEnumerable<string> GetRowValues(OleDbDataReader dataReader)
        {
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                yield return dataReader.GetValue(index).ToString();
            }
        }

        private static string GenerateOutputPath(string mdbPath, string outputPath, string tableName)
        {
            string fileName = string.Format("{0}-{1}.tsv",
                Path.GetFileNameWithoutExtension(mdbPath), tableName);

            string filePath = Path.Combine(outputPath, fileName);

            string root = Path.GetDirectoryName(filePath);

            Directory.CreateDirectory(root);

            return filePath;
        }

        private static OleDbConnection ConnectToMdbFile(string mdbPath)
        {
            OleDbConnectionStringBuilder sb = new OleDbConnectionStringBuilder();
            sb.Provider = "Microsoft.Jet.OLEDB.4.0";
            sb.PersistSecurityInfo = false;
            sb.DataSource = mdbPath;
            OleDbConnection conn = new OleDbConnection(sb.ToString());
            conn.Open();
            return conn;
        }

        private static IEnumerable<string> GetTableNames(OleDbConnection conn)
        {
            DataTable schema = conn.GetSchema("Tables");

            foreach (DataRow dataRow in schema.Rows)
            {
                if (dataRow["TABLE_TYPE"].ToString() == "TABLE")
                {
                    yield return dataRow["TABLE_NAME"].ToString();
                }
            }
        }
    }
}