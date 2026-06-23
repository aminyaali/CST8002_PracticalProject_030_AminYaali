/*
 * CST8002 Programming Language Research Project
 * Practical Project 2 - Project Review I
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: See Brightspace
 *
 * References:
 * [1] xUnit.net Documentation, https://xunit.net/
 * [2] Microsoft Learn C# Documentation, https://learn.microsoft.com/
 */

using Project_2.Persistence;
using Xunit;

namespace Project_2.Tests
{
    /// <summary>
    /// Unit tests proving the persistence layer correctly reads the data set file and
    /// places each column's data into the matching field of a <see cref="Model.DatasetRecord"/>.
    /// Uses a small, self-contained sample file (<c>TestData/SampleDataset.csv</c>) that
    /// mirrors the real data set's structure: the same number of metadata rows before the
    /// header, and a Location column value that contains an embedded comma, to confirm
    /// that comma-containing quoted fields are parsed as a single value rather than being
    /// split apart.
    /// </summary>
    public class DatasetFileManagerTests
    {
        /// <summary>Path to the small sample CSV file used by these tests.</summary>
        private const string SampleFilePath = "TestData/SampleDataset.csv";

        /// <summary>
        /// Verifies that loading records from the sample data set produces the expected
        /// number of records and that each field of the first record matches the
        /// corresponding column value from the source CSV row, including a Location value
        /// that contains a comma.
        /// </summary>
        [Fact]
        public void LoadRecords_ParsesFieldsFromCsvIntoCorrectRecordProperties()
        {
            // Arrange
            var fileManager = new DatasetFileManager(SampleFilePath);

            // Act
            var records = fileManager.LoadRecords();

            // Assert
            Assert.Equal(3, records.Count);

            var firstRecord = records[0];
            Assert.Equal("W000001-01", firstRecord.SampleID);
            Assert.Equal("Ampelisca sp.", firstRecord.Species);
            Assert.Equal("Amphipoda", firstRecord.Taxon);
            Assert.Equal("Whole body", firstRecord.TissueAnalysed);

            // The Location field contains a comma inside its quoted CSV value; if the
            // parser were splitting on every comma instead of respecting quoted fields,
            // this value would be truncated to "Northwest of Cape Bathurst" and every
            // field after it would be shifted by one column.
            Assert.Equal("Northwest of Cape Bathurst, NT", firstRecord.Location);

            Assert.Equal("2007-07-31", firstRecord.CollectionDate);
            Assert.Equal("70.6952", firstRecord.Latitude);
            Assert.Equal("-128.8393", firstRecord.Longitude);
            Assert.Equal("22.31", firstRecord.Depth);
            Assert.Equal("4", firstRecord.NumberOfIndividuals);
            Assert.Equal("8.3", firstRecord.D15N);
            Assert.Equal("19.5", firstRecord.D34S);
            Assert.Equal("4", firstRecord.MeHg);
        }

        /// <summary>
        /// Verifies that attempting to load a data set file that does not exist raises a
        /// <see cref="FileNotFoundException"/> rather than an unhandled crash, confirming
        /// the persistence layer's exception handling for a missing file.
        /// </summary>
        [Fact]
        public void LoadRecords_MissingFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileManager = new DatasetFileManager("TestData/ThisFileDoesNotExist.csv");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => fileManager.LoadRecords());
        }
    }
}
