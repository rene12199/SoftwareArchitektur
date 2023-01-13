using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CCPScoringEngine.Scoring;

public class Move
{
    public Move(ServiceModel service)
    {
        Service = service;
    }
    
    public ServiceModel Service { get; private set; }
    
    public PackageModel BestPackage { get; set; }
}