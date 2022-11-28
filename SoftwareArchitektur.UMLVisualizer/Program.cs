// See https://aka.ms/new-console-template for more information

using System.Text;
using Newtonsoft.Json;
using SoftwareArchitektur.UMLVisualizer;


StringBuilder CreateClassDiagram(List<PackageVisualizerModel> packageVisualizerModels)
{
    var stringBuilder = new StringBuilder();

    stringBuilder.Append("@startuml" + System.Environment.NewLine);

    foreach (var package in packageVisualizerModels)
    {
        stringBuilder.Append($"class {package.PackageName}"+ "{"+ System.Environment.NewLine);

        foreach (var service in package.HasService)
        {
            stringBuilder.Append($"string {service}" + Environment.NewLine);
        }
        
        stringBuilder.Append("}"+ System.Environment.NewLine);
    }
    
    stringBuilder.Append("@enduml");
    return stringBuilder;
}

StringBuilder CreateDeploymentDiagram(List<PackageVisualizerModel> packageVisualizerModels)
{
    var dependencyUmlBuilder = new StringBuilder();
    
    dependencyUmlBuilder.Append("@startuml" + System.Environment.NewLine);
    
    foreach (var model in packageVisualizerModels)
    {
        dependencyUmlBuilder.Append($"component {model.PackageName}" + System.Environment.NewLine);
    }

    foreach (var model in packageVisualizerModels)
    {
        foreach (var dependency in model.DependsOn)
        {
            dependencyUmlBuilder.Append($"{model.PackageName} --> {dependency}" + System.Environment.NewLine);
        }
    }

    dependencyUmlBuilder.Append("@enduml");

    return dependencyUmlBuilder;
}

Console.WriteLine("Creating UML");

var file = File.ReadAllText(@"./BestArchitecture.json");
var models = JsonConvert.DeserializeObject<List<PackageVisualizerModel>>(file)!;

var dependencyUmlBuilder = CreateDeploymentDiagram(models);

var classUmlBuilder = CreateClassDiagram(models);

using (var fp = File.Open(@"../../../ArchitectureDeployment.puml", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(dependencyUmlBuilder.ToString()));
}

using (var fp = File.Open(@"../../../ArchitectureClass.puml", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(classUmlBuilder.ToString()));
}