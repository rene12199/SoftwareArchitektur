﻿using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class LayeringEngine
{

    private int _currentLayer = 0;
    public IList<GroupingPackageModel> CreateLayering(IList<GroupingPackageModel> packageModels)
    {
        _currentLayer = 0;
        CreateLayer0(packageModels);

        do
        {
            var currentLayerModel = packageModels.Where(model => model.Layer == _currentLayer);
            var dependencies = currentLayerModel.SelectMany(model => model.DependsOn).ToList();
            Console.WriteLine($"Currentlayer {_currentLayer} with {dependencies.Count()}");
            _currentLayer++;
            if (dependencies.Count == 0 || dependencies.All(s => s.HasBeenLookedAt  == true))
            {
                break;
            }
            foreach (var groupingDependencyModel in dependencies)
            {
                groupingDependencyModel.HasBeenLookedAt = true;
                packageModels.First(p => p.PackageName == groupingDependencyModel.Callee.PackageName).SetLayer(_currentLayer);
            }

           
        } while (true);

        return packageModels;
    }

    public int GetMaxLayer() => _currentLayer;

    private static void CreateLayer0(IList<GroupingPackageModel> packageModels)
    {
        var layer1Models = packageModels.Where(model => model.CalledBy.Count == 0);
        foreach (var layer1Model in layer1Models)
        {
            layer1Model.SetLayer(0);
        }
        Console.WriteLine($"Layer 0 Created with {layer1Models.Count()}");
    }
}