using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Service
{
    class ExportToCSV
    {

        /// <summary>
        /// Creates the CSV from a generic list.
        /// </summary>;
        /// <typeparam name="T"></typeparam>;
        /// <param name="list">The List.</param>;
        /// <param name="csvCompletePath">Name of CSV (w/ path) w/ file ext.</param>;
        public static bool CreateCSVFromGenericList<T>(List<T> list, string csvCompletePath)
        {
            if (list == null || list.Count == 0) return false;

            string newLine = Environment.NewLine;

            try
            {

                if (!Directory.Exists(Path.GetDirectoryName(csvCompletePath))) Directory.CreateDirectory(Path.GetDirectoryName(csvCompletePath));

                if (!File.Exists(csvCompletePath)) File.Create(csvCompletePath).Close();
                    
                using (var sw = new StreamWriter(csvCompletePath))
                {

                    //gets all properties
                    var properties = typeof(T).GetProperties();
                    //PropertyInfo[] props = o.GetType().GetProperties();

                    var result = new StringBuilder();

                    //Get header row
                    var HeaderLine = string.Join(",", properties.Select(d => d.Name).ToArray());
                    //Create CSV String format
                    result.AppendLine(HeaderLine); //Insert Hearder row First
                    result.Append(CreateCSVTextFormat(list, ",")); //Insert Body
                
                    //Writ to csv File
                    sw.Write(result);

                }
                return true;
            }catch
            {
                return false;
            }
        }

        private static StringBuilder CreateCSVTextFormat<T>(List<T> data, string seperator = ",")
        {
            var properties = typeof(T).GetProperties();
            var result = new StringBuilder();

            foreach (var row in data)
            {
                var values = properties.Select(p => p.GetValue(row, null));
                var line = string.Join(seperator, values);
                result.AppendLine(line);
            }

            return result;
        }

    }
}
