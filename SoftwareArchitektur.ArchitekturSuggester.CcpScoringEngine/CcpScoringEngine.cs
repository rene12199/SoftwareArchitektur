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
        if (_packageModels == null || _packageModels.Count == 0) throw new ApplicationException("No Packagemodel List set");

        _snapShotHash = CreateShaFromRemainingServices(remainingServices);

        //IList<Tuple<ServiceModel, IList<CcpScoringCommonChangeClass>>> change = new List<Tuple<ServiceModel, IList<CcpScoringCommonChangeClass>>>();
        
        int iterator = 0;
        while (remainingServices.Count > 0)
        {
            Console.WriteLine($"Checking Common Changes for Service {remainingServices[iterator].Name}");
            if (remainingServices[iterator].ChangedWith.Count == 0)
            {
                Console.WriteLine($"Warning: {remainingServices[iterator].Name} has no Changes, Service removed without being added to Package");
                remainingServices.RemoveAt(iterator);
                continue;
            }
            
            //todo maybe add MultiThreading
            var changedWithPackage = _converter.CreateCcpCommonChangeList(remainingServices[iterator].ChangedWith).OrderByDescending(d => d.NumberOfChanges);
            CcpScoringCommonChangeClass bestPackage = null!;
            if (_lenientMode)
            {
                bestPackage = changedWithPackage.FirstOrDefault(package => !string.IsNullOrWhiteSpace(package.OtherPackage));
            }
            else
            {
                bestPackage = changedWithPackage.First();
            }
            
            
            if (bestPackage == null || bestPackage.OtherPackage == String.Empty)
            {
                Console.WriteLine($"No Package found for {remainingServices[iterator].Name}, skipping");
                iterator++;
            }
            else
            {
                Console.WriteLine($"Package {bestPackage.OtherPackage} found for {remainingServices[iterator].Name}, Service gets added");
                _packageModels.Single(s => s.PackageName == bestPackage.OtherPackage).AddService(remainingServices[iterator]);
                remainingServices.RemoveAt(iterator);
            }
            
            if (iterator >= remainingServices.Count)
            {
                iterator = 0;
                var currentSnapShotHash = CreateShaFromRemainingServices(remainingServices);

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

        return _packageModels;
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