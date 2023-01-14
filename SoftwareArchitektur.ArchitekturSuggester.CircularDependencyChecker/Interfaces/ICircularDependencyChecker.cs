using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Interfaces;

public interface ICircularDependencyChecker
{
    public List<PackageModel> CreatePackages();
}