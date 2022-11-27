using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Models;

public class PackageModel
{
    public PackageModel(string packageName)
    {
        PackageName = packageName;
    }

    public string PackageName { get; set; }

    private List<ServiceModel> Services = new List<ServiceModel>();

    public void AddService(ServiceModel service)
    {
        Services.Add(service);
        service.InPackage = PackageName;
    }  
    
    public void AddServiceRange(IEnumerable<ServiceModel> service)
    {
        Services.AddRange(service);
        service.ToList().ForEach(s => s.InPackage = PackageName) ;
    }

    public List<ServiceModel> GetServices()
    {
        return Services;
    }
}