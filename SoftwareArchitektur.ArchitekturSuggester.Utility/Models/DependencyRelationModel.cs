namespace SoftwareArchitektur.Utility.Models;

public class DependencyRelationModel
{
    public DependencyRelationModel(ServiceModel callerService, ServiceModel calleeService, long numberOfCalls)
    {
        CallerService = callerService;
        CalleeService = calleeService;
        NumberOfCalls = numberOfCalls;
    }

    public ServiceModel CallerService { get; }

    public string Caller => CallerService.Name;
    
    public ServiceModel CalleeService { get; }
    
    public string Callee => CalleeService.Name;
    
    public long NumberOfCalls { get; }
}