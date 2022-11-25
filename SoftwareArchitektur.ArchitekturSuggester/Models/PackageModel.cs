using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Models;

public class PackageModel
{
    public PackageModel(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    
    public List<ServiceModel> Services { get; set; } = new List<ServiceModel>();

    public IEnumerable<string> GetAllServiceNames()
    {
        return Services.Select(s => s.Name).ToList();
    }
}