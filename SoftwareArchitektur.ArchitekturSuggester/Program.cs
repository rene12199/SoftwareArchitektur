// See https://aka.ms/new-console-template for more information

using System.Text;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester;

var suggester = new ArchitectureSuggester("Data/FullServiceData.json", "Data/DependencyRelation.json", "Data/ChangeRelationData.json");

var bestArchitecture = suggester.CalculateArchitecture();

var content = JsonConvert.SerializeObject(bestArchitecture);
using (var fp = File.Open(@"../../../../SoftwareArchitektur.UMLVisualizer/BestArchitecture.json", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(content));
}