using System;
using TrainFair.Constants;
using TrainFair.FareCalculator;
using TrainFair.Models;

namespace TrainFair.FareRuleBuilder
{
    public class FareRuleBuilder : IFareRuleBuilder
    {
        public void DisplayTrainFare()
        {
            //Fare pricing rules 
            //According to the rail transit network fare system approved by the municipal price department, that is:
            //rail transit implements multi-level fare based on mileage, 3 yuan for 0~6 kilometers, 1 yuan for every 10 kilometers after 6 kilometers;
            //fare calculation The shortest path method is adopted, that is, when there is more than one transfer path between two stations, the shortest path is selected as the basis for calculating the fare between the two stations.
            Console.WriteLine("Metro fare calculation");
            Console.WriteLine("-----------------------");
            Console.WriteLine("Enter the starting station name:");
            var fromStation = Console.ReadLine();
            Console.WriteLine("Enter destination station name:");
            var toStation = Console.ReadLine();

            StationFareRuleModel stations = new StationFareRuleModel();
            stations.FareRuleId = 1;
            stations.StationDistance = this.CountStationsDistance(fromStation, toStation);
            stations.IncrementalPrice = FareConstants.OnStationFare;

            FareCalculatorContext fareCalculatorContext = new FareCalculatorContext(new StationRuleFareCalculator());
            var totalFare = fareCalculatorContext.GetFareDetails(stations, FareConstants.BasicFare);
            Console.WriteLine("-----------------------");
            Console.WriteLine("from {1} to {2}, distance {3}KM, car fare is: {0} yuan", totalFare, fromStation, toStation, stations.StationDistance);
            Console.WriteLine("-----------------------");

            Console.WriteLine("Is it a VIP (y/n):");
            var isSeniorCitize = Console.ReadKey();
            if (isSeniorCitize.Key == ConsoleKey.Y)
            {
                VIPFareRuleModel ageFareRuleModel = new VIPFareRuleModel();
                ageFareRuleModel.FareRuleId = 2;
                ageFareRuleModel.Discount = FareConstants.VIPDiscount;

                fareCalculatorContext = new FareCalculatorContext(new VIPRuleFareCalculator());
                totalFare = fareCalculatorContext.GetFareDetails(ageFareRuleModel, totalFare);
                Console.WriteLine("\n-----------------------");
                Console.WriteLine("Enjoy Member Discount");
                Console.WriteLine("From {1} to {2}, enjoy a {3} discount, the car fare is: {0} yuan", totalFare, fromStation, toStation, FareConstants.VIPDiscount);
            }

            Console.WriteLine("\n is there any other fee (y/n):");
            var isFestival = Console.ReadKey();
            if (isFestival.Key == ConsoleKey.Y)
            {
                var otherFareRuleModel = new OtherFareRuleModel();
                Console.WriteLine("\nEnter the cost name:");
                otherFareRuleModel.FareRuleId = 3;
                otherFareRuleModel.OtherFareName = Console.ReadLine();
                Console.WriteLine("\n input fee (yuan):");
                otherFareRuleModel.AdditionalFare = float.Parse(Console.ReadLine());
                fareCalculatorContext = new FareCalculatorContext(new OtherRuleFareCalculator());
                totalFare = fareCalculatorContext.GetFareDetails(otherFareRuleModel, totalFare);
                Console.WriteLine("-----------------------");
                Console.WriteLine("Total fare");
                Console.WriteLine("from {1} to {2}, car fare is: {0} yuan", totalFare, fromStation, toStation);
            }
        }

        public float CountStationsDistance(string from, string to)
        {
            return this.DistanceOfStations(from, to);
        }

        public float DistanceOfStations(string from, string to)
        {
            string[] stations = new string[]
            {
                "1s", "2s", "3s", "4s", "5s", "6s", "7s", "8s", "9s", "10s", "11s", "12s", "13s", "14s",
                "15s", "16s", "17s", "18s", "19s", "20s", "21s", "22s"
            };
            float[] distances = new float[] { 1.8F, 1.2F, 1.2F, 1.6F, 2.2F, 2.6F, 1.2F, 1.5F, 1.6F, 2.0F, 2.8F, 0.8F, 1.2F, 3.5F, 1.6F, 1.8F, 1.2F, 2.8F, 2.4F, 1.8F, 2.3F, 2.5F };
            var fromIndex = Array.IndexOf(stations, from);
            var toIndex = Array.IndexOf(stations, to);
            float distance = 0.0f;
            if (fromIndex <= toIndex)
            {
                for (int i = fromIndex; i <= toIndex; i++)
                {
                    distance += distances[i];
                }
            }

            return distance;
        }
    }
}