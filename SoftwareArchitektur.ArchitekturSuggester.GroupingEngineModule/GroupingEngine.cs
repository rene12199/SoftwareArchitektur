using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class GroupingEngine
{
    private readonly GroupingPackageModelFactory _groupingPackageModelFactory;

    public GroupingEngine()
    {
        _groupingPackageModelFactory = new GroupingPackageModelFactory(new DependencyModelToGroupingDependencyConverter(), new CommonChangeToGroupingCommonChangeConverter());
    }

    public IList<GroupingPackageModel> CreateGrouping(IList<PackageModel> _packageModels)
    {
        var groupingPackageModels = _groupingPackageModelFactory.ConvertPackageModelsToGroupingModels(_packageModels);
        var layerCreator = new LayeringEngine();
        layerCreator.CreateLayering(groupingPackageModels);

        return groupingPackageModels;
    }
}