using Newtonsoft.Json;

namespace SoftwareArchitektur.Utility.Models;

public class CommonChangeRelationPackageModel
{
    [JsonIgnore] public PackageModel OtherPackage { get; }

    [JsonIgnore] public PackageModel CurrentPackage { get; }

    public long NumberOfChanges { get; }

    public string NameOfCurrentService => CurrentPackage.PackageName;

    public string NameOfOtherService => OtherPackage.PackageName;


    public CommonChangeRelationPackageModel(PackageModel currentPackage, PackageModel otherPackage, long numberOfChanges)
    {
        OtherPackage = otherPackage;
        CurrentPackage = currentPackage;
        NumberOfChanges = numberOfChanges;
    }
}