// See https://aka.ms/new-console-template for more information

using System.Text;
using Autofac;
using Newtonsoft.Json;
using SoftwareArchitektur.ArchitekturSuggester;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker;
using SoftwareArchitektur.ArchitekturSuggester.CircularDependencyChecker.Interfaces;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Services;

var builder = new ContainerBuilder();
builder.RegisterType<DataProvider>().As<IDataProvider>()
    .WithParameter("completeDataFileAddress", "Data/FullServiceData.json")
    .WithParameter("dependencyFileAddress", "Data/DependencyRelation.json")
    .WithParameter("changeFileAddress", "Data/ChangeRelationData.json");

builder.RegisterType<CircularDependencyChecker>().As<ICircularDependencyChecker>();

builder.RegisterType<CcpScoringEngine>().As<ICcpScoringEngine>();


var suggester = new ArchitectureSuggester(builder.Build());

var bestArchitecture = suggester.CalculateArchitecture();

var content = JsonConvert.SerializeObject(bestArchitecture);
using (var fp = File.Open(@"../../../../SoftwareArchitektur.UMLVisualizer/BestArchitecture.json", FileMode.OpenOrCreate))
{
    fp.Write(Encoding.ASCII.GetBytes(content));
}