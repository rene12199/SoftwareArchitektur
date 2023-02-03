namespace SoftwareArchitektur.Utility.Models;

public class CommonChangeRelationPackageModel
{
    public PackageModel OtherPackage { get; }

    public PackageModel CurrentPackage { get; }

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