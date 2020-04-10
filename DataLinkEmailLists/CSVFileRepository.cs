using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace DataLinkEmailLists
{

    public class ParseMap : CsvMapping<FileRowEntity>
    {

        public ParseMap() : base()
        {
            MapProperty(0, c => c.FirstName);
            MapProperty(1, c => c.LastName);
            MapProperty(2, c => c.EMailAddress);
        }

    }

    public class CSVFileRepository
    {

        private readonly string fileToRead;
        private readonly CsvParserOptions parseOptions;
        private readonly ParseMap parseMap;

        public CSVFileRepository(string fileName)
        {
            fileToRead = fileName;
            parseOptions = new CsvParserOptions(true, ',');
            parseMap = new ParseMap();
        }

        /// <summary>
        /// Get the file data into a list.
        /// </summary>
        public List<FileRowEntity> ReadFileIntoList()
        {

            var parser = new CsvParser<FileRowEntity>(parseOptions, parseMap);

            var result = parser
                .ReadFromFile(fileToRead, encoding: Encoding.ASCII)
                .ToList()
                .Select(r => r.Result).ToList<FileRowEntity>();

            return result;

        }

        /// <summary>
        /// Get the dictionary from the file data for the dictionary test.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ReadFileIntoDictionary(List<FileRowEntity> fileContents)
        {

            var result = new Dictionary<string, Dictionary<string, string>>();

            foreach(var entity in fileContents)
            {

                Dictionary<string, string> dictionary = null;

                if(result.TryGetValue(entity.FirstName, out dictionary) == false)
                {
                    result.Add(entity.FirstName, new Dictionary<string, string>());
                    dictionary = result[entity.FirstName];
                }

                dictionary.Add(entity.LastName, entity.EMailAddress);

            }

            return result;

        }

        /// <summary>
        /// Get the hash set from the file data for the hash set test.
        /// </summary>
        public HashSet<string> ReadFileIntoHashSet(List<FileRowEntity> fileContents)
        {

            var result = new HashSet<string>();

            foreach (var entity in fileContents)
            {

                var key = string.Concat(entity.FirstName, entity.LastName);
                result.Add(key);

            }

            return result;

        }

    }
}
