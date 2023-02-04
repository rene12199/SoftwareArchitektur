using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class GroupingPackageModelFactory
{
    private readonly CommonChangeToGroupingCommonChangeConverter _commonChangeConverter;
    private readonly DependencyModelToGroupingDependencyConverter _dependencyConverter;
    private readonly IList<GroupingPackageModel> _groupingPackageModels= new List<GroupingPackageModel>();
    private readonly List<GroupingDependendencyModel> _groupingDependencyModels= new List<GroupingDependendencyModel>();

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

            _groupingPackageModels.Add(groupingPackageModel);
        }
        
        CreateCalledByRelation();

        return _groupingPackageModels;
    }

    private void CreateCalledByRelation()
    {
        foreach (var dependencyModel in _groupingDependencyModels)
        {
            var packageModel = _groupingPackageModels.FirstOrDefault(x => x.PackageName == dependencyModel.Callee.PackageName);

            packageModel?.CalledBy.Add(dependencyModel);
        }
    }

    private GroupingPackageModel ConvertToGroupingModel(PackageModel packageModel)
    {
        var groupingCommonChange = _commonChangeConverter.CreateGroupingCommonChangeModelsList(packageModel.ChangesWith);
        var groupingDependency = _dependencyConverter.CreateGroupingDependencyModelsList(packageModel.DependsOn);
        _groupingDependencyModels.AddRange(groupingDependency);
        
        return new GroupingPackageModel(packageModel, groupingDependency, groupingCommonChange);
    }
}