// See https://aka.ms/new-console-template for more information

using System.Text;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SoftwareArchitektur.ExcelDataReader;
using SoftwareArchitektur.Utility.Models;

void EncodeAndSaveDataToJson()
{
    var fullServiceListJsonEncoded = JsonConvert.SerializeObject(DataHolder.ServiceList);
    var listDependencyRelation = JsonConvert.SerializeObject(DataHolder.DependencyList);
    var listChangeRelation = JsonConvert.SerializeObject(DataHolder.ChangedWithList);

    WriteDataInFile(fullServiceListJsonEncoded, @"../../../../SoftwareArchitektur.ArchitekturSuggester/Data/FullServiceData.json");

    WriteDataInFile(listDependencyRelation, @"../../../../SoftwareArchitektur.ArchitekturSuggester/Data/DependencyRelation.json");

    WriteDataInFile(listChangeRelation, @"../../../../SoftwareArchitektur.ArchitekturSuggester/Data/ChangeRelationData.json");
}

void CreateChangeRelation(IWorkbook workbook4)
{
    ISheet changesSheet = workbook4.GetSheet("CommonChanges");
    var changesEnumerator = changesSheet.GetRowEnumerator();
    while (changesEnumerator.MoveNext())
    {
        var name = ((XSSFRow)changesEnumerator.Current!).Cells.FirstOrDefault()?.ToString();
        if (string.IsNullOrWhiteSpace(name))
            break;
        if (name == "Service1") continue;

        var numberOfChanges = ((XSSFRow)changesEnumerator.Current!).Cells[2].NumericCellValue;
        var change = new CommonChangeRelationModel
        {
            NameOfCurrentService = name,
            NameOfOtherService = ((XSSFRow)changesEnumerator.Current).Cells[1].ToString()!,
            NumberOfChanges = (long)numberOfChanges
        };

        DataHolder.ChangedWithList.Add(change);
        DataHolder.ServiceList.First(lm => lm.Name == name).ChangedWith.Add(change);
    }
}

void CreateDependencyRelation(IWorkbook workbook3)
{
    ISheet dependencySheet = workbook3.GetSheet("CallerCallee");
    var dependencyEnumerator = dependencySheet.GetRowEnumerator();
    while (dependencyEnumerator.MoveNext())
    {
        var name = ((XSSFRow)dependencyEnumerator.Current!).Cells.FirstOrDefault()?.ToString();
        if (string.IsNullOrWhiteSpace(name))
            break;
        if (name == "Caller") continue;

        var numberOfCalls = ((XSSFRow)dependencyEnumerator.Current!).Cells[2].NumericCellValue;
        var dependency = new DependencyRelationModel
        {
            Caller = name,
            Callee = ((XSSFRow)dependencyEnumerator.Current).Cells[1].ToString()!,
            NumberOfCalls = (long)numberOfCalls
        };
        DataHolder.DependencyList.Add(dependency);
        DataHolder.ServiceList.First(lm => lm.Name == name).DependsOn.Add(dependency);
    }
}

void CreateServiceList(IWorkbook workbook2)
{
    ISheet serviceSheet = workbook2.GetSheet("ServiceList");
    var enumerator = serviceSheet.GetRowEnumerator();
    while (enumerator.MoveNext())
    {
        var name = ((XSSFRow)enumerator.Current!).Cells.FirstOrDefault()?.ToString();
        if (string.IsNullOrWhiteSpace(name))
            break;
        if (name.Equals("Service")) continue;

        var newDependencyObject = new ServiceModel(((XSSFRow)enumerator.Current).Cells.FirstOrDefault()?.ToString());
        DataHolder.ServiceList.Add(newDependencyObject);
    }
}

IWorkbook CreateWorkBook(string s)
{
    IWorkbook workbook1;
    using (var fs = new FileStream(s, FileMode.OpenOrCreate))
    {
        workbook1 = new XSSFWorkbook(fs);
    }

    return workbook1;
}

void WriteDataInFile(string s, string fileName)
{
    using (var fp = File.Open(fileName, FileMode.OpenOrCreate))
    {
        byte[] data = new UTF8Encoding(true).GetBytes(s);

        fp.Write(data, 0, data.Length);
    }
}

Console.WriteLine("Reading Excel");

var newFile = @".\Data.xlsx";

IWorkbook workbook = null;
workbook = CreateWorkBook(newFile);

CreateServiceList(workbook);

CreateDependencyRelation(workbook);

CreateChangeRelation(workbook);

EncodeAndSaveDataToJson();