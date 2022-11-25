// See https://aka.ms/new-console-template for more information

using SoftwareArchitektur.ArchitekturSuggester;

Console.WriteLine("Hello, World!");


var suggester = new ArchitectureSuggester("FullServiceData.json", "DependencyRelation.json", "ChangeRelationData.json");

suggester.CalculateArchitecture(3);