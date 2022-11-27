using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Scoring;

public class Move
{
    public Move(ServiceModel service)
    {
        Service = service;
        Score = double.MaxValue;
    }
    
    public ServiceModel Service { get; private set; }
    
    public PackageModel BestPackage { get; private set; }
    public double Score { get; set; } = 0;
  

    public void SetNewBestPackage(PackageModel packageModel, double score)
    {
        BestPackage = packageModel;
        Score = score;

    }
}