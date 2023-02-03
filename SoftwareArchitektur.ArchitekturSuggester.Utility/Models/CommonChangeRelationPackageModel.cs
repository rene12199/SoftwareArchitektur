namespace SoftwareArchitektur.Utility.Models;

public class CommonChangeRelationPackageModel
{
    public PackageModel OtherService { get; }

    public PackageModel CurrentService { get; }

    public long NumberOfChanges {get;}

    public string NameOfCurrentService => CurrentService.PackageName;
    
    public string NameOfOtherService => OtherService.PackageName;
    
    
    public CommonChangeRelationPackageModel(PackageModel currentService, PackageModel otherService, long numberOfChanges)
    {
        OtherService = otherService;
        CurrentService = currentService;
        NumberOfChanges = numberOfChanges;
    }
    
    

}