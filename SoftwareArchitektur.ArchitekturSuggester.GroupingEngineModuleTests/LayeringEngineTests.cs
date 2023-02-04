using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine;
using SoftwareArchitektur.ArchitekturSuggester.GroupingEngine.Model;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester._GroupingEngine;

public class LayeringEngineTests
{
    [Test]
    public void LayeringEngineTests_EvenLayering_DenseGraph_Returns3LayersWith3Packages()
    {
        // Arrange
        var groupPackageModels = new List<GroupingPackageModel>();
        var packageModels = new List<PackageModel>();


        for (int i = 9; i > 0; i--)
        {
            var groupingDependencies = new List<GroupingDependendencyModel>();

            var packageModel = new PackageModel($"P{i}");
            packageModels.Add(packageModel);


            for (int j = 0; j < 3; j++)
            {
                if (i < 7 && i > 3)
                {
                    var dependsOn = packageModels.FirstOrDefault(x => x.PackageName == $"P{9 - j}");
                    var dependency = new GroupingDependendencyModel(packageModel, dependsOn, 1);
                    if (dependsOn != null) groupingDependencies.Add(dependency);
                    groupPackageModels.First(m => m.PackageName == $"P{9 - j}").CalledBy.Add(dependency);
                }
                else if (i < 4 && i > -1)
                {
                    var dependsOn = packageModels.FirstOrDefault(x => x.PackageName == $"P{6 - j}");

                    var dependency = new GroupingDependendencyModel(packageModel, dependsOn, 1);
                    if (dependsOn != null) groupingDependencies.Add(dependency);
                    groupPackageModels.First(m => m.PackageName == $"P{6 - j}").CalledBy.Add(dependency);
                }
            }

            var newGroupPackageModel = new GroupingPackageModel(packageModel, groupingDependencies, new List<GroupingCommonChangeModel>());
            groupPackageModels.Add(newGroupPackageModel);
        }

        var layeringEngine = new LayeringEngine();

        // Act
        var result = layeringEngine.CreateLayering(groupPackageModels);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(9));
        Assert.That(result.Count(m => m.Layer == 0), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 1), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 2), Is.EqualTo(3));
    }


    [Test]
    public void LayeringEngineTests_EvenLayering_ShallowGraph_Returns3LayersWith3Packages()
    {
        // Arrange
        var groupPackageModels = new List<GroupingPackageModel>();
        var packageModels = new List<PackageModel>();


        for (int i = 9; i > 0; i--)
        {
            var groupingDependencies = new List<GroupingDependendencyModel>();

            var packageModel = new PackageModel($"P{i}");
            packageModels.Add(packageModel);


            var dependsOn = packageModels.FirstOrDefault(x => x.PackageName == $"P{i + 3}");
            if (dependsOn != null)
            {
                var dependency = new GroupingDependendencyModel(packageModel, dependsOn, 1);
                groupingDependencies.Add(dependency);
                groupPackageModels.First(m => m.PackageName == $"P{i + 3}").CalledBy.Add(dependency);
            }

            var newGroupPackageModel = new GroupingPackageModel(packageModel, groupingDependencies, new List<GroupingCommonChangeModel>());
            groupPackageModels.Add(newGroupPackageModel);
        }

        var layeringEngine = new LayeringEngine();

        // Act
        var result = layeringEngine.CreateLayering(groupPackageModels);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(9));
        Assert.That(result.Count(m => m.Layer == 0), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 1), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 2), Is.EqualTo(3));
    } 
    
    [Test]
    public void LayeringEngineTests_UnevenLayering_ShallowGraph_Returns3LayersWith3Packages()
    {
        // Arrange
        var groupPackageModels = new List<GroupingPackageModel>();
        var packageModels = new List<PackageModel>();


        for (int i = 9; i > 0; i--)
        {
            if(i == 9) continue;
            var groupingDependencies = new List<GroupingDependendencyModel>();

            var packageModel = new PackageModel($"P{i}");
            packageModels.Add(packageModel);


            var dependsOn = packageModels.FirstOrDefault(x => x.PackageName == $"P{i + 3}");
            if (dependsOn != null)
            {
                var dependency = new GroupingDependendencyModel(packageModel, dependsOn, 1);
                groupingDependencies.Add(dependency);
                groupPackageModels.First(m => m.PackageName == $"P{i + 3}").CalledBy.Add(dependency);
            }

            var newGroupPackageModel = new GroupingPackageModel(packageModel, groupingDependencies, new List<GroupingCommonChangeModel>());
            groupPackageModels.Add(newGroupPackageModel);
        }

        var layeringEngine = new LayeringEngine();

        // Act
        var result = layeringEngine.CreateLayering(groupPackageModels);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(8));
        Assert.That(result.Count(m => m.Layer == 0), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 1), Is.EqualTo(3));
        Assert.That(result.Count(m => m.Layer == 2), Is.EqualTo(2));
    }
}