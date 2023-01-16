using System.Linq.Expressions;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.TestUtility;

public class TestServiceModelFactory
{
    public List<ServiceModel> ServiceModels { get;} = new List<ServiceModel>();

    public ServiceModel CreateServiceModel(string name)
    {
        var newServiceModel = new ServiceModel(name);
        ServiceModels.Add(newServiceModel);
        return newServiceModel;
    }  
    
    public ServiceModel CreateServiceModel(string name, string packageName)
    {
        var newServiceModel = new ServiceModel(name)
        {
            InPackage = packageName
        };
        ServiceModels.Add(newServiceModel);
        return newServiceModel;
    }

    public ServiceModel CreateServiceModel(string name, Func<ServiceModel, int> expression)
    {
        var newServiceModel = new ServiceModel(name);
        expression(newServiceModel);
        ServiceModels.Add(newServiceModel);
        return newServiceModel;
    }
}