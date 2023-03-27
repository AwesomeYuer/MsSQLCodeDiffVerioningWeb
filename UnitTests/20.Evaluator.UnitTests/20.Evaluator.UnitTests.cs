namespace UnitTests;

using Microshaoft;
using Newtonsoft.Json.Linq;
using System;
using System.IO.Compression;
using System.Reflection;
using UnitTests.Utilities;

[TestClass]
public class EvaluatorUnitTests
{
    [Fact]
    [TestCase]
    [TestMethod]
    public void TestMethod1()
    {
        //Evaluator
        //    .EvaluateToVoid
        //            (
        //                "MessageBox.Show(\"Test\");"
        //                , new Dictionary<string, string>()
        //                    {
        //                        {
        //                            "System.Windows.Forms.dll"
        //                            , "System.Windows.Forms"
        //                        } 
        //                    }
        //            );


        string str = @"""
using System;
namespace a
{ 
    public class b
    {
        public  void c()
        {
            Console.WriteLine(1);
        }
    }
}
""";
        var r = 7 == Evaluator.EvaluateToInteger("1 + 2 * 3");
        MAssert.IsTrue(r);
        NAssert.IsTrue(r);
        xAssert.True(r);

        var s = Console
            .Out
            .CaptureOutput
                (
                    () =>
                    {
                        Evaluator
                            .EvaluateByAllCode(str, "a", "b", "c");
                    }
                );
        r = s.Message == "1";

        MAssert.IsTrue(r);
        NAssert.IsTrue(r);
        xAssert.True(r);
    }
}