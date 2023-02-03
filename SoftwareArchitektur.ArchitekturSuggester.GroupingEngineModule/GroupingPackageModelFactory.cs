using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class GroupingPackageModelFactory
{
    private readonly CommonChangeToGroupingCommonChangeConverter _commonChangeConverter;
    private readonly DependencyModelToGroupingDependencyConverter _dependencyConverter;

    public GroupingPackageModelFactory(DependencyModelToGroupingDependencyConverter dependencyConverter, CommonChangeToGroupingCommonChangeConverter commonChangeConverter)
    {
        _commonChangeConverter = commonChangeConverter;
        _dependencyConverter = dependencyConverter;
    }

    public IList<GroupingPackageModel> ConvertPackageModelsToGroupingModels(IList<PackageModel> packageModels)
    {
        var groupingPackageModels = new List<GroupingPackageModel>();
        foreach (var packageModel in packageModels)
        {
            var groupingPackageModel = ConvertToGroupingModel(packageModel);
            groupingPackageModels.Add(groupingPackageModel);
        }

        return groupingPackageModels;
    }

    private GroupingPackageModel ConvertToGroupingModel(PackageModel packageModel)
    {
        var groupingCommonChange = _commonChangeConverter.CreateGroupingCommonChangeModelsList(packageModel.);
        var groupingDependency = _dependencyConverter.CreateGroupingDependencyModelsList(packageModel.PackageDependencies);
    }
}