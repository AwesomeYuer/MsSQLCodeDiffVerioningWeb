namespace UnitTests;

using NuGet.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using NUnit.Framework;
using NAssert = NUnit.Framework.Assert;

using Xunit;
using xAssert = Xunit.Assert;
using xTheoryAttribute = Xunit.TheoryAttribute;

[TestClass, TestCategory(nameof(NugetSemanticVersionUnitTests))]
public partial class NugetSemanticVersionUnitTests
{
    [TestCase("1.0.1",       ">=",       "1.0.0"             , true)]
    [TestCase("1.0.1",       ">=",       "001.000.001"       , true)]

    [TestCase("1.0.01",      ">=",       "0.0.0001"          , true)]
    [TestCase("1.0.001",     ">=",       "00.0000.01"        , true)]

    [TestCase("1.0.1",       "==",       "1.000.01"          , true)]
    [TestCase("01.0.1",      "=",        "1.000.00001"       , true)]

    [TestCase("0.9.1",       ">=",       "0.9.2"             , false)]
    [TestCase("0.9.1",       "<=",       "000.0999.0999"     , true)]
    [TestCase("0.9.1",       "<=",       "000.09.0001"       , true)]

    [TestCase("000.0.1",     "<",        "1.0.0"             , true)]
    [TestCase("0.0.9999",    "<",        "0.001.0"           , true)]
    [TestCase("0001.0.2",    "<",        "1.0.003"           , true)]


    [DataRow("1.0.1",       ">=",       "1.0.0"             , true)]
    [DataRow("1.0.1",       ">=",       "001.000.001"       , true)]
                                                              
    [DataRow("1.0.01",      ">=",       "0.0.0001"          , true)]
    [DataRow("1.0.001",     ">=",       "00.0000.01"        , true)]
                                                              
    [DataRow("1.0.1",       "==",       "1.000.01"          , true)]
    [DataRow("01.0.1",      "=",        "1.000.00001"       , true)]
                                                              
    [DataRow("0.9.1",       ">=",       "0.9.2"             , false)]
    [DataRow("0.9.1",       "<=",       "000.0999.0999"     , true)]
    [DataRow("0.9.1",       "<=",       "000.09.0001"       , true)]
                                                              
    [DataRow("000.0.1",     "<",        "1.0.0"             , true)]
    [DataRow("0.0.9999",    "<",        "0.001.0"           , true)]
    [DataRow("0001.0.2",    "<",        "1.0.003"           , true)]
    [TestMethod]


    [InlineData("1.0.1", ">=", "1.0.0", true)]
    [InlineData("1.0.1", ">=", "001.000.001", true)]

    [InlineData("1.0.01", ">=", "0.0.0001", true)]
    [InlineData("1.0.001", ">=", "00.0000.01", true)]

    [InlineData("1.0.1", "==", "1.000.01", true)]
    [InlineData("01.0.1", "=", "1.000.00001", true)]

    [InlineData("0.9.1", ">=", "0.9.2", false)]
    [InlineData("0.9.1", "<=", "000.0999.0999", true)]
    [InlineData("0.9.1", "<=", "000.09.0001", true)]

    [InlineData("000.0.1", "<", "1.0.0", true)]
    [InlineData("0.0.9999", "<", "0.001.0", true)]
    [InlineData("0001.0.2", "<", "1.0.003", true)]
    [xTheory]
    public void Test_For_NugetSemanticVersionCompare
                                        (
                                            string left
                                            , string @operator
                                            , string right
                                            , bool expect
                                        )
    {
        bool r;

        @operator = @operator.Trim();

        r = NuGetVersion.TryParse(left, out NuGetVersion leftNugetVersion);

        var failedMessage = $"failed! {nameof(left)} is not a {nameof(NuGetVersion)}";

        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        SemanticVersion? leftSemanticVersion = leftNugetVersion;

        failedMessage = $"failed! {nameof(NuGetVersion)}: {nameof(leftNugetVersion)}: [{leftNugetVersion}] can't convert to {nameof(SemanticVersion)}";
        MAssert.IsNotNull(leftSemanticVersion, failedMessage);
        NAssert.IsNotNull(leftSemanticVersion, failedMessage);
        xAssert.NotNull(leftSemanticVersion);

        r = NuGetVersion.TryParse(right, out NuGetVersion rightNugetVersion);

        failedMessage = $"failed! {nameof(right)} is not a {nameof(NuGetVersion)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        SemanticVersion? rightSemanticVersion = rightNugetVersion;

        failedMessage = $"failed! {nameof(NuGetVersion)}: {nameof(rightNugetVersion)}: [{rightNugetVersion}] can't convert to {nameof(SemanticVersion)}";
        MAssert.IsNotNull(rightSemanticVersion, failedMessage);
        NAssert.IsNotNull(rightSemanticVersion, failedMessage);
        xAssert.NotNull(rightSemanticVersion);

        r = @operator switch
        {
            ">"     => leftSemanticVersion >  rightSemanticVersion,
            ">="    => leftSemanticVersion >= rightSemanticVersion,
            "!="    => leftSemanticVersion != rightSemanticVersion,
            "<>"    => leftSemanticVersion != rightSemanticVersion,
            "><"    => leftSemanticVersion != rightSemanticVersion,
            "=="    => leftSemanticVersion == rightSemanticVersion,
            "="     => leftSemanticVersion == rightSemanticVersion,
            "<="    => leftSemanticVersion <= rightSemanticVersion,
            "<"     => leftSemanticVersion <  rightSemanticVersion,
            _       => false
        };

        failedMessage = $"failed! compare result is NOT: {left} {@operator} {right}";
        MAssert.AreEqual(r, expect , failedMessage);
        NAssert.That(expect, Is.EqualTo(r), failedMessage);
        xAssert.Equal<bool>(r, expect);

        var x = leftSemanticVersion.ToString();
        r = x == left;

        failedMessage = $"failed! {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToString)} [{x}] != [{left}] {nameof(left)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = leftNugetVersion.ToString();
        r = x == left;
        failedMessage = $"failed! {nameof(leftNugetVersion)}.{nameof(leftNugetVersion.ToString)} [{x}] != [{left}] {nameof(left)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = rightSemanticVersion.ToString();
        r = x == right;

        failedMessage = $"failed! {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToString)} [{x}] != [{right}] {nameof(right)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = rightNugetVersion.ToString();
        r = x == right;

        failedMessage = $"failed! {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToString)} [{x}] != [{right}] {nameof(right)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = leftSemanticVersion.ToFullString();
        var y = leftSemanticVersion.ToNormalizedString();
        r = x == y;

        failedMessage = $"failed! {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToFullString)} [{x}] != [{y}] {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToNormalizedString)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = leftNugetVersion.ToFullString();
        y = leftNugetVersion.ToNormalizedString();
        r = x == y;

        failedMessage = $"failed! {nameof(leftNugetVersion)}.{nameof(leftNugetVersion.ToFullString)} [{x}] != [{y}] {nameof(leftNugetVersion)}.{nameof(leftSemanticVersion.ToNormalizedString)}";
        MAssert.IsTrue(r, failedMessage);
        MAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = rightSemanticVersion.ToFullString();
        y = rightSemanticVersion.ToNormalizedString();
        r = x == y;

        failedMessage = $"failed! {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToFullString)} [{x}] != [{y}] {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToNormalizedString)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);

        x = rightNugetVersion.ToFullString();
        y = rightNugetVersion.ToNormalizedString();
        r = x == y;

        failedMessage = $"failed! {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToFullString)} [{x}] != [{y}] {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToNormalizedString)}";
        MAssert.IsTrue(r, failedMessage);
        NAssert.IsTrue(r, failedMessage);
        xAssert.True(r, failedMessage);
    }
}