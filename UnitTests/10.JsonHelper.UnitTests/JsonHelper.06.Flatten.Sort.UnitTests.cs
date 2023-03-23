namespace UnitTests;

using Microshaoft;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using System;

[TestClass]
public class JsonHelperUnitTests
{
    [Fact(DisplayName = $"{nameof(FactAttribute)}")]
    [TestCase(Description = $"{nameof(TestCaseAttribute)}")]
    [TestMethod($"{nameof(TestMethod)}")]
    public void Test()
    {
        
        JObject jObject = new JObject
        {
            { "F1", "aaa" },
            { "F2", "aaa" },
            { "F3", 1 },
            {
                "F4", new JObject
                        {
                            { "F5", 1 }
                            ,
                                { 
                                    "F6" , new JArray
                                            {
                                                "item 1"
                                                , "item 2"
                                                ,
                                                    new JObject
                                                    {
                                                        { "F5", "aaaa"}
                                                    }
                                                ,
                                                    new JObject
                                                    {
                                                        { "F5", "aaaa"}
                                                    }

                                            }
                                }
                        } 
            }
        };
        var json = jObject
                        .Flatten
                            (
                                (x) =>
                                {
                                    var i = x.IndexInArray;
                                    var propertyName = x.PropertyName;
                                    if
                                        (
                                            x.IndexInArray >= 0
                                            &&
                                            string.IsNullOrEmpty(x.Seprator)
                                        )
                                    {
                                        i *= -1;
                                        propertyName = $"[{i}]";
                                    }
                                    
                                    return
                                        string.IsNullOrEmpty(x.Prefix) ? $"({propertyName})" : $"{x.Prefix}{x.Seprator}({propertyName})";

                                    // return x.PrefixPropertyName;
                                }
                            )
                        .ToString()
                        ;
        Console.WriteLine( json );

        json = jObject.Flatten().ToString();
        Console.WriteLine(json);
    }

}
