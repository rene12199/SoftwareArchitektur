using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Models;

internal class CircularDependencyRelationModel
{
    public readonly string Caller;
    public readonly string Callee;
    public readonly long NumberOfCalls;

    public CircularDependencyRelationModel(DependencyRelationModel dependencyRelationModel)
    {
        Callee = dependencyRelationModel.Callee;
        Caller = dependencyRelationModel.Caller;
        NumberOfCalls = dependencyRelationModel.NumberOfCalls;
    }
}