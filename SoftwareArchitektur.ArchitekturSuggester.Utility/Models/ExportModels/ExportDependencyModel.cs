namespace SoftwareArchitektur.Utility.Models.ExportModels;

public class ExportDependencyModel
{
    public string Caller { get; set; }

    public string Callee { get; set; }

    public long NumberOfCalls { get; set; }
}