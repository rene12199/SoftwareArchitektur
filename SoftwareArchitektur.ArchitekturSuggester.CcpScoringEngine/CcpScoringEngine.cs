using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Autofac.Core;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Converter;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Interfaces;
using SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine.Models;
using SoftwareArchitektur.Utility.Interface;
using SoftwareArchitektur.Utility.Models;

namespace SoftwareArchitektur.ArchitekturSuggester.CcpScoringEngine;

public class CcpScoringEngine : ICcpScoringEngine
{
    private ReadOnlyCollection<PackageModel> _packageModels = null!;

    private readonly CommonChangeToCcpCommonChangeConverter _converter;

    private byte[] _snapShotHash = null!;

    private bool _lenientMode = false;


    public CcpScoringEngine(IDataProvider dataProvider)
    {
        _converter = new CommonChangeToCcpCommonChangeConverter(dataProvider);
    }

    public void SetPossiblePackages(IList<PackageModel> packagesModels)
    {
        _packageModels = packagesModels.ToList().AsReadOnly();
    }

    public IList<PackageModel> DistributeRemainingServices(IList<ServiceModel> remainingServices)
    {
        if (_packageModels == null || _packageModels.Count == 0) throw new ApplicationException("No PackageModel List set");

        _snapShotHash = CreateShaFromRemainingServices(remainingServices);
        
        var remainingServicesTuple = remainingServices.Select(s => new Tuple<ServiceModel, List<CcpScoringCommonChangeClass>>(s, new List<CcpScoringCommonChangeClass>())).ToList();

        var t = Task.Run(async () => await DistributePackages(remainingServicesTuple));
        Task.WaitAll(t);
        return _packageModels;
    }

    private async Task DistributePackages(List<Tuple<ServiceModel, List<CcpScoringCommonChangeClass>>> remainingServicesTuple)
    {
        int iterator = 0;
        await CalculateChangesByPackageAsync(remainingServicesTuple);
        while (remainingServicesTuple.Count > 0)
        {
            Console.WriteLine($"Checking Common Changes for Service {remainingServicesTuple[iterator].Item1.Name}");
            if (remainingServicesTuple[iterator].Item1.ChangedWith.Count == 0)
            {
                Console.WriteLine($"Warning: {remainingServicesTuple[iterator].Item1.Name} has no Changes, Service removed without being added to Package");
                remainingServicesTuple.RemoveAt(iterator);
                continue;
            }
            
            CcpScoringCommonChangeClass bestPackage = null!;

            bestPackage = GetBestPackage(remainingServicesTuple, iterator);


            if (bestPackage == null || bestPackage.OtherPackage == String.Empty)
            {
                Console.WriteLine($"No Package found for {remainingServicesTuple[iterator].Item1.Name}, skipping");
                iterator++;
            }
            else
            {
                Console.WriteLine($"Package {bestPackage.OtherPackage} found for {remainingServicesTuple[iterator].Item1.Name}, Service gets added");
                _packageModels.Single(s => s.PackageName == bestPackage.OtherPackage).AddService(remainingServicesTuple[iterator].Item1);
                remainingServicesTuple.RemoveAt(iterator);
            }

            if (iterator >= remainingServicesTuple.Count)
            {
                await CalculateChangesByPackageAsync(remainingServicesTuple);
                iterator = 0;
                var currentSnapShotHash = CreateShaFromRemainingServices(remainingServicesTuple.Select(s => s.Item1).ToList());

                if (_snapShotHash.SequenceEqual(currentSnapShotHash))
                {
                    if (!_lenientMode)
                    {
                        Console.WriteLine($"Warning Could Not Find more Common Changes, Activating LenientMode");
                        _lenientMode = true;
                    }
                    else
                    {
                        Console.WriteLine($"Warning Could Not Find more Common Changes, Aborting Operation");
                        break;
                    }
                }
                else
                {
                    _snapShotHash = currentSnapShotHash;
                }
            }
        }
    }

    private CcpScoringCommonChangeClass? GetBestPackage(List<Tuple<ServiceModel, List<CcpScoringCommonChangeClass>>> remainingServicesTuple, int iterator)
    {
        CcpScoringCommonChangeClass? bestPackage;
        if (_lenientMode)
        {
            bestPackage = remainingServicesTuple[iterator].Item2.FirstOrDefault(package => !string.IsNullOrWhiteSpace(package.OtherPackage));
        }
        else
        {
            bestPackage = remainingServicesTuple[iterator].Item2.First();
        }

        return bestPackage;
    }

    private async Task CalculateChangesByPackageAsync(IEnumerable<Tuple<ServiceModel, List<CcpScoringCommonChangeClass>>> remainingServicesTuple)
    {
        await Parallel.ForEachAsync(remainingServicesTuple, (tuple, token) =>
        {
            Console.WriteLine($"Updating {tuple.Item1.Name}");
            tuple?.Item2?.Clear();
            tuple.Item2.AddRange(_converter.CreateCcpCommonChangeList(tuple.Item1.ChangedWith).OrderByDescending(d => d.NumberOfChanges).ToList());
            return ValueTask.CompletedTask;
        });
    }

    private static byte[] CreateShaFromRemainingServices(IList<ServiceModel> remainingServices)
    {
        byte[] currentSnapShot;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            var sb = new StringBuilder();

            foreach (var remainingService in remainingServices)
            {
                sb.Append(remainingService.Name);
            }

            // ComputeHash - returns byte array  
            currentSnapShot = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            // Convert byte array to a string   
        }

        return currentSnapShot;
    }
}