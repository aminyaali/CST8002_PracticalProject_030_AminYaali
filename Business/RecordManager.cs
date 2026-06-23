/*
 * CST8002 Programming Language Research Project
 * Practical Project 2 - Project Review I
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: See Brightspace
 *
 * References:
 * [1] Microsoft Learn C# Documentation, https://learn.microsoft.com/
 */

using Project_2.Model;
using Project_2.Persistence;

namespace Project_2.Business
{
    /// <summary>
    /// Business layer (problem domain) class that owns the in-memory sequential data
    /// structure of <see cref="DatasetRecord"/> objects and exposes the operations the
    /// presentation layer needs: reload, save, select/display, create, edit, and delete.
    /// This class contains no console input/output and no direct file handling; it
    /// delegates persistence to <see cref="DatasetFileManager"/>.
    /// </summary>
    public class RecordManager
    {
        /// <summary>The in-memory sequential data structure holding the loaded records.</summary>
        private List<DatasetRecord> records;

        /// <summary>The persistence-layer collaborator used for File-IO.</summary>
        private readonly DatasetFileManager fileManager;

        /// <summary>
        /// Creates a new record manager backed by the given file manager. The in-memory
        /// list starts empty; call <see cref="LoadFromFile"/> to populate it from disk.
        /// </summary>
        /// <param name="fileManager">The persistence-layer collaborator used for File-IO.</param>
        public RecordManager(DatasetFileManager fileManager)
        {
            this.fileManager = fileManager;
            records = new List<DatasetRecord>();
        }

        /// <summary>Gets the number of records currently held in memory.</summary>
        public int Count => records.Count;

        /// <summary>
        /// Loads (or reloads) records from the data set file, replacing whatever is
        /// currently held in memory. Any exception raised by the persistence layer (for
        /// example, a missing file) is allowed to propagate so the presentation layer can
        /// report it to the user.
        /// </summary>
        public void LoadFromFile()
        {
            records = fileManager.LoadRecords();
        }

        /// <summary>
        /// Writes the current in-memory records to a new CSV file on disk using a
        /// GUID-based file name.
        /// </summary>
        /// <param name="outputDirectory">Directory the export file should be written into.</param>
        /// <returns>The full path of the newly created file.</returns>
        public string SaveToFile(string outputDirectory = ".")
        {
            return fileManager.SaveRecords(records, outputDirectory);
        }

        /// <summary>
        /// Returns a read-only snapshot of all records currently held in memory, in their
        /// current order, for display purposes.
        /// </summary>
        /// <returns>All in-memory records.</returns>
        public IReadOnlyList<DatasetRecord> GetAllRecords()
        {
            return records;
        }

        /// <summary>
        /// Returns the single record at the given zero-based index, or null if the index
        /// is out of range.
        /// </summary>
        /// <param name="index">Zero-based index of the record to retrieve.</param>
        /// <returns>The matching record, or null if the index is invalid.</returns>
        public DatasetRecord? GetRecordAt(int index)
        {
            if (index < 0 || index >= records.Count)
            {
                return null;
            }

            return records[index];
        }

        /// <summary>
        /// Adds a new record to the end of the in-memory sequential data structure.
        /// </summary>
        /// <param name="newRecord">The record to add.</param>
        public void AddRecord(DatasetRecord newRecord)
        {
            records.Add(newRecord);
        }

        /// <summary>
        /// Replaces the record at the given zero-based index with the supplied updated
        /// record, simulating an in-place edit of the sequential data structure.
        /// </summary>
        /// <param name="index">Zero-based index of the record to update.</param>
        /// <param name="updatedRecord">The new values to store at that index.</param>
        /// <returns>True if the update succeeded, false if the index was out of range.</returns>
        public bool EditRecord(int index, DatasetRecord updatedRecord)
        {
            if (index < 0 || index >= records.Count)
            {
                return false;
            }

            records[index] = updatedRecord;
            return true;
        }

        /// <summary>
        /// Removes the record at the given zero-based index from the in-memory sequential
        /// data structure.
        /// </summary>
        /// <param name="index">Zero-based index of the record to remove.</param>
        /// <returns>True if a record was removed, false if the index was out of range.</returns>
        public bool DeleteRecord(int index)
        {
            if (index < 0 || index >= records.Count)
            {
                return false;
            }

            records.RemoveAt(index);
            return true;
        }
    }
}
