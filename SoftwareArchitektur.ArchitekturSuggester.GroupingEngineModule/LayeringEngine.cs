using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;

namespace SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;

public class LayeringEngine
{
    public IList<GroupingPackageModel> CreateLayering(IList<GroupingPackageModel> packageModels)
    {
        int currentLayer = 0;
        CreateLayer0(packageModels);

        do
        {
            var currentLayerModel = packageModels.Where(model => model.Layer == currentLayer);
            var dependencies = currentLayerModel.SelectMany(model => model.DependsOn).ToList();
            currentLayer++;
            if (dependencies.Count == 0)
            {
                break;
            }
            foreach (var groupingDependencyModel in dependencies)
            {
                packageModels.First(p => p.PackageName == groupingDependencyModel.Callee.PackageName).SetLayer(currentLayer);
            }
            
        } while (true);

        return packageModels;
    }

    private static void CreateLayer0(IList<GroupingPackageModel> packageModels)
    {
        var layer1Models = packageModels.Where(model => model.CalledBy.Count == 0);
        foreach (var layer1Model in layer1Models)
        {
            layer1Model.SetLayer(0);
        }
    }
}