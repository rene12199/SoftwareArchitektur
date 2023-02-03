using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class GroupingPackageModelFactory
{
    private readonly CommonChangeToGroupingCommonChangeConverter _commonChangeConverter;
    private readonly DependencyModelToGroupingDependencyConverter _dependencyConverter;
    private readonly IList<GroupingPackageModel> _groupingPackageModels= new List<GroupingPackageModel>();

    public GroupingPackageModelFactory(DependencyModelToGroupingDependencyConverter dependencyConverter, CommonChangeToGroupingCommonChangeConverter commonChangeConverter)
    {
        _commonChangeConverter = commonChangeConverter;
        _dependencyConverter = dependencyConverter;
    }

    public IList<GroupingPackageModel> ConvertPackageModelsToGroupingModels(IList<PackageModel> packageModels)
    {
     
        foreach (var packageModel in packageModels)
        {
            var groupingPackageModel = ConvertToGroupingModel(packageModel);

            CreateCalledByRelation(groupingPackageModel);
            
            _groupingPackageModels.Add(groupingPackageModel);
        }

        return _groupingPackageModels;
    }

    private void CreateCalledByRelation(GroupingPackageModel groupingPackageModel)
    {
        foreach (var dependencyModel in groupingPackageModel.DependsOn)
        {
            var packageModel = _groupingPackageModels.FirstOrDefault(x => x.PackageName == dependencyModel.Caller.PackageName);

            packageModel?.CalledBy.Add(dependencyModel);
        }
    }

    private GroupingPackageModel ConvertToGroupingModel(PackageModel packageModel)
    {
        var groupingCommonChange = _commonChangeConverter.CreateGroupingCommonChangeModelsList(packageModel.ChangesWith);
        var groupingDependency = _dependencyConverter.CreateGroupingDependencyModelsList(packageModel.DependsOn);
        return new GroupingPackageModel(packageModel, groupingDependency, groupingCommonChange);
    }
}