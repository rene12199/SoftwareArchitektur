using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Models;

internal class CircularDependencyRelationModel
{
    public readonly string Caller;
    public readonly string Callee;
    public readonly long NumberOfCalls;

    public CircularDependencyRelationModel(DependencyRelationServiceModel dependencyRelationServiceModel)
    {
        Callee = dependencyRelationServiceModel.Callee;
        Caller = dependencyRelationServiceModel.Caller;
        NumberOfCalls = dependencyRelationServiceModel.NumberOfCalls;
    }
}