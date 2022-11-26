using SoftwareArchitektur.ArchitekturSuggester.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Scoring.ScorerClasses;

public class CCPScorer
{
    private List<PackageModel> Packages { get; }

    public CCPScorer(List<PackageModel> packages)
    {
        Packages = packages;

    }
}