using CsvHelper;
using System.Collections.Generic;

namespace LibDayDataExtractor.Extensions
{
    public static class CsvWriterExtensions
    {
        public static void WriteFullRecord(this CsvWriter csvWriter, IEnumerable<string> record)
        {
            foreach (string headerColumn in record)
            {
                csvWriter.WriteField(headerColumn);
            }

            csvWriter.NextRecord();
        }
    }
}
