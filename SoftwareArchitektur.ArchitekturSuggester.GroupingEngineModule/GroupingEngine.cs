using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;
//todo make new GroupingEngine Which groups by Calledby and Calls

public class GroupingEngine
{
    private readonly GroupingPackageModelFactory _groupingPackageModelFactory;

    private readonly LayeringEngine _layeringEngine;

    private readonly CohesionAttractorEngine _cohesionAttractorEngine;

    private IList<PackageModel> _packageModels;


    public GroupingEngine(GroupingPackageModelFactory factory, LayeringEngine layeringEngine, CohesionAttractorEngine cohesionAttractorEngine)
    {
        _groupingPackageModelFactory = factory;
        _layeringEngine = layeringEngine;
        _cohesionAttractorEngine = cohesionAttractorEngine;
    }

    public void GroupPackages(IList<PackageModel> packageModels)
    {
        _packageModels = packageModels;
        var groupingPackageModels = _groupingPackageModelFactory.ConvertPackageModelsToGroupingModels(packageModels);

        _layeringEngine.CreateLayering(groupingPackageModels);

        var mergeRequests = new List<MergeRequestModel>();
        _cohesionAttractorEngine.SetPackageLookup(groupingPackageModels);

        for (int i = 0; i <= _layeringEngine.GetMaxLayer(); i++)
        {
            var layerPackage = groupingPackageModels.Where(m => m.Layer == i).ToList();
            if (layerPackage.Count > 0)
            {
                mergeRequests.AddRange(_cohesionAttractorEngine.GroupPackages(layerPackage));
            }
        }
        
        foreach (var mergeRequestModel in mergeRequests)
        {
            MergePackages(mergeRequestModel);
        }
    }

    private void MergePackages(MergeRequestModel models)
    {
        Console.WriteLine($"Merging {models.ToBeMergedModel.PackageName} into {models.BasePackageModel.PackageName}");
        GeTPackageModelByPackageName(models.BasePackageModel).Merge(GeTPackageModelByPackageName(models.ToBeMergedModel));
    }

    private PackageModel GeTPackageModelByPackageName(GroupingPackageModel package)
    {
        return _packageModels.First(m => m.PackageName == package.PackageName);
    }
}