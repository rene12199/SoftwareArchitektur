using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Scoring;

public class Move
{
    public Move(ServiceModel service)
    {
        Service = service;
    }
    
    public ServiceModel Service { get; private set; }
    
    public PackageModel BestPackage { get; set; }
}