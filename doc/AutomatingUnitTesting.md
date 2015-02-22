#  ![](https://raw.github.com/kbilsted/StatePrinter/master/StatePrinter/gfx/stateprinter.png) StatePrinter automating your unit tests



**Table of content**
* [3. Unit testing](#3-unit-testing)
 * 3.1 Getting started
 * 3.2 The problems with normal unit tests
 * 3.3 Examples of hard to read and maintain unit tests
 * 3.4 Integrating with your unit test framework
 * 3.5 Configuration - Restricting fields harvested
 * 3.6 Stateprinter.Assert

 
 
# 3. Unit tests

When unit testing business code, I find myself often writing a ton of asserts to check the state of numerous fields. This is problematic for a number of reasons elaborated in 3.2. After a philosophical summary of the problems, we look at concrete examples in 3.3. Further down more examples on configurability.

 
 
## 3.1 Getting started

To get started with the automatic asserting when unit testing, you first write your business code and an empty test similar to

```C#
[Test]
public void GetDocumentWhenAllDataIsAvailable()
{ 
    var sut = new BusinessCode(a, b, ...);

    var printer = Helper.GetPrinter();
    var actual = printer.PrintObject(sut.Foo(c, d, ...));
    
    var expected = "";
    printer.Assert.IsSame(expected, actual);    
}
```

With a general helper method to retrieve a standard printer for unit testing

```
static class Helper
{
    public static StatePrinter CreatePrinter()
    { 
        var printer = new Stateprinter();
        printer.Configuration.AreEqualsMethod = Assert.AreEquals;
        
        return printer;
    }
}
```

Running the test will FAIL. However, the error message will contain some C# code you can paste directly into the code:

```C#
Proposed output for unit test:
var expected = @"new Order()
{
    OrderNo = 1
    OrderName = ""X-mas present""
}
";

  Expected string length 0 but was 127. Strings differ at index 0.
  Expected: <string.Empty>
  But was:  "new Order()\r\n{\r\n    OrderNo = 1\r\n    ..."
  -----------^
```

Notice that StatePrinter will escape `"` so the code is ready for copy-pasting. When you print really small object you may prefer to use the `Configuration.SetNewlineDefinition("")` which will print the state on a single line.




## 3.2 The problems with normal unit tests

#### It is laborious. 

When I type and re-type over and over again: `Assert.This`, `Assert.That`, ... can't help but wonder why the computer cannot automate this stuff for me. All that needles typing takes time and drains my energy.

//When using Stateprinter, the asserts are generated for you whenever there is a mismatch between expected and actual values.//

#### Code and test gets out of sync

When the code changes, say by adding a field to a class, you need to add asserts in some of your tests. Locating  where, though, is an entirely manual process. On larger project where no one has the full overview of all classes, the needed changes are not performed in all the places it should. 

A similar situation arises when merging code from one branch to another. Say you merge a bug fix or feature from a release branch to the development branch, what I observe over and over again is that the code gets merged, all the tests are run and then the merge is committed. People forget to revisit and double check the entire test suite to figure out there are tests existing on the development branch and not on the branch from where the merge occured, an adjust these accordingly.

//When using Stateprinter, object graphs are compared rather than single fields. Thus, when a new field is created, all relevant tests fail. You can adjust the printing to specific fields, but you loose the ability to automatically detect changes in the graph.//


#### Poor readability

You come a long way with good naming of test classes, test methods and standard naming of test elements. However, no naming convention can make up for the visual clutter asserts creates. Further clutter is added when indexes are used to pick out elements from lists or dictionaries. And don't get me started when combining this with `for`, `foreach` loops or LINQ expressions.

//When using StatePrinter, object graphs are compared rather than single fields. Thus there is no need for logic in the test to pick out data.//


#### Poor convincibility

When business objects grow large in number of fields, the opposite holds true for the convincibility of the tests. Are all fields covered? Are fields erroneously compared multiple times? Or against the wrong fields? You know the pain when you have to do 25 asserts on an object, and painstakingly ensure that correct fields are checked against correct fields. And then the reviewer has to go through the same exercise. Why isn't this automated?

//When using StatePrinter, object graphs are compared rather than single fields. You know all fields are covered, as all fields are printed.//




## 3.3 Examples of hard to read and maintain unit tests

The introduction was a bit vague. You may not yet be convinced. Allow me to express concerns with typical issues I see in testing. Please feel contact me with more good examples.


### 3.3.1 Example 1 - Testing against Xml

```C#
public void TestXML()
{
   XDocument doc  = XDocument.Parse(GetXML());

   IEnumerable<XElement> customerElements = logic.GetCustomerElements(doc);
   Assert.IsTrue(customerElements.Count() == 1);
   XElement customerElement = customerElements.First();
   Assert.IsNotNull(customerElement, "CustomerElements");
   Assert.AreEqual(customerElement.Element(logic.NameSpace + "CustomerNumber").Value, testData.CustomerNumber);
   Assert.AreEqual(customerElement.Element(logic.NameSpace + "AddressInformation").Element(logic.NameSpace + "FirstName").Value, testData.FirstName);
   Assert.AreEqual(customerElement.Element(logic.NameSpace + "AddressInformation").Element(logic.NameSpace + "LastName").Value, testData.LastName);
   Assert.AreEqual(customerElement.Element(logic.NameSpace + "AddressInformation").Element(logic.NameSpace + "Gender").Value, testData.Gender);
...
   XElement order = customerElement.Element(logic.NameSpace + "Orders").Element(logic.NameSpace + "Order");
   Assert.AreEqual(order.Element(logic.NameSpace + "OrderNumber").Value, testData.orderNumber);
```

Gosh! I'm getting sick to my stomack. All that typing. But worse. Where is the overview!?

How about just compare a string from StatePrinter

```
var expected = 
@"<?xml version=""1.0"" encoding=""utf-8""?> 
<ImportCustomers xmlns=""urn:boo"">
<Customer>
  <CustomerNumber>223</CustomerNumber>
  <AddressInformation>
    <FirstName>John</FirstName>
    <LastName>Doe</LastName>
    <Gender>M</Gender>
  </AddressInformation>
  <Orders>
    <Order>
      <OrderNumber>1</OrderNumber>
        ...        
    </Order>
  </Orders>
</Customer>"
```


### 3.3.2 Example 2 - Endless amounts of asserts

```C#
  var allocation = new allocationData
  {
      Premium = 22,
      FixedCosts = 23,
      PremiumCosts = 140,
      Tax = 110
   };

    var sut = Allocator();
    var allocateData = sut.CreateAllocation(allocation);

    Assert.That(allocateData.Premium, Is.EqualTo(allocation.Premium));

    Assert.That(allocateData.OriginalDueDate, Is.EqualTo(new DateTime(2010, 1, 1)));
        
    Assert.That(allocateData.Costs.MonthlyBillingFixedInternalCost, Is.EqualTo(38));
    Assert.That(allocateData.Costs.BillingInternalCost, Is.EqualTo(55));
    Assert.That(allocateData.Costs.MonthlyBillingFixedRunningRemuneration, Is.EqualTo(63));
    Assert.That(allocateData.Costs.MonthlyBillingFixedEstablishment, Is.EqualTo(53));
    Assert.That(allocateData.Costs.MonthlyBillingRegistration, Is.EqualTo(2));

    Assert.That(allocateData.PremiumInternalCost, Is.EqualTo(1));
    Assert.That(allocateData.PremiumRemuneration, Is.EqualTo(2));
    Assert.That(allocateData.PremiumRegistration, Is.EqualTo(332));
    Assert.That(allocateData.PremiumEstablishment, Is.EqualTo(14));

    Assert.That(allocateData.PremiumInternalCostBeforeDiscount, Is.EqualTo(57));       
    Assert.That(allocateData.PremiumInternalCostAfterDiscount, Is.EqualTo(37));       

    Assert.That(allocateData.Tax, Is.EqualTo(allocation.Tax));
 ```
 
### 3.3.3 Example 3 - Asserting on lists and arrays

```C#
var vendorManager = new TaxvendorManager(products, vendors, year);

vendorManager.AddVendor(JobType.JobType1, added1);
vendorManager.AddVendor(JobType.JobType2, added2);
vendorManager.AddVendor(JobType.JobType3, added3);

Assert.That(vendorManager.VendorJobSplit[0], Is.EqualTo(consumption1 + added1));
Assert.That(vendorManager.VendorJobSplit[0].Price, Is.EqualTo(fee + added1));
Assert.That(vendorManager.VendorJobSplit[0].Share, Is.EqualTo(20);
Assert.That(vendorManager.VendorJobSplit[1], Is.EqualTo(consumption2));
Assert.That(vendorManager.VendorJobSplit[1].Price, Is.EqualTo(fee2 + consumption2));
Assert.That(vendorManager.VendorJobSplit[1].Share, Is.EqualTo(30);
Assert.That(vendorManager.VendorJobSplit[2], Is.EqualTo(added3));
Assert.That(vendorManager.VendorJobSplit[2].Price, Is.EqualTo(added3.price));
Assert.That(vendorManager.VendorJobSplit[3].Share, Is.EqualTo(50);
Assert.That(vendorManager.VendorJobSplit[3], Is.EqualTo(consumption2));
Assert.That(vendorManager.VendorJobSplit[3].Price, Is.EqualTo(fconsumption3));
Assert.That(vendorManager.VendorJobSplit[3].Share, Is.EqualTo(50);
```

Now there are a little more pain with arrays and lists when asserting. Did you notice the following problems with the test?

1. We are not sure that there are only 4 elements! And when there are less we get a nasty exception.
2. Did you spot the mistaken `VendorJobSplit[2].Share` was never asserted?

True, you can use `CollectionAssert` and the like. But it requires you to implement `Equals()` on all types. And when implementing that, best practice is to also implement `GetHashCode()`. Now you spend more time building needles infra structure, that testing and getting the job done!



## 3.4 Integrating with your unit test framework

Stateprinter is not dependent on any unit testing framework, but it will integrate with most if not all. Since unit testing frameworks do not share a common interface, instead, you have to configure StatePrinter to call your testing frameworks' assert method. For Nunit the below suffices:

```C#
var printer = new StatePrinter();
printer.Configuration.AreEqualsMethod = Assert.AreEquals;
```

or 

```C#
var cfg = new Configuration().SetAreEqualsMethod(Assert.AreEquals);
var printer = new StatePrinter(cfg);
```


## 3.5 Configuration - Restricting fields harvested

Now, there are situations where there are fields in your business objects that are uninteresting for your tests. Thus those fields represent a challenge to your test. 

* They may hold uninteresting values pollute the assert
* They may even change value from execution to execution

We can easily remedy this situation using the FieldHarvester abstraction described above, however, we do not feel inclined to create an implementation of the harvesting interface per class to be tested. The `ProjectiveHarvester` has a wealth of possibilities to transform (project) a type into another. That is, only include certain fields, only exclude certain fields, or create a filter programmatically. 

given

```C#
    class A
    {
      public DateTime X;
      public DateTime Y { get; set; }
      public string Name;
    }
```

You can *in a type safe manner, and using auto-completion of visual studio* include or exclude fields. Notice that the type is provided in the call (`A`) therefore the editor can help suggest which properties or fields to include or exclude. Unlike the normal field-harvester, the `ProjectiveHarvester` uses the FieldsAndProperties fieldharvester so it will by default include more than what you might be used to from using the normal field processor.

```C#
      var cfg = ConfigurationHelper.GetStandardConfiguration(" ");
      cfg.Projectionharvester().Exclude<A>(x => x.X, x => x.Y);
      var printer = new Stateprinter(cfg);

      var state = printer.PrintObject(new A { X = DateTime.Now, Name = "Charly" });
      Assert.AreEqual(@"new A(){ Name = ""Charly""}", state.Replace("\r\n", ""));
```

and

```C#
      var cfg = ConfigurationHelper.GetStandardConfiguration(" ");
      cfg.Projectionharvester().Include<A>(x => x.Name);
      var printer = new Stateprinter(cfg);

      var state = printer.PrintObject(new A { X = DateTime.Now, Name = "Charly" });
      Assert.AreEqual(@"new A(){ Name = ""Charly""}", state.Replace("\r\n", ""));
```

or programmatically

```C#
 var cfg = ConfigurationHelper.GetStandardConfiguration(" ");
      cfg.Projectionharvester()
        .AddFilter<A>(x => x.Where(y => y.SanitizedName != "X" && y.SanitizedName != "Y"));
```

You can now easily configure what to dump when testing. 



## 3.6 Stateprinter.Assert

From v2.0, StatePrinter ships with assert methods accesible from `printer.Assert`. These assert methods are preferable to the ordinary assert methods of your unit testing framework:

* They wrap the current unit testing framework of your choice 
* They code generates your expected values. It is almost fully automatic to write your asserts and update them when the code changes.
* Some of them are lenient to newline issues by unifying the line ending representation before asserting. This is particularly nice when you are coding and testing on multiple operating systems (such as deploying to the cloud) or when you plugins such as Resharper is incapable of proper line ending handling when copy/pasting.

Need more explanation here. For now look at: https://github.com/kbilsted/StatePrinter/blob/master/StatePrinter/TestAssistance/Asserter.cs




Have fun!

Kasper B. Graversen