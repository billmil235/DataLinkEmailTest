using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataLinkEmailLists
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("CSV Parser");
            Console.WriteLine("");

            // Prompt user for files to process.
            Console.WriteLine("Enter first filename: ");
            var firstFileName = Console.ReadLine();

            Console.WriteLine("Enter second filename: ");
            var secondFileName = Console.ReadLine();

            // Create our CSV reader repositories.
            var firstFile = new CSVFileRepository(firstFileName);
            var secondFile = new CSVFileRepository(secondFileName);

            // Obtain the various data structures that are needed for the different
            // methods.
            var firstFileDataList = firstFile.ReadFileIntoList();
            var secondFileDataList = secondFile.ReadFileIntoList();
            var firstFileDataDict = firstFile.ReadFileIntoDictionary(firstFileDataList);
            var firstFileHashSet = firstFile.ReadFileIntoHashSet(firstFileDataList);

            Console.WriteLine("Running tests.....");
            Console.WriteLine("");

            // Start running tests
            MainClass.RunLinq(firstFileDataList, secondFileDataList);

            MainClass.RunDictionary(firstFileDataDict, secondFileDataList);

            MainClass.RunHashSet(firstFileHashSet, secondFileDataList);

            // Done.
            return;

        }

        /// <summary>
        /// This method uses a LINQ query to perform an inner join on the two lists of data.  For all matching
        /// rows, print the associated email address.
        /// </summary>
        private static void RunLinq(List<FileRowEntity> firstFileDataList, List<FileRowEntity> secondFileDataList)
        {

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            // Perform an INNER JOIN using LINQ to look for common firstname / lastname pairs
            IEnumerable<string> result = from secondrow in secondFileDataList
                         join firstrow in firstFileDataList
                         on new { secondrow.FirstName, secondrow.LastName }
                         equals new { firstrow.FirstName, firstrow.LastName }
                         select secondrow.EMailAddress;

            // Print the results.
            foreach(var row in result)
            {
                Console.WriteLine(row);
            }

            stopWatch.Stop();

            Console.WriteLine($"Total elapsed time (LINQ): {stopWatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine("");

        }

        /// <summary>
        /// This method uses a dictionary of dictionaries to determine if the email address should be printed.  It
        /// retrieves the dictionary for the associated last name.  It then checks that second dictionary for the
        /// first name.  If it is found, it prints the email address.
        /// </summary>
        private static void RunDictionary(Dictionary<string, Dictionary<string, string>> firstFileDataDict, List<FileRowEntity> secondFileDataList)
        {

            var stopWatch = new Stopwatch();
            Dictionary<string, string> tempDict = null;

            stopWatch.Start();

            // Iterate through list of users to look for.
            foreach(var row in secondFileDataList)
            {

                // Get the dictionary that makes on the first name.
                if(firstFileDataDict.TryGetValue(row.FirstName, out tempDict))
                {

                    // Get the dictionary tha matches on the last name.
                    // Output to the discard variable since we only care if something exists.
                    if(tempDict.TryGetValue(row.LastName, out _))
                    {
                        Console.WriteLine(row.EMailAddress);
                    }

                }

            }

            stopWatch.Stop();

            Console.WriteLine($"Total elapsed time (DICTIONARY): {stopWatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine("");

        }

        /// <summary>
        /// This method uses a HASH SET to match rows from the first file to rows in the second file.  If the concatenation
        /// of first ane last name from the second file exists as a key in the first file, it prints the email address
        /// from the second file.
        /// </summary>
        private static void RunHashSet(HashSet<string> firstFileHashSet, List<FileRowEntity> secondFileDataList)
        {

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            foreach (var fileRowEntity in secondFileDataList)
            {

                var key = string.Concat(fileRowEntity.FirstName, fileRowEntity.LastName);

                // Check to see if the HASH SET contains the key.  If it does,
                // print the new email address.
                if (firstFileHashSet.Contains(key))
                {
                    Console.WriteLine(fileRowEntity.EMailAddress);
                }

            }

            stopWatch.Stop();

            Console.WriteLine($"Total elapsed time (HASH SET): {stopWatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine("");

        }

    }
}
