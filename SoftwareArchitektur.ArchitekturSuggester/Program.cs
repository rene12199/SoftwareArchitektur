// See https://aka.ms/new-console-template for more information

using System.Text;
using MNCD.CommunityDetection.SingleLayer;
using MNCD.Core;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester;
using SoftwareArchitektur.Utility.Models;

// Console.WriteLine("Hello, World!");
// var actors = new List<Actor>
// {
//     new Actor("Actor_0"),
//     new Actor("Actor_1"),
//     new Actor("Actor_2"),
//     new Actor("Actor_3"),
//     new Actor("Actor_4"),
//     new Actor("Actor_5"),
// };
// var edges = new List<Edge>
// {
//     new Edge(actors[0], actors[1]),
//     new Edge(actors[0], actors[2]),
//     new Edge(actors[1], actors[2]),
//     new Edge(actors[2], actors[3]),
//     new Edge(actors[3], actors[4]),
//     new Edge(actors[3], actors[5]),
//     new Edge(actors[4], actors[5])
// };
// var layer = new Layer(edges);
// var network = new Network(layer, actors);
//
// var louvain = new Louvain();
// var communities = louvain.Apply(network);

var suggester = new ArchitectureSuggester("Data/FullServiceData.json", "Data/DependencyRelation.json", "Data/ChangeRelationData.json");

var bestArchitecture = suggester.CalculateArchitecture();

var content = JsonConvert.SerializeObject(bestArchitecture);
using (var fp  = File.Open(@"../../../../SoftwareArchitektur.UMLVisualizer/BestArchitecture.json", FileMode.OpenOrCreate))
{
    
    fp.Write(Encoding.ASCII.GetBytes(content));
}
