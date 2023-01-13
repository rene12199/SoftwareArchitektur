using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public static class Extensions
{
    public static void CreateDependenciesToPackages(this IEnumerable<PackageModel> values)
    {
        foreach (var packageModel in values)
        {
            CreateDependenciesToOtherPackages(values, packageModel);
        }
        values.ToList();
    }

    private static void CreateDependenciesToOtherPackages(IEnumerable<PackageModel> values, PackageModel packageModel)
    {
        var externalDependencies =
            packageModel.GetServices()
                .SelectMany(s => s.DependsOn).
                DistinctBy(d => d.Callee);

        foreach (var externalDependency in externalDependencies)
        {
            AddPackageContainingDependency(values, packageModel, externalDependency);
        }
    }

    private static void AddPackageContainingDependency(IEnumerable<PackageModel> values, PackageModel packageModel, DependencyRelationModel externalDependency)
    {
        var searched = externalDependency.Callee;

        var foundPackage = values.First(p => p.GetServices().FirstOrDefault(s => s.Name == searched) != null);

        if (!packageModel.PackageDependencies.Any(p => p.PackageName == foundPackage.PackageName))
        {
            packageModel.PackageDependencies.Add(foundPackage);
        }
    }
}