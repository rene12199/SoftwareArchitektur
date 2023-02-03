using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.Utility.Services;

public class DataProvider : IDataProvider
{
    private readonly ReadOnlyCollection<ServiceModel> _servicesLookUp;
    private readonly ReadOnlyCollection<DependencyRelationServiceModel> _dependencyRelations;
    private readonly ReadOnlyCollection<CommonChangeRelationServiceModel> _changeRelations;

    public DataProvider(string completeDataFileAddress, string dependencyFileAddress, string changeFileAddress)
    {
        _servicesLookUp = ReadData<ReadOnlyCollection<ServiceModel>>(completeDataFileAddress);
        _dependencyRelations = ReadData<ReadOnlyCollection<DependencyRelationServiceModel>>(dependencyFileAddress);
        _changeRelations = ReadData<ReadOnlyCollection<CommonChangeRelationServiceModel>>(changeFileAddress);
        CheckIfServiceIsLeafOrRoot();
    }

    private T ReadData<T>(string fileName)
    {
        var file = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(file)!;
    }

    private void CheckIfServiceIsLeafOrRoot()
    {
        CheckIfRoot();

        CheckIfLeaf();

        CheckIfHasChangeRelation();
    }

    private void CheckIfHasChangeRelation()
    {
        var hasNoChangeRelationServiceModels = _servicesLookUp.Where(s => _changeRelations.Any(cc => cc.NameOfCurrentService == s.Name || cc.NameOfOtherService == s.Name));

        foreach (var serviceModel in hasNoChangeRelationServiceModels) serviceModel.HasChangeRelation = true;
    }

    private void CheckIfRoot()
    {
        _servicesLookUp.Where(s => s.DependsOn.Count == 0).ToList().ForEach(i => i.IsRoot = true);
    }

    private void CheckIfLeaf()
    {
        var allCallees = _dependencyRelations.Select(d => d.CalleeService).Distinct().ToList();

        foreach (var callee in allCallees) _servicesLookUp.First(s => s.Name == callee.Name).IsLeaf = false;
    }

    public IList<ServiceModel> GetServices()
    {
        return _servicesLookUp.ToList();
    }

    public IList<DependencyRelationServiceModel> GetDependencyRelation()
    {
        return _dependencyRelations.ToList();
    }

    public IList<CommonChangeRelationServiceModel> GetCommonChangeRelation()
    {
        return _changeRelations.ToList();
    }
}