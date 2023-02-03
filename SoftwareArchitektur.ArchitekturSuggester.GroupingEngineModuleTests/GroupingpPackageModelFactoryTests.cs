using System.Runtime.Intrinsics.X86;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester._GroupingEngine;

public class GroupingpPackageModelFactoryTests
{
    [Test]
    public void CreateGroupingPackageModelTest_EnsureFactoryWorks_CheckIfCalledByIsFilledCorrectly()
    {
        var p1 = new PackageModel("P1");
        var p2 = new PackageModel("P2");

        var packageList = new List<PackageModel>();
        var s1 = new ServiceModel("S1");
        var s2 = new ServiceModel("S2");
        s1.DependsOn.Add(new DependencyRelationServiceModel(s1, s2, 1));
        s1.ChangedWith.Add(new CommonChangeRelationServiceModel(s1, s2, 1));
        p1.AddService(s1);
        p2.AddService(s2);

        packageList.Add(p1);
        packageList.Add(p2);

        var groupingPackageModelFactory = new GroupingPackageModelFactory(new DependencyModelToGroupingDependencyConverter(), new CommonChangeToGroupingCommonChangeConverter());
        var groupingPackageModel = groupingPackageModelFactory.ConvertPackageModelsToGroupingModels(packageList);
        Assert.That(groupingPackageModel.Count, Is.EqualTo(2));
        
        var groupingPackageModel1 = groupingPackageModel.First(f => f.PackageName  == "P1");
        Assert.That(groupingPackageModel1.DependsOn.Count, Is.EqualTo(1));
        Assert.That(groupingPackageModel1.ChangesWith.Count, Is.EqualTo(1));        
        Assert.That(groupingPackageModel1.CalledBy.Count, Is.EqualTo(0));        
        
        var groupingPackageModel2 = groupingPackageModel.First(f => f.PackageName  == "P2");
        Assert.That(groupingPackageModel2.DependsOn.Count, Is.EqualTo(0));
        Assert.That(groupingPackageModel2.ChangesWith.Count, Is.EqualTo(1));
        Assert.That(groupingPackageModel2.CalledBy.Count, Is.EqualTo(1));
        
    }
}