namespace SoftwareArchitektur.ArchitekturSuggester;

public static class Extensions
{
    // public static void CreateDependenciesToPackages(this IEnumerable<PackageModel> values)
    // {
    //     foreach (var packageModel in values) CreateDependenciesToOtherPackages(values, packageModel);
    //     values.ToList();
    // }

    // private static void CreateDependenciesToOtherPackages(IEnumerable<PackageModel> values, PackageModel packageModel)
    // {
    //     var externalDependencies = GroupDependencies(packageModel.GetServices());
    //     foreach (var externalDependency in externalDependencies) AddPackageContainingDependency(values, packageModel, externalDependency);
    // }
    //
    // private static IEnumerable<DependencyRelationModel> GroupDependencies(IEnumerable<ServiceModel> serviceModels)
    // {
    //     var allDependencies = serviceModels.SelectMany(s => s.DependsOn);
    //     var grouped = allDependencies.GroupBy(d => d.Callee).Select(gr => new DependencyRelationModel(gr.First().CallerService, gr.First().CalleeService, gr.Sum(g => g.NumberOfCalls)));
    //     return grouped;
    // }
    //
    // private static void AddPackageContainingDependency(IEnumerable<PackageModel> values, PackageModel packageModel, DependencyRelationModel externalDependency)
    // {
    //     var searched = externalDependency.Callee;
    //
    //     var foundPackage = values.First(p => p.GetServices().FirstOrDefault(s => s.Name == searched) != null);
    //
    //     if (packageModel.PackageDependencies.All(p => p.PackageName != foundPackage.PackageName))
    //     {
    //         packageModel.PackageDependencies.Add(foundPackage);
    //     }
    // }
}