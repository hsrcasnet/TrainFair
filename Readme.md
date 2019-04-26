# Application mode strategy mode in the subway fare system application #
## 引言##
Design patterns are a very exciting part of object-oriented programming. Using design patterns is to re-use code, make code easier to understand, and ensure code reliability. It helps us organize applications into easy-to-understand, easy-to-maintain, and resilient architectures. This article uses a simple case to describe the application of the strategy model in the subway fare system.

## 案例描述##

Passengers take the subway from one station to another and he/she needs to buy a ticket. The railway department has some special fare rules for fares:

> According to the rail transit network fare system approved by the municipal price department, that is: rail transit implements multi-level fare based on mileage, 3 yuan for 0~6 kilometers, and 1 yuan for every 10 kilometers after 6 kilometers; The shortest path method, that is, when there are more than one transfer path between two sites, the shortest route is selected as the basis for calculating the fare between the two stations. 

## case study##

Let us consider 20 sites: 1s, 2s, 3s...20s, and passengers take the subway in different scenarios. In order to explain the problem more clearly, we have virtualized some application scenarios on the original pricing standards.


- If passenger A rides less than 6 kilometers, he will be charged a $3 ticket.


- If passenger B rides a mileage greater than 6 km, he will be required to pay an extra fee for the excess ticket, which is increased by 1 yuan per 10 km after 6 km.


- If passenger C is a VIP customer, he will receive a 10% discount on the original billing standard.

- If there are some additional charges or additional offers, please make adjustments based on the above charges.

## solution##

This problem can be solved by using the "strategic design pattern". Because different types of fare strategies can be applied based on different rules. Here are the different types of fare strategies:
- Basic fare rule strategy
- VIP fare rule strategy
- Additional fare rule strategy

Each fare rule strategy will be written separately to the fare calculation algorithm, and these algorithms will not interfere with each other. New fare rules can add and write new fare rule policies. This model will also follow the concept of “open to extension, closed to modification”.

**Dependency diagram**
![](https://i.imgur.com/4yrVtyG.png)
**Class Diagram**
![](https://i.imgur.com/iKagQ4Z.png)

**Code Description**

**IFareStrategy interface**

This interface defines a common strategy for fare calculations, enabling a class to implement a context-based fare algorithm.
```
Using TrainFair.Models;    
Namespace TrainFair.FareCalculator  
{  
    Public interface IFareStrategy {  
        Float GetFare(IFareRule ruleValues, float basicFare);  
    }  
}  
```
**FareConstants class**
FareConstants defines the rules for billing, including the starting price, the mileage increase price and the VIP discount price.
```
Namespace TrainFair.Constants
{
    Public class FareConstants {
        Public const float BasicFare = 3.0F;
        Public const float OnStationFare = 1.0F;
        Public const float VIPDiscount = 0.1F;
    }
}
```
**StationRuleFareCalculator class**

The StationRuleFareCalculator class calculates the fare based on the set of rules defined in the station mileage and problem statement section.
```
Using System;  
Using TrainFair.Models;  
  
Namespace TrainFair.FareCalculator  
{  
    Public class StationRuleFareCalculator : IFareStrategy  
    {  
        Public float GetFare(IFareRule ruleValues, float basicFare) {  

            Var stationFareRuleModel = ruleValues ​​as StationFareRuleModel;  
             If (stationFareRuleModel == null || stationFareRuleModel.StationDistance <= 0.0f)
                Return 0;
            
            If (stationFareRuleModel.StationDistance < 6)
                Return basicFare;

            Int restChargingStations = (int)Math.Ceiling((stationFareRuleModel.StationDistance - 6.0f)/10.0f);
            Var totalFare = basicFare + restChargingStations * stationFareRuleModel.IncrementalPrice;           

            Return totalFare;
        }  
    }  
}  
```
**VIPRuleFareCalculator class**
This class implements the VIP fare algorithm. If the passenger is a VIP, he/she will receive a special offer. This class implements this algorithm.
```
Using TrainFair.Models;  
  
Namespace TrainFair.FareCalculator  
{  
     Public class VIPRuleFareCalculator : IFareStrategy
    {
        Public float GetFare(IFareRule ruleValues, float basicFare) {
            Var vipFareRuleModel = ruleValues ​​as VIPFareRuleModel;
            If (vipFareRuleModel == null)
                Return 0;
           
            Var totalFare = basicFare - (basicFare * vipFareRuleModel.Discount);
            Return totalFare;
        }
    }
}   
```

**OtherRuleFareCalculator class**
This class implements an algorithm for other additional fees or discounted fares. Some extra price will be added to the total cost. The extra price can be an additional charge (positive value) or an additional discount (negative value).
```
Using TrainFair.Models;  
  
Namespace TrainFair.FareCalculator  
{  
    Public class OtherRuleFareCalculator : IFareStrategy
    {
        Public float GetFare(IFareRule ruleValues, float basicFare) {
            Var otherFareRuleModel = ruleValues ​​as OtherFareRuleModel;
            If (otherFareRuleModel == null)
                Return basicFare;

            Float totalFare = basicFare + otherFareRuleModel.AdditionalFare;
            Return totalFare;
        }
    }
}  
```
**FareRuleCalculatorContext class**
```
Using TrainFair.Models;  
  
Namespace TrainFair.FareCalculator  
{  
    Public class FareCalculatorContext {  
  
        Private readonly IFareStrategy _fareStrategy;  
        Public FareCalculatorContext(IFareStrategy fareStrategy) {  
            this._fareStrategy = fareStrategy;  
        }  
  
        Public float GetFareDetails(IFareRule fareRules, float basicFare)  
        {  
            Return _fareStrategy.GetFare(fareRules, basicFare);  
        }  
    }  
}  
```

There are some model classes based on station fares, VIP fares, extra fares, etc. in the code structure.

**IFareRule interface**

This is the basic fare rule model interface, which is implemented by each model class.
```
Namespace TrainFair.Models  
{  
    Public interface IFareRule  
    {  
        Int FareRuleId { get; set; }  
    }  
}  
```
**StationFareRuleModel class**
This class defines the basic properties of the station fare rules.
```
Namespace TrainFair.Models  
{  
    Public class StationFareRuleModel : IFareRule  
    {  
        Public int FareRuleId { get; set; }

        Public int StationsCounts { get; set; }

        Public float IncrementalPrice { get; set; }

        Public float StationDistance { get; set; }  
    }  
} 
``` 
**VIPFareRuleModel class**

This class defines the properties of the VIP discount.
```
Namespace TrainFair.Models  
{  
    Public class VIPFareRuleModel : IFareRule  
    {  
        Public int FareRuleId { get; set; }       

        Public float Discount { get; set; }
    }  
}  
```
**OtherFareRuleModel class**

This class defines properties for other extra charges.
```
Namespace TrainFair.Models  
{  
    Public class OtherFareRuleModel : IFareRule  
    {  
        Public int FareRuleId { get; set; }  
  
        Public string OtherFareName { get; set; }  
  
        Public float AdditionalFare { get; set; }  
    }  
}  
```
The attributes of the model can be enhanced and adjusted according to future needs, and can be flexibly applied in the algorithm class.

**Results of the**

The following is the console output:

![](https://i.imgur.com/lIXYFnR.jpg)
 

The program code is attached at the end of this article. 


##结语##

Different types of fare calculation rules are different for station base fares, VIP fares, extra fares, etc. All algorithms are decomposed into different classes so that different algorithms can be selected at runtime. The purpose of a policy pattern is to encapsulate each algorithm or logic into separate classes with a common interface for a set of algorithms or logic so that they can be replaced with each other. The policy pattern allows the algorithm or logic to change without affecting the client. Speaking of the strategy mode, I have to mention the OCP (Open Closed Principle) principle of opening and closing, that is, opening to the extension and closing the modification. The emergence of the strategy model is a good interpretation of the principle of opening and closing, effectively reducing the branching statement.

**Program Code**:<a href="https://github.com/daivven/TrainFair">https://github.com/daivven/TrainFair</a>



<div style="background: #f0f8ff; padding: 10px; border: 2px dashed #990d0d; font-family: Microsoft Yahei;">
& Nbsp; Author: <a href="http://www.cnblogs.com/yayazi/"> A child </a>
<br>
Blog address: <a href="http://www.cnblogs.com/yayazi/">http://www.cnblogs.com/yayazi/</a>
<br>
This article addresses: <a href="http://www.cnblogs.com/yayazi/p/8350679.html">http://www.cnblogs.com/yayazi/p/8350679.html</a>
<br>
Declaration: The original text of this blog is allowed to be reproduced. This paragraph must be retained when reprinting, and the original text connection is given in the obvious position of the article page.

</div>