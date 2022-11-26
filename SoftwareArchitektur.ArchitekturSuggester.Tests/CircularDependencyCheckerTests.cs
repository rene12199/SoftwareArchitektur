using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.Tests;

public class CircularDependencyCheckerTests
{
    [Test]
    public void CircularDependencyCheckerTests_Only2Dependencies_ReturnsPackageWith1Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2"
                }
            }
        });
        serviceModels.Add(new ServiceModel("S2"));
        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.AreEqual(1, result.Count);
        Assert.NotNull(result.FirstOrDefault().Services.FirstOrDefault(s => s.Name == "S1"));
    } 
    
    [Test]
    public void CircularDependencyCheckerTests_Only3DependenciesWithCiruclarDependency_ReturnsPackageWith3Services()
    {
        //Arrange
        var serviceModels = new List<ServiceModel>();
        serviceModels.Add(new ServiceModel("S1")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S1",
                    Callee = "S2",
                    NumberOfCalls = 10,
                }
            }
        }); 
        serviceModels.Add(new ServiceModel("S2")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S2",
                    Callee = "S3"
                }
            }
        });       
        serviceModels.Add(new ServiceModel("S3")
        {
            DependsOn =
            {
                new DependencyRelationModel()
                {
                    Caller = "S3",
                    Callee = "S1"
                }
            }
        });
      
        var checker = new CircularDependencyChecker(serviceModels);
        //Assert
        var result = checker.CreatePackages();

        //Act
        Assert.AreEqual(1, result.Count);
        Assert.NotNull(result.FirstOrDefault().Services.FirstOrDefault(s => s.Name == "S1"));
        Assert.NotNull(result.FirstOrDefault().Services.FirstOrDefault(s => s.Name == "S2"));
        Assert.NotNull(result.FirstOrDefault().Services.FirstOrDefault(s => s.Name == "S3"));
    }
}