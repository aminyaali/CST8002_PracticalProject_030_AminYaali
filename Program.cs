/*
 * CST8002 Programming Language Research Project
 * Practical Project 2 - Project Review I
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: See Brightspace
 *
 * References:
 * [1] CsvHelper Documentation, https://joshclose.github.io/CsvHelper/
 * [2] Microsoft Learn C# Documentation, https://learn.microsoft.com/
 */

using Project_2.Business;
using Project_2.Persistence;
using Project_2.Presentation;

namespace Project_2
{
    /// <summary>
    /// Application entry point. This class only wires the three layers together
    /// (Persistence, Business, Presentation) and starts the interactive menu; it
    /// contains no business logic, no file handling, and no console interaction of
    /// its own.
    /// </summary>
    internal class Program
    {
        /// <summary>Path to the source data set CSV file used by the persistence layer.</summary>
        private const string DataSetFilePath = "CanadianBeaufortSea_Invert_MeHg&SI_EN_FR.csv";

        /// <summary>Application entry point.</summary>
        /// <param name="args">Command-line arguments (unused).</param>
        static void Main(string[] args)
        {
            var fileManager = new DatasetFileManager(DataSetFilePath);
            var recordManager = new RecordManager(fileManager);
            var menu = new MenuPresenter(recordManager);

            menu.Run();
        }
    }
}
