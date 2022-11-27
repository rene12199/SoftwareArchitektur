using SoftwareArchitektur.ArchitekturSuggester.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public static class Extensions
{
    public static void CreateDependenciesToPackages(this IEnumerable<PackageModel> values)
    {
        foreach (var packageModel in values)
        {
            var externalDependencies =
                packageModel.GetServices().SelectMany(s => s.DependsOn).DistinctBy(d => d.Callee);

            foreach (var externalDependency in externalDependencies)
            {
                var searched = externalDependency.Callee;

                var foundPackage = values.First(p => p.GetServices().FirstOrDefault(s => s.Name == searched) != null);

                if (!packageModel.PackageDependencies.Any(p => p.PackageName == foundPackage.PackageName))
                {
                    packageModel.PackageDependencies.Add(foundPackage);
                }
            }
        }
    }
}