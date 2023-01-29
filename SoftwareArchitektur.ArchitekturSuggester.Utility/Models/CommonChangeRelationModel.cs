namespace SoftwareArchitektur.Utility.Models;

public class CommonChangeRelationModel
{
    public ServiceModel OtherService { get; }

    public ServiceModel CurrentService { get; }

    public long NumberOfChanges = 0;

    public string NameOfCurrentService => CurrentService.Name;
    
    public string NameOfOtherService => OtherService.Name;
    
    
    public CommonChangeRelationModel(ServiceModel currentService, ServiceModel otherService, long numberOfChanges)
    {
        OtherService = otherService;
        CurrentService = currentService;
        NumberOfChanges = numberOfChanges;
    }
    
    

}