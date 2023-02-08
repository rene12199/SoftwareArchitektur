using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class GroupingEngine
{
    private readonly GroupingPackageModelFactory _groupingPackageModelFactory;
    
    private readonly LayeringEngine _layeringEngine;
    
    private readonly CohesionAttractorEngine _cohesionAttractorEngine;
    
    private IList<PackageModel> _packageModels;
    
    

    public GroupingEngine(GroupingPackageModelFactory factory,LayeringEngine layeringEngine, CohesionAttractorEngine cohesionAttractorEngine)
    {
        _groupingPackageModelFactory = factory;
        _layeringEngine = layeringEngine;
        _cohesionAttractorEngine = cohesionAttractorEngine;
    }

    public IList<PackageModel> CreateGrouping(IList<PackageModel> packageModels)
    {
        _packageModels = packageModels;
        var groupingPackageModels = _groupingPackageModelFactory.ConvertPackageModelsToGroupingModels(packageModels);
        
        _layeringEngine.CreateLayering(groupingPackageModels);

        var mergeRequests = new List<MergeRequestModel>();

        for (int i = 0; i <= _layeringEngine.GetMaxLayer(); i++)
        {
            mergeRequests.AddRange(_cohesionAttractorEngine.GroupPackages(groupingPackageModels.Where(m => m.Layer == i).ToList()));
        }

        foreach (var mergeRequestModel in mergeRequests)
        {
            MergePackages(mergeRequestModel);
        }

        return _packageModels;
    }

    private void MergePackages(MergeRequestModel models)
    {
        GerPackageModelByPackageName(models.BasePackageModel).Merge(GerPackageModelByPackageName(models.ToBeMergedModel));
    }

    private PackageModel GerPackageModelByPackageName(GroupingPackageModel package)
    {
        return _packageModels.First(m => m.PackageName == package.PackageName);
    }
}