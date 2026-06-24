/*
 * CST8002 Programming Language Research Project
 * Practical Project 2 - Project Review I
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: June 21, 2026
 *
 * References:
 * [1] Microsoft Learn C# Documentation, https://learn.microsoft.com/
 */

using Project_2.Business;
using Project_2.Model;

namespace Project_2.Presentation
{
    /// <summary>
    /// Presentation layer class. All console input and output for the application lives
    /// here; this class talks to the business layer (<see cref="RecordManager"/>) for any
    /// data operation and never touches the file system directly.
    /// </summary>
    public class MenuPresenter
    {
        /// <summary>The student's full name, displayed on screen so it always remains visible.</summary>
        private const string StudentName = "Amin Yaali";

        /// <summary>How often (every N displayed records) the student name banner repeats.</summary>
        private const int NameBannerInterval = 10;

        /// <summary>The business-layer collaborator used for all record operations.</summary>
        private readonly RecordManager recordManager;

        /// <summary>
        /// Creates a new menu presenter backed by the given business-layer record manager.
        /// </summary>
        /// <param name="recordManager">The business-layer collaborator used for record operations.</param>
        public MenuPresenter(RecordManager recordManager)
        {
            this.recordManager = recordManager;
        }

        /// <summary>
        /// Runs the main interactive menu loop until the user chooses to exit. Displays
        /// the student name banner before every interaction per assignment requirements.
        /// </summary>
        public void Run()
        {
            PrintNameBanner();

            bool initialLoadSucceeded = TryInitialLoad();

            if (!initialLoadSucceeded)
            {
                Console.WriteLine("Startup data load failed. You can still try 'Reload data' from the menu.");
            }

            bool keepRunning = true;

            while (keepRunning)
            {
                PrintMenu();
                string choice = Console.ReadLine() ?? string.Empty;
                Console.WriteLine();

                switch (choice.Trim())
                {
                    case "1":
                        ReloadData();
                        break;
                    case "2":
                        SaveData();
                        break;
                    case "3":
                        DisplayRecords();
                        break;
                    case "4":
                        CreateRecord();
                        break;
                    case "5":
                        EditRecord();
                        break;
                    case "6":
                        DeleteRecord();
                        break;
                    case "7":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please choose a number from the menu.");
                        break;
                }

                Console.WriteLine();
                PrintNameBanner();
            }

            Console.WriteLine("Goodbye!");
        }

        /// <summary>
        /// Performs the very first data load on application startup, reporting any
        /// exception to the console instead of letting the program crash.
        /// </summary>
        /// <returns>True if the load succeeded, false otherwise.</returns>
        private bool TryInitialLoad()
        {
            try
            {
                recordManager.LoadFromFile();
                Console.WriteLine($"Loaded {recordManager.Count} record(s) from the data set.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data set on startup: {ex.Message}");
                return false;
            }
        }

        /// <summary>Prints the always-visible student name banner.</summary>
        private void PrintNameBanner()
        {
            Console.WriteLine($"Program by {StudentName}");
        }

        /// <summary>Prints the main menu options.</summary>
        private void PrintMenu()
        {
            Console.WriteLine("==================== MAIN MENU ====================");
            Console.WriteLine("1. Reload data from the data set file");
            Console.WriteLine("2. Save in-memory data to a new CSV file");
            Console.WriteLine("3. Select and display record(s)");
            Console.WriteLine("4. Create a new record");
            Console.WriteLine("5. Edit an existing record");
            Console.WriteLine("6. Delete a record");
            Console.WriteLine("7. Exit");
            Console.WriteLine("====================================================");
            Console.Write("Enter your choice: ");
        }

        /// <summary>Handles menu option 1: reload data from disk, replacing in-memory data.</summary>
        private void ReloadData()
        {
            try
            {
                recordManager.LoadFromFile();
                Console.WriteLine($"Reloaded {recordManager.Count} record(s) from the data set.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reloading data set: {ex.Message}");
            }
        }

        /// <summary>Handles menu option 2: persist in-memory data to a new GUID-named CSV file.</summary>
        private void SaveData()
        {
            try
            {
                string outputPath = recordManager.SaveToFile();
                Console.WriteLine($"Saved {recordManager.Count} record(s) to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data set: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles menu option 3: lets the user choose between displaying a single record
        /// by its position, or all records currently in memory.
        /// </summary>
        private void DisplayRecords()
        {
            if (recordManager.Count == 0)
            {
                Console.WriteLine("There are no records in memory to display.");
                return;
            }

            Console.WriteLine("Display options:");
            Console.WriteLine("  a) Display a single record");
            Console.WriteLine("  b) Display all records");
            Console.Write("Enter your choice (a/b): ");
            string choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
            Console.WriteLine();

            if (choice == "a")
            {
                int index = PromptForIndex("Enter the record number to display");
                var record = recordManager.GetRecordAt(index);

                if (record == null)
                {
                    Console.WriteLine("No record exists at that position.");
                    return;
                }

                PrintRecord(index, record);
            }
            else if (choice == "b")
            {
                var all = recordManager.GetAllRecords();

                for (int i = 0; i < all.Count; i++)
                {
                    PrintRecord(i, all[i]);

                    // Repeat the student name banner every NameBannerInterval records,
                    // per assignment requirement, when displaying many records.
                    if ((i + 1) % NameBannerInterval == 0)
                    {
                        PrintNameBanner();
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        /// <summary>Handles menu option 4: prompts for field values and adds a new record.</summary>
        private void CreateRecord()
        {
            Console.WriteLine("Enter values for the new record.");

            var newRecord = new DatasetRecord(
                sampleId: PromptForField("Sample ID"),
                species: PromptForField("Species"),
                taxon: PromptForField("Taxon"),
                tissueAnalysed: PromptForField("Tissue analysed"),
                location: PromptForField("Location"),
                collectionDate: PromptForField("Collection date"),
                latitude: PromptForField("Latitude"),
                longitude: PromptForField("Longitude"),
                depth: PromptForField("Depth (m)"),
                numberOfIndividuals: PromptForField("n (ind)"),
                d13C: PromptForField("d13C (\u2030)"),
                d15N: PromptForField("d15N (\u2030)"),
                d34S: PromptForField("d34S (\u2030)"),
                meHg: PromptForField("MeHg (ng/g, DW)"));

            recordManager.AddRecord(newRecord);
            Console.WriteLine($"New record added at position {recordManager.Count - 1}.");
        }

        /// <summary>Handles menu option 5: selects a record by position and overwrites its fields.</summary>
        private void EditRecord()
        {
            if (recordManager.Count == 0)
            {
                Console.WriteLine("There are no records in memory to edit.");
                return;
            }

            int index = PromptForIndex("Enter the record number to edit");
            var existing = recordManager.GetRecordAt(index);

            if (existing == null)
            {
                Console.WriteLine("No record exists at that position.");
                return;
            }

            Console.WriteLine("Current values are shown in brackets. Press Enter to keep a value unchanged.");

            var updated = new DatasetRecord(
                sampleId: PromptForFieldWithDefault("Sample ID", existing.SampleID),
                species: PromptForFieldWithDefault("Species", existing.Species),
                taxon: PromptForFieldWithDefault("Taxon", existing.Taxon),
                tissueAnalysed: PromptForFieldWithDefault("Tissue analysed", existing.TissueAnalysed),
                location: PromptForFieldWithDefault("Location", existing.Location),
                collectionDate: PromptForFieldWithDefault("Collection date", existing.CollectionDate),
                latitude: PromptForFieldWithDefault("Latitude", existing.Latitude),
                longitude: PromptForFieldWithDefault("Longitude", existing.Longitude),
                depth: PromptForFieldWithDefault("Depth (m)", existing.Depth),
                numberOfIndividuals: PromptForFieldWithDefault("n (ind)", existing.NumberOfIndividuals),
                d13C: PromptForFieldWithDefault("d13C (\u2030)", existing.D13C),
                d15N: PromptForFieldWithDefault("d15N (\u2030)", existing.D15N),
                d34S: PromptForFieldWithDefault("d34S (\u2030)", existing.D34S),
                meHg: PromptForFieldWithDefault("MeHg (ng/g, DW)", existing.MeHg));

            recordManager.EditRecord(index, updated);
            Console.WriteLine($"Record at position {index} updated.");
        }

        /// <summary>Handles menu option 6: selects a record by position and removes it.</summary>
        private void DeleteRecord()
        {
            if (recordManager.Count == 0)
            {
                Console.WriteLine("There are no records in memory to delete.");
                return;
            }

            int index = PromptForIndex("Enter the record number to delete");
            var existing = recordManager.GetRecordAt(index);

            if (existing == null)
            {
                Console.WriteLine("No record exists at that position.");
                return;
            }

            PrintRecord(index, existing);
            Console.Write("Are you sure you want to delete this record? (y/n): ");
            string confirm = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (confirm == "y")
            {
                recordManager.DeleteRecord(index);
                Console.WriteLine("Record deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        /// <summary>Prints a single record to the console with a labelled position.</summary>
        /// <param name="index">Zero-based position of the record in memory.</param>
        /// <param name="record">The record to print.</param>
        private void PrintRecord(int index, DatasetRecord record)
        {
            Console.WriteLine($"--- Record #{index} ---");
            Console.WriteLine($"Sample ID: {record.SampleID}");
            Console.WriteLine($"Species: {record.Species}");
            Console.WriteLine($"Taxon: {record.Taxon}");
            Console.WriteLine($"Tissue analysed: {record.TissueAnalysed}");
            Console.WriteLine($"Location: {record.Location}");
            Console.WriteLine($"Collection date: {record.CollectionDate}");
            Console.WriteLine($"Latitude: {record.Latitude}");
            Console.WriteLine($"Longitude: {record.Longitude}");
            Console.WriteLine($"Depth (m): {record.Depth}");
            Console.WriteLine($"n (ind): {record.NumberOfIndividuals}");
            Console.WriteLine($"d13C (\u2030): {record.D13C}");
            Console.WriteLine($"d15N (\u2030): {record.D15N}");
            Console.WriteLine($"d34S (\u2030): {record.D34S}");
            Console.WriteLine($"MeHg (ng/g, DW): {record.MeHg}");
            Console.WriteLine("-------------------------------------");
        }

        /// <summary>Prompts the user for a field value, allowing blank entries.</summary>
        /// <param name="label">The field label shown to the user.</param>
        /// <returns>The entered value, or an empty string if nothing was entered.</returns>
        private string PromptForField(string label)
        {
            Console.Write($"{label}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Prompts the user for a field value, showing the current value as a default that
        /// is kept if the user presses Enter without typing anything.
        /// </summary>
        /// <param name="label">The field label shown to the user.</param>
        /// <param name="currentValue">The current value, shown in brackets and used as the default.</param>
        /// <returns>The new value, or the unchanged current value.</returns>
        private string PromptForFieldWithDefault(string label, string currentValue)
        {
            Console.Write($"{label} [{currentValue}]: ");
            string input = Console.ReadLine() ?? string.Empty;
            return input.Length == 0 ? currentValue : input;
        }

        /// <summary>
        /// Prompts the user for a zero-based record index, re-prompting until a valid
        /// integer is entered.
        /// </summary>
        /// <param name="prompt">The prompt text shown to the user.</param>
        /// <returns>The parsed zero-based index.</returns>
        private int PromptForIndex(string prompt)
        {
            int index;

            while (true)
            {
                Console.Write($"{prompt} (0-{recordManager.Count - 1}): ");
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out index))
                {
                    return index;
                }

                Console.WriteLine("Please enter a valid whole number.");
            }
        }
    }
}
