/*
 * CST8002 Programming Language Research Project
 * Practical Project 1
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: May 31, 2026
 *
 * References:
 * [1] CsvHelper Documentation, https://joshclose.github.io/CsvHelper/
 * [2] Microsoft Learn C# Documentation,
 *     https://learn.microsoft.com/
 */


using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Project_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Amin Yaali");
            Console.WriteLine();

            try
            {
                string fileName = "CanadianBeaufortSea_Invert_MeHg&SI_EN_FR.csv";

                var records = new List<DatasetRecord>();

                using (var reader = new StreamReader(fileName))
                {
                    // Skip metadata rows
                    for (int i = 0; i < 39; i++)
                    {
                        reader.ReadLine();
                    }

                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        MissingFieldFound = null,
                        HeaderValidated = null
                    };

                    using (var csv = new CsvReader(reader, config))
                    {
                         csv.Read();
                         csv.ReadHeader();

                        while (csv.Read())
                        {
                            DatasetRecord record = new DatasetRecord();

                            record.SampleID = csv.GetField("Sample ID");
                            record.Species = csv.GetField("Species");
                            record.Taxon = csv.GetField("Taxon");
                            record.TissueAnalysed = csv.GetField("Tissue analysed");
                            record.Location = csv.GetField("Location");
                            record.CollectionDate = csv.GetField("Collection date");

                            record.Latitude = csv.GetField("Latitude");
                            record.Longitude = csv.GetField("Longitude");
                            record.Depth = csv.GetField("Depth (m)");
                            record.NumberOfIndividuals = csv.GetField("n (ind)");

                            record.D13C = csv.GetField("d13C (�)");
                            record.D15N = csv.GetField("d15N (�)");
                            record.D34S = csv.GetField("d34S (�)");

                            record.MeHg = csv.GetField("MeHg (ng/g, DW)");
                            

                            records.Add(record);

                            // only load first 5 records
                            if (records.Count >= 5)
                                break;
                        }
                    }
                }

                foreach (var record in records)
                    {
                        Console.WriteLine($"Sample ID: {record.SampleID}");
                        Console.WriteLine($"Species: {record.Species}");
                        Console.WriteLine($"Taxon: {record.Taxon}");
                        Console.WriteLine($"Tissue: {record.TissueAnalysed}");
                        Console.WriteLine($"Location: {record.Location}");
                        Console.WriteLine($"Date: {record.CollectionDate}");
                        Console.WriteLine($"Latitude: {record.Latitude}");
                        Console.WriteLine($"Longitude: {record.Longitude}");
                        Console.WriteLine($"Depth: {record.Depth}");
                        Console.WriteLine($"Individuals: {record.NumberOfIndividuals}");
                        Console.WriteLine($"d13C: {record.D13C}");
                        Console.WriteLine($"d15N: {record.D15N}");
                        Console.WriteLine($"d34S: {record.D34S}");
                        Console.WriteLine($"MeHg: {record.MeHg}");
                        Console.WriteLine("-------------------------------------");
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}