namespace SoftwareArchitektur.Utility.Models;

public class DependencyRelationPackageModel
{
    public PackageModel CallerService { get; }

    public string Caller => CallerService.PackageName;

    public PackageModel CalleeService { get; } 

    public string Callee => CalleeService.PackageName;

    public long NumberOfCalls { get; }

    public DependencyRelationPackageModel(PackageModel callerService, PackageModel calleeService, long numberOfCalls)
    {
        CallerService = callerService;
        CalleeService = calleeService;
        NumberOfCalls = numberOfCalls;
    }
}