using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Interface;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;

public class ServiceModelsToGroupingServiceModel
{
    private IDataProvider _dataProvider;
    
    public ServiceModelsToGroupingServiceModel(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public IList<GroupingPackageModel> ConvertPackageModels(IList<GroupingPackageModel> packageModels)
    {
        
        
        var groupingPackageModels = new List<GroupingPackageModel>();

        foreach (var packageModel in packageModels)
        {
         
            
            
        }

        return groupingPackageModels;
    }
}