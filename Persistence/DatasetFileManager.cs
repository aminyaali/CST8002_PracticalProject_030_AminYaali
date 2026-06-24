/*
 * CST8002 Programming Language Research Project
 * Practical Project 2 - Project Review I
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: June 21, 2026
 *
 * References:
 * [1] CsvHelper Documentation, https://joshclose.github.io/CsvHelper/
 * [2] Microsoft Learn C# Documentation, "Guid Struct,"
 *     https://learn.microsoft.com/en-us/dotnet/api/system.guid
 */

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Project_2.Model;

namespace Project_2.Persistence
{
    /// <summary>
    /// Persistence layer responsible for all File-IO for the application: reading the
    /// source data set from disk into record objects, and writing the in-memory record
    /// objects back out to a new CSV file. No business rules or user interaction live
    /// in this class; it only knows how to move data between memory and disk.
    /// </summary>
    public class DatasetFileManager
    {
        /// <summary>Number of non-data metadata rows at the top of the source CSV file
        /// that must be skipped before the header row is reached.</summary>
        private const int MetadataRowsToSkip = 39;

        /// <summary>Maximum number of records loaded from the source data set on startup.</summary>
        private const int MaxRecordsToLoad = 100;

        /// <summary>Path to the source data set CSV file.</summary>
        private readonly string sourceFilePath;

        /// <summary>
        /// Creates a new file manager targeting the given source data set file. Registers
        /// the code-pages encoding provider needed to read the Windows-1252 encoded source
        /// file; this is safe to call more than once, so both the application entry point
        /// and the unit test project can construct this class directly without depending
        /// on each other to have registered the provider first.
        /// </summary>
        /// <param name="sourceFilePath">Path to the source CSV data set.</param>
        public DatasetFileManager(string sourceFilePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            this.sourceFilePath = sourceFilePath;
        }

        /// <summary>
        /// Reads the source data set from disk, skipping the metadata rows at the top of
        /// the file, and parses up to <see cref="MaxRecordsToLoad"/> records into a list of
        /// <see cref="DatasetRecord"/> objects. If the file contains fewer records than the
        /// maximum, all available records are loaded.
        /// </summary>
        /// <returns>A list of record objects parsed from the data set.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
        /// <exception cref="IOException">Thrown when the source file cannot be read.</exception>
        public List<DatasetRecord> LoadRecords()
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"Data set file was not found: {sourceFilePath}");
            }

            var records = new List<DatasetRecord>();

            // The source file is encoded as Windows-1252, not UTF-8. The per mille sign
            // (\u2030) used in the d13C / d15N / d34S column headers is stored as the
            // single byte 0x89, which is only correctly decoded as \u2030 under
            // Windows-1252 (it is invalid/undefined under UTF-8 and decodes to the wrong
            // character under Latin-1). Reading with the wrong encoding silently corrupts
            // these column headers, which then fail to match when looked up by name.
            using var reader = new StreamReader(sourceFilePath, System.Text.Encoding.GetEncoding(1252));

            // Skip the metadata rows above the header row.
            for (int i = 0; i < MetadataRowsToSkip; i++)
            {
                reader.ReadLine();
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var record = new DatasetRecord
                {
                    SampleID = csv.GetField("Sample ID") ?? string.Empty,
                    Species = csv.GetField("Species") ?? string.Empty,
                    Taxon = csv.GetField("Taxon") ?? string.Empty,
                    TissueAnalysed = csv.GetField("Tissue analysed") ?? string.Empty,
                    Location = csv.GetField("Location") ?? string.Empty,
                    CollectionDate = csv.GetField("Collection date") ?? string.Empty,
                    Latitude = csv.GetField("Latitude") ?? string.Empty,
                    Longitude = csv.GetField("Longitude") ?? string.Empty,
                    Depth = csv.GetField("Depth (m)") ?? string.Empty,
                    NumberOfIndividuals = csv.GetField("n (ind)") ?? string.Empty,
                    D13C = csv.GetField("d13C (\u2030)") ?? string.Empty,
                    D15N = csv.GetField("d15N (\u2030)") ?? string.Empty,
                    D34S = csv.GetField("d34S (\u2030)") ?? string.Empty,
                    MeHg = csv.GetField("MeHg (ng/g, DW)") ?? string.Empty
                };

                records.Add(record);

                if (records.Count >= MaxRecordsToLoad)
                {
                    break;
                }
            }

            return records;
        }

        /// <summary>
        /// Persists the given records to a brand new CSV file on disk. The output file
        /// name is generated using a GUID so each save produces a unique, non-colliding
        /// file name, per [2].
        /// </summary>
        /// <param name="records">The in-memory records to write to disk.</param>
        /// <param name="outputDirectory">Directory the new file will be written into.</param>
        /// <returns>The full path of the newly created file.</returns>
        /// <exception cref="IOException">Thrown when the file cannot be written.</exception>
        public string SaveRecords(List<DatasetRecord> records, string outputDirectory = ".")
        {
            Directory.CreateDirectory(outputDirectory);

            // Generate a unique output file name using a GUID so repeated saves never
            // overwrite a previous export.
            string fileName = $"DatasetExport_{Guid.NewGuid()}.csv";
            string fullPath = Path.Combine(outputDirectory, fileName);

            using var writer = new StreamWriter(fullPath, false, System.Text.Encoding.UTF8);

            writer.WriteLine(DatasetRecord.CsvHeader());

            foreach (var record in records)
            {
                writer.WriteLine(record.ToCsvLine());
            }

            return fullPath;
        }
    }
}
