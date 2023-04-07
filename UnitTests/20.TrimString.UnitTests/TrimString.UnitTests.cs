namespace UnitTests;

using Microshaoft;
using Newtonsoft.Json.Linq;
using System;
using System.IO.Compression;
using System.Reflection;
using UnitTests.Utilities;

[TestClass]
public class TrimStringUnitTests
{
    [xTheory]
    [InlineData("aaaaaaBBBBB", "BBb", "aaaaaaBB")]
    [InlineData("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]

    [TestMethod]
    [DataRow("aaaaaaBBBBB", "BBb", "aaaaaaBB")]
    [DataRow("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]

    [TestCase("aaaaaaBBBBB", "BBb", "aaaaaaBB")]
    [TestCase("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]

    public void Test_TrimEndString_Method1Async
                                (
                                    string orininal
                                    , string trim
                                    , string expect           
                                )
    {
        var r = orininal.TrimEndString(trim, StringComparison.OrdinalIgnoreCase) == expect;
        xAssert.True(r);
        MAssert.IsTrue(r);
        NAssert.IsTrue(r);
    }


    [xTheory]
    [InlineData("aaaaaaBBBBB", "AaA", "aaaBBBBB")]
    [InlineData("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [InlineData("", "AaA", "")]

    [TestMethod]
    [DataRow("aaaaaaBBBBB", "AaA", "aaaBBBBB")]
    [DataRow("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [DataRow("", "AaA", "")]

    [TestCase("aaaaaaBBBBB", "AaA", "aaaBBBBB")]
    [TestCase("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [TestCase("", "AaA", "")]


    public void Test_TrimStartString_Method1Async
                                (
                                    string orininal
                                    , string trim
                                    , string expect
                                )
    {
        var r = orininal.TrimStartString(trim, StringComparison.OrdinalIgnoreCase) == expect;
        xAssert.True(r);
        MAssert.IsTrue(r);
        NAssert.IsTrue(r);
    }


    [xTheory]
    [InlineData("aaaaaaBBBBBaaaa", "AaA", "aaaBBBBBa")]
    [InlineData("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [InlineData("", "AaA", "")]
    [InlineData(null, "AaA", null)]
    [InlineData(null, null, null)]


    [TestMethod]
    [DataRow("aaaaaaBBBBBaaaa", "AaA", "aaaBBBBBa")]
    [DataRow("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [DataRow("", "AaA", "")]
    [DataRow(null, "AaA", null)]
    [DataRow(null, null, null)]

    [TestCase("aaaaaaBBBBBaaaa", "AaA", "aaaBBBBBa")]
    [TestCase("aaaaaaBBBBB", "ccc", "aaaaaaBBBBB")]
    [TestCase("", "AaA", "")]
    [TestCase(null, "AaA", null)]
    [TestCase(null, null, null)]

    public void Test_TrimString_Method1Async
                                (
                                    string orininal
                                    , string trim
                                    , string expect
                                )
    {
        var r = orininal.TrimString(trim, StringComparison.OrdinalIgnoreCase) == expect;
        xAssert.True(r);
        MAssert.IsTrue(r);
        NAssert.IsTrue(r);
    }

}