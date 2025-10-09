namespace Krafter.UI.Web.Client.Features.Tenants;

public static class TablesToCopy
{
    public static List<TableToCopy> Data { get; set; } = new ()
    {
       
        new TableToCopy { Name = "Unit", DisplayName = "Unit" },
        new TableToCopy { Name = "Country", DisplayName = "Country" },
        new TableToCopy { Name = "Language", DisplayName = "Language" },
        new TableToCopy { Name = "Currency", DisplayName = "Currency" },
        new TableToCopy { Name = "DocumentType", DisplayName = "Document Type" },
        new TableToCopy { Name = "StorageFormat", DisplayName = "Storage Format" },
        new TableToCopy { Name = "Risk", DisplayName = "Risk" },
        new TableToCopy { Name = "TrainingRisk", DisplayName = "Training Risk" },
        new TableToCopy { Name = "TrainingType", DisplayName = "Training Type" },
        new TableToCopy { Name = "EvaluationMethod", DisplayName = "Evaluation Method" },
        new TableToCopy { Name = "StorageArea", DisplayName = "StorageArea" },
        

    };
}

public class TableToCopy
{
    public string Name { get; set; }   
    public string DisplayName { get; set; }
}