namespace SoftwareArchitektur.Utility.Models;

public class CommonChangeRelationServiceModel
{
    public ServiceModel OtherService { get; }

    public ServiceModel CurrentService { get; }

    public long NumberOfChanges;

    public string NameOfCurrentService => CurrentService.Name;

    public string NameOfOtherService => OtherService.Name;


    public CommonChangeRelationServiceModel(ServiceModel currentService, ServiceModel otherService, long numberOfChanges)
    {
        OtherService = otherService;
        CurrentService = currentService;
        NumberOfChanges = numberOfChanges;
    }
}