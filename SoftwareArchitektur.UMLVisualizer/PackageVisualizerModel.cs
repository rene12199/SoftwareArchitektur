using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.UMLVisualizer;

public class PackageVisualizerModel
{
    public PackageVisualizerModel(string packageName)
    {
        PackageName = packageName;
    }

    public string PackageName { get; set; }

    private readonly List<ServiceModel> _services = new List<ServiceModel>();


    public double AverageChangeRate;

    //Sourcehttps://socratic.org/statistics/random-variables/addition-rules-for-variances
    public double StandardDeviationOfChangeRate =>
        Math.Sqrt(_services.Sum(sd => sd.StandardDeviationChangeRate * sd.StandardDeviationChangeRate));

    public List<string> DependsOn = new List<string>();
}