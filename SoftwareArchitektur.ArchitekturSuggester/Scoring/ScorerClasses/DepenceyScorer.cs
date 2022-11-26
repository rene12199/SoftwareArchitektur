using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Scoring.ScorerClasses;

public class DependencyScorer
{
    private List<PackageModel> Packages { get; }

    public DependencyScorer(List<PackageModel> packages)
    {
        Packages = packages;
    }

    public void ScoreByDependency(Move move, ServiceModel service)
    {
        long finalScore = 0;
        var currentPackage = Packages.First(f => f.Name == move.PackageName);
        var dependenciesForService = service.DependsOn;
        finalScore = ScoreDependency(dependenciesForService, currentPackage, finalScore);

        move.DependencyScore = finalScore;
    }

    private static long ScoreDependency(List<DependencyRelationModel> dependenciesForService, PackageModel currentPackage, long finalScore)
    {
        foreach(var dependency in dependenciesForService)
        {
            if(!currentPackage.Services.Any(d => d.Name == dependency.Callee))
            {
                finalScore = finalScore + dependency.NumberOfCalls;
            }
        }

        return finalScore;
    }
}