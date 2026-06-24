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
 * [2] Microsoft Learn C# Documentation, https://learn.microsoft.com/
 */

namespace Project_2.Model
{
    /// <summary>
    /// Record object (entity / data-transfer object) representing a single row of the
    /// Canadian Beaufort Sea benthic invertebrate methylmercury (MeHg) and stable
    /// isotope (SI) data set. Property names mirror the data set's column names so the
    /// mapping between source data and source code is explicit and easy to verify.
    /// </summary>
    public class DatasetRecord
    {
        /// <summary>Unique sample identifier, dataset column "Sample ID".</summary>
        public string SampleID { get; set; } = string.Empty;

        /// <summary>Species name, dataset column "Species".</summary>
        public string Species { get; set; } = string.Empty;

        /// <summary>Taxonomic grouping, dataset column "Taxon".</summary>
        public string Taxon { get; set; } = string.Empty;

        /// <summary>Tissue type analysed, dataset column "Tissue analysed".</summary>
        public string TissueAnalysed { get; set; } = string.Empty;

        /// <summary>
        /// Sample collection location, dataset column "Location". Values in this column
        /// may contain a comma (e.g. "Northwest of Cape Bathurst, NT"), so this field is
        /// always quoted when the record is written back out to CSV.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>Date the sample was collected, dataset column "Collection date".</summary>
        public string CollectionDate { get; set; } = string.Empty;

        /// <summary>Latitude of the collection site, dataset column "Latitude".</summary>
        public string Latitude { get; set; } = string.Empty;

        /// <summary>Longitude of the collection site, dataset column "Longitude".</summary>
        public string Longitude { get; set; } = string.Empty;

        /// <summary>Sample depth in metres, dataset column "Depth (m)".</summary>
        public string Depth { get; set; } = string.Empty;

        /// <summary>Number of individuals composited into the sample, dataset column "n (ind)".</summary>
        public string NumberOfIndividuals { get; set; } = string.Empty;

        /// <summary>Stable carbon isotope value, dataset column "d13C (‰)".</summary>
        public string D13C { get; set; } = string.Empty;

        /// <summary>Stable nitrogen isotope value, dataset column "d15N (‰)".</summary>
        public string D15N { get; set; } = string.Empty;

        /// <summary>Stable sulfur isotope value, dataset column "d34S (‰)".</summary>
        public string D34S { get; set; } = string.Empty;

        /// <summary>Methylmercury concentration, dataset column "MeHg (ng/g, DW)".</summary>
        public string MeHg { get; set; } = string.Empty;

        /// <summary>
        /// Parameterless constructor required so the record can be created first and
        /// populated field-by-field while parsing the CSV file, and so a blank record can
        /// be created for the "create new record" presentation-layer workflow.
        /// </summary>
        public DatasetRecord()
        {
        }

        /// <summary>
        /// Convenience constructor used by the "create new record" feature and by the
        /// unit test project to build a fully populated record in one call.
        /// </summary>
        public DatasetRecord(string sampleId, string species, string taxon, string tissueAnalysed,
            string location, string collectionDate, string latitude, string longitude,
            string depth, string numberOfIndividuals, string d13C, string d15N, string d34S, string meHg)
        {
            SampleID = sampleId;
            Species = species;
            Taxon = taxon;
            TissueAnalysed = tissueAnalysed;
            Location = location;
            CollectionDate = collectionDate;
            Latitude = latitude;
            Longitude = longitude;
            Depth = depth;
            NumberOfIndividuals = numberOfIndividuals;
            D13C = d13C;
            D15N = d15N;
            D34S = d34S;
            MeHg = meHg;
        }

        /// <summary>
        /// Returns this record formatted as a single CSV line. The Location field is always
        /// wrapped in double quotes because its values may contain a comma, and any field
        /// that itself contains a double quote has that quote escaped per CSV convention.
        /// </summary>
        /// <returns>A comma-separated, CSV-escaped representation of this record.</returns>
        public string ToCsvLine()
        {
            return string.Join(",",
                EscapeCsvField(SampleID),
                EscapeCsvField(Species),
                EscapeCsvField(Taxon),
                EscapeCsvField(TissueAnalysed),
                EscapeCsvField(Location),
                EscapeCsvField(CollectionDate),
                EscapeCsvField(Latitude),
                EscapeCsvField(Longitude),
                EscapeCsvField(Depth),
                EscapeCsvField(NumberOfIndividuals),
                EscapeCsvField(D13C),
                EscapeCsvField(D15N),
                EscapeCsvField(D34S),
                EscapeCsvField(MeHg));
        }

        /// <summary>
        /// Wraps a field in double quotes if it contains a comma, double quote, or
        /// newline, escaping any embedded double quotes by doubling them. This keeps
        /// values such as Location ("Northwest of Cape Bathurst, NT") intact when the
        /// record is persisted back to disk as CSV.
        /// </summary>
        /// <param name="field">The raw field value.</param>
        /// <returns>The CSV-safe field value.</returns>
        private static string EscapeCsvField(string? field)
        {
            field ??= string.Empty;

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        /// <summary>
        /// Returns the CSV header line matching the order of fields produced by
        /// <see cref="ToCsvLine"/>, used when writing a new output file.
        /// </summary>
        /// <returns>The CSV header line.</returns>
        public static string CsvHeader()
        {
            return "Sample ID,Species,Taxon,Tissue analysed,Location,Collection date,Latitude,Longitude," +
                   "Depth (m),n (ind),d13C (\u2030),d15N (\u2030),d34S (\u2030),MeHg (ng/g, DW)";
        }
    }
}
