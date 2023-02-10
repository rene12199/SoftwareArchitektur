namespace SoftwareArchitektur.Utility.Models.ExportModels;

public class ExportPakageModel
{
    public string PackageName { get; set; }
    public List<string> Services { get; set; } = new();
    public List<ExportDependencyModel> DependsOn { get; set; } = new();
    public List<ExportCommonChangeModel> CommonChange { get; set; } = new();
}