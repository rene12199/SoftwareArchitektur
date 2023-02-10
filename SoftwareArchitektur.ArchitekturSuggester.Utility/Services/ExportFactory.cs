using SoftwareArchitektur.Utility.Models;
using SoftwareArchitektur.Utility.Models.ExportModels;

namespace SoftwareArchitektur.Utility.Services;

public class ExportFactory
{
    public IList<ExportPakageModel> ConvertPackagesToExportPackages(IList<PackageModel> models)
    {
        var exportModels = new List<ExportPakageModel>();

        foreach (var packageModel in models)
        {
            var exportModel = new ExportPakageModel
            {
                PackageName = packageModel.PackageName,
                Services = packageModel.Services.Select(p => p.Name).ToList(),
                DependsOn = packageModel.DependsOn.Select(x => new ExportDependencyModel
                {
                    NumberOfCalls = x.NumberOfCalls,
                    Caller = x.Caller,
                    Callee = x.Callee
                    
                }).ToList(),
                CommonChange = packageModel.ChangesWith.Select(x => new ExportCommonChangeModel
                {
                    NumberOfChanges = x.NumberOfChanges,
                    NameOfCurrentService = x.NameOfCurrentService,
                    NameOfOtherService = x.NameOfOtherService
                }).ToList()
            };

            exportModels.Add(exportModel);
        }
        
        return exportModels;
    }
}