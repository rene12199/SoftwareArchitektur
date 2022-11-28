using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Models;

public class PackageModel
{
    public double AverageChangeRate => _services.Where(s => !double.IsNaN(s.AverageChange)).Sum(s => s.AverageChange) / _services.Count;

    //Sourcehttps://socratic.org/statistics/random-variables/addition-rules-for-variances
    public double StandardDeviationOfChangeRate =>
        Math.Sqrt(_services.Sum(sd => sd.StandardDeviationChangeRate * sd.StandardDeviationChangeRate));
    
    public List<string> DependsOn => GetDependentPackages();
    
    public List<string> HasService => GetServices().Select(s => s.Name).ToList();

    [JsonIgnore]
    public List<PackageModel> PackageDependencies = new List<PackageModel>();
    
    public string PackageName { get; set; }
    
    [JsonIgnore]
    private readonly HashSet<ServiceModel> _services = new HashSet<ServiceModel>();
    
    public PackageModel(string packageName)
    {
        PackageName = packageName;
    }

    public void AddService(ServiceModel service)
    {
        _services.Add(service);
        service.InPackage = PackageName;
    }

    public void AddServiceRange(IEnumerable<ServiceModel> service)
    {
        foreach (var s in service)
        {
            _services.Add(s);
            s.InPackage = PackageName;

        }
    }

    public HashSet<ServiceModel> GetServices()
    {
        return _services;
    }

    
    private  List<string> GetDependentPackages()
    {
        return PackageDependencies.Select(d => d.PackageName).Distinct().ToList();
    }
}