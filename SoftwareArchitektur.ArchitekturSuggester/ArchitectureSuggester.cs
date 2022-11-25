using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester.Models;
using SoftwareArchitektur.ArchitekturSuggester.Scoring;
using SoftwareArchitektur.ArchitekturSuggester.Scoring.ScorerClasses;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester;

public class ArchitectureSuggester
{
    private readonly List<ServiceModel> _services;
    private readonly List<DependencyRelationModel> _dependencyRelations;
    private List<CommonChangeRelationModel> _commonChangeRelations;
    private readonly List<PackageModel> _packageModels = new List<PackageModel>();
    private readonly DependencyScorer _dependencyScorer;

    public ArchitectureSuggester(string completeDataFileAddress, string dependencyFileAddress, string changeFileAddress)
    {
        _services = ReadData<List<ServiceModel>>(completeDataFileAddress);
        _dependencyRelations = ReadData<List<DependencyRelationModel>>(dependencyFileAddress);
        _commonChangeRelations = ReadData<List<CommonChangeRelationModel>>(changeFileAddress);
        _dependencyScorer = new DependencyScorer(_packageModels);
        CheckIfServiceIsLeafOrRoot();
    }

    public void CalculateArchitecture(int numberOfPackages)
    {
        for (int i = 0; i < numberOfPackages; i++)
        {
            _packageModels.Add(new PackageModel($"Package {i}"));
        }
        _packageModels.Add(new PackageModel("GarbageBin"));
        var possibleMoves = new List<Move>();
        foreach (var service in _services)
        {
            if (service.IsIsolated)
            {
                _packageModels.FirstOrDefault(m => m.Name == "GarbageBin").Services.Add(service);
                continue;
            }
            CleanMoves(possibleMoves, service.Name);
            foreach (var move in possibleMoves)
            {
               
                
                _dependencyScorer.ScoreByDependency(move, service);
            }
        }
    }

    private T ReadData<T>(string fileName)
    {
        var file = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(file)!;
    }

    private void CleanMoves(List<Move> moves, string serviceName)
    {
        moves.Clear();
        for (int i = 0; i < _packageModels.Count; i++)
        {
            moves.Add(new Move(packageName: _packageModels[i].Name ,serviceName: serviceName));
        }
    }
    
    private void CheckIfServiceIsLeafOrRoot()
    {
        _services.Where(s => s.DependsOn.Count == 0).ToList().ForEach(i => i.IsRoot = true);

        var allCallers = _dependencyRelations.Select(d => d.Callee).Distinct();

        _services.Where(s => allCallers.Any(c => c != s.Name)).ToList().ForEach(s => s.IsLeaf = true);
        
        _services.Where(s => s.IsLeaf == true && s.IsRoot == true).ToList().ForEach(s => s.IsIsolated = true);
    }
}