using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Models;

public class PackageModel
{
    public PackageModel(string packageName)
    {
        PackageName = packageName;
    }

    public string PackageName { get; set; }
    
    private readonly List<ServiceModel> _services = new List<ServiceModel>();

    public void AddService(ServiceModel service)
    {
        _services.Add(service);
        service.InPackage = PackageName;
    }

    public void AddServiceRange(IEnumerable<ServiceModel> service)
    {
        _services.AddRange(service);
        service.ToList().ForEach(s => s.InPackage = PackageName);
    }

    public List<ServiceModel> GetServices()
    {
        return _services;
    }

    public double AverageChangeRate => _services.Where(s => !double.IsNaN(s.AverageChange)).Sum(s => s.AverageChange) / _services.Count;

    //Sourcehttps://socratic.org/statistics/random-variables/addition-rules-for-variances
    public double StandardDeviationOfChangeRate =>
        Math.Sqrt(_services.Sum(sd => sd.StandardDeviationChangeRate * sd.StandardDeviationChangeRate));
    
    public List<string> DependsOn => GetDependentPackages();

    [JsonIgnore]
    public List<PackageModel> PackageDependencies = new List<PackageModel>();

    private  List<string> GetDependentPackages()
    {
        return PackageDependencies.Select(d => d.PackageName).Distinct().ToList();
    }
}