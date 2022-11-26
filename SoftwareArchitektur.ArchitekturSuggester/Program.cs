// See https://aka.ms/new-console-template for more information

using MNCD.CommunityDetection.SingleLayer;
using MNCD.Core;
using SoftwareArchitektur.ArchitekturSuggester;

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


var suggester = new ArchitectureSuggester("FullServiceData.json", "DependencyRelation.json", "ChangeRelationData.json");

suggester.CalculateArchitecture(3);