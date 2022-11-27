// See https://aka.ms/new-console-template for more information

using System.Text;
using Newtonsoft.Json;
using SoftwareArchitektur.UMLVisualizer;


List<PackageVisualizerModel> modelLookUp = new List<PackageVisualizerModel>();

Console.WriteLine("Hello, World!");

var file = File.ReadAllText(@"./BestArchitecture.json");
var models = JsonConvert.DeserializeObject<List<PackageVisualizerModel>>(file)!;
modelLookUp = models;
var umlBuilder = new StringBuilder();
var umlBuilderWithLimit = new StringBuilder();
umlBuilder.Append("@startuml" + System.Environment.NewLine);
umlBuilderWithLimit.Append("@startuml" + System.Environment.NewLine);

//FindCicle(models);
foreach (var model in models)
{
    umlBuilder.Append($"component {model.PackageName}" + System.Environment.NewLine);
    if (model.DependsOn.Count < 30)
    {
        umlBuilderWithLimit.Append($"component {model.PackageName}" + System.Environment.NewLine);
    }
}

foreach (var model in models)
{
    foreach (var dependency in model.DependsOn)
    {
        umlBuilder.Append($"{model.PackageName} --> {dependency}" + System.Environment.NewLine);
    }

    if (model.DependsOn.Count < 30)
    {
        foreach (var dependency in model.DependsOn)
        {
            umlBuilderWithLimit.Append($"{model.PackageName} --> {dependency}" + System.Environment.NewLine);
        }
    }
}

umlBuilder.Append("@enduml");
umlBuilderWithLimit.Append("@enduml");

using (var fp = File.Open(@"../../../ArchitectureDeployment.puml", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(umlBuilder.ToString()));
}

using (var fp = File.Open(@"../../../ArchitectureDeploymentLimit.puml", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(umlBuilderWithLimit.ToString()));
}