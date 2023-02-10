// See https://aka.ms/new-console-template for more information

using System.Text;
using Newtonsoft.Json;
using SoftwareArchitektur.Utility.Models.ExportModels;

StringBuilder CreateClassDiagram(List<ExportPakageModel> packageVisualizerModels)
{
    var stringBuilder = new StringBuilder();

    stringBuilder.Append("@startuml" + Environment.NewLine);

    foreach (var package in packageVisualizerModels)
    {
        stringBuilder.Append($"class {package.PackageName}" + "{" + Environment.NewLine);

        foreach (var service in package.Services) stringBuilder.Append($"string {service}" + Environment.NewLine);

        stringBuilder.Append("}" + Environment.NewLine);
    }

    stringBuilder.Append("@enduml");
    return stringBuilder;
}

StringBuilder CreateDeploymentDiagram(List<ExportPakageModel> packageVisualizerModels)
{
    var dependencyUmlBuilder = new StringBuilder();

    dependencyUmlBuilder.Append("@startuml" + Environment.NewLine);

    foreach (var model in packageVisualizerModels) dependencyUmlBuilder.Append($"component {model.PackageName}" + Environment.NewLine);

    foreach (var model in packageVisualizerModels)
    foreach (var dependency in model.DependsOn)
        dependencyUmlBuilder.Append($"{model.PackageName} --> {dependency.Callee}" + Environment.NewLine);

    dependencyUmlBuilder.Append("@enduml");

    return dependencyUmlBuilder;
}

Console.WriteLine("Creating UML");

var file = File.ReadAllText(@"./BestArchitecture.json");
var models = JsonConvert.DeserializeObject<List<ExportPakageModel>>(file)!;

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