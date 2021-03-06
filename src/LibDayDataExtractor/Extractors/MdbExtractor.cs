﻿using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LibDayDataExtractor.Extensions;
using LibDayDataExtractor.Progress;
using System.Linq;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts contents of MDB files into TSV files.
    /// The MDB files are Microsoft Jet databases, that store data in tables.
    /// </summary>
    public class MdbExtractor : IExtractor
    {
        public void Extract(ExtractionPaths path, ProgressReporter progress = null)
        {
            using (OleDbConnection mdbConnection = ConnectToMdbFile(path.OriginalFilePath))
            {
                var tables = GetTableNames(mdbConnection).ToList();
                for (int i = 0; i < tables.Count; i++)
                {
                    ExportTableToTsv(path, mdbConnection, tables[i]);

                    progress?.Report(100 * (i + 1) / tables.Count);
                }
            }
        }

        private static void ExportTableToTsv(
            ExtractionPaths path, OleDbConnection mdbConnection, string tableName)
        {
            string outputFileName = GenerateOutputPath(path, tableName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));

            using (StreamWriter streamWriter = new StreamWriter(outputFileName))
            {
                string query = string.Format("Select * from [{0}]", tableName);
                using (OleDbCommand cmd = new OleDbCommand(query, mdbConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        var csvWriter = new CsvWriter(streamWriter, GetCsvConfig());
                        csvWriter.WriteFullRecord(GetHeaderValues(dataReader));

                        while (dataReader.Read())
                        {
                            csvWriter.WriteFullRecord(GetRowValues(dataReader));
                        }
                    }
                }
            }
        }

        private static CsvConfiguration GetCsvConfig()
        {
            return new CsvConfiguration()
            {
                Delimiter = "\t",
                QuoteAllFields = true,
            };
        }

        private static IEnumerable<string> GetHeaderValues(OleDbDataReader dataReader)
        {
            foreach (DataRow dataRow in dataReader.GetSchemaTable().Rows)
            {
                yield return dataRow.ItemArray[0].ToString();
            }
        }

        private static IEnumerable<string> GetRowValues(OleDbDataReader dataReader)
        {
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                yield return dataReader.GetValue(index).ToString();
            }
        }

        private static string GenerateOutputPath(ExtractionPaths paths, string tableName)
        {
            return Path.Combine(paths.OutputDirectory, paths.OriginalFileName, $"{tableName}.tsv");
        }

        private static OleDbConnection ConnectToMdbFile(string mdbPath)
        {
            OleDbConnectionStringBuilder sb = new OleDbConnectionStringBuilder()
            {
                Provider = "Microsoft.Jet.OLEDB.4.0",
                PersistSecurityInfo = false,
                DataSource = mdbPath
            };

            string password = GetPassword(mdbPath);
            if (!string.IsNullOrEmpty(password))
            {
                sb.Add("Jet OLEDB:Database Password", password);
            }

            OleDbConnection conn = new OleDbConnection(sb.ToString());
            conn.Open();
            return conn;
        }

        private static string GetPassword(string mdbPath)
        {
            byte[] key = { 0x86, 0xfb, 0xec, 0x37, 0x5d, 0x44, 0x9c, 0xfa, 0xc6, 0x5e, 0x28, 0xe6, 0x13, 0xb6 };
            byte[] password = new byte[14];

            using (FileStream file = File.OpenRead(mdbPath))
            {
                BinaryReader reader = new BinaryReader(file);

                int passwordLength = 0;
                for (passwordLength = 0; passwordLength < 14; passwordLength++)
                {
                    file.Seek(0x42 + passwordLength, SeekOrigin.Begin);

                    byte j = (byte)reader.ReadInt32();

                    j ^= key[passwordLength];

                    if (j != 0)
                    {
                        password[passwordLength] = j;
                    }
                    else
                    {
                        password[passwordLength] = 0;

                        break;
                    }
                }

                return Encoding.ASCII.GetString(password, 0, passwordLength);
            }
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
