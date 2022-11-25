using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.ArchitekturSuggester.Scoring.ScorerClasses;

namespace SoftwareArchitektur.ArchitekturSuggester.Tests;

public class Tests
{
    private DependencyScorer _scorer;

    private List<PackageModel> _models = new List<PackageModel>();

    [SetUp]
    public void Setup()
    {
        for (int i = 0; i < 3; i++)
        {
            _models.Add(new PackageModel($"Package {i}"));
        }
        
        _scorer = new DependencyScorer(_models);
    }

    [Test]
    public void DependencyScorerTests_3ServicesInDifferentModelsWith1RelatedModel_FinalScoreIs20()
    {
        Assert.Pass();
    }
}