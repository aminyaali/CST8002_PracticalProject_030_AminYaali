/*
 * CST8002 Programming Language Research Project
 * Practical Project 1
 *
 * Author: Amin Yaali
 * Professor: Gustavo Adami
 * Due Date: May 31, 2026
 */
public class DatasetRecord
{
    public string SampleID { get; set; }
    public string Species { get; set; }
    public string Taxon { get; set; }
    public string TissueAnalysed { get; set; }
    public string Location { get; set; }
    public string CollectionDate { get; set; }

    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Depth { get; set; }
    public string NumberOfIndividuals { get; set; }

    public string D13C { get; set; }
    public string D15N { get; set; }
    public string D34S { get; set; }

    public string MeHg { get; set; }
}