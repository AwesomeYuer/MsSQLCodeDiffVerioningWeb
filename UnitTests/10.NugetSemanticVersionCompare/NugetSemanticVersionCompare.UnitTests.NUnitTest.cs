namespace UnitTests.NUnitTest;
using NuGet.Versioning;
using NUnit.Framework;


public partial class NugetSemanticVersionNUnitTests
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

    //[DataRow("0001.0.2", "!<", "1.0.003")]


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
        
        Assert.IsTrue(r, $"failed! {nameof(left)} is not a {nameof(NuGetVersion)}");

        SemanticVersion? leftSemanticVersion = leftNugetVersion;

        Assert.IsNotNull(leftSemanticVersion, $"failed! {nameof(NuGetVersion)}: {nameof(leftNugetVersion)}: [{leftNugetVersion}] can't convert to {nameof(SemanticVersion)}");

        r = NuGetVersion.TryParse(right, out NuGetVersion rightNugetVersion);

        Assert.IsTrue(r, $"failed! {nameof(right)} is not a {nameof(NuGetVersion)}");

        SemanticVersion? rightSemanticVersion = rightNugetVersion;

        Assert.IsNotNull(rightSemanticVersion, $"failed! {nameof(NuGetVersion)}: {nameof(rightNugetVersion)}: [{rightNugetVersion}] can't convert to {nameof(SemanticVersion)}");

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

        Assert.AreEqual(r, expect , $"failed! compare result is NOT: {left} {@operator} {right}");

        var x = leftSemanticVersion.ToString();
        r = x == left;
        Assert.IsTrue(r, $"failed! {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToString)} [{x}] != [{left}] {nameof(left)}");

        x = leftNugetVersion.ToString();
        r = x == left;
        Assert.IsTrue(r, $"failed! {nameof(leftNugetVersion)}.{nameof(leftNugetVersion.ToString)} [{x}] != [{left}] {nameof(left)}");

        x = rightSemanticVersion.ToString();
        r = x == right;
        Assert.IsTrue(r, $"failed! {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToString)} [{x}] != [{right}] {nameof(right)}");

        x = rightNugetVersion.ToString();
        r = x == right;
        Assert.IsTrue(r, $"failed! {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToString)} [{x}] != [{right}] {nameof(right)}");

        x = leftSemanticVersion.ToFullString();
        var y = leftSemanticVersion.ToNormalizedString();
        r = x == y;
        Assert.IsTrue(r, $"failed! {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToFullString)} [{x}] != [{y}] {nameof(leftSemanticVersion)}.{nameof(leftSemanticVersion.ToNormalizedString)}");

        x = leftNugetVersion.ToFullString();
        y = leftNugetVersion.ToNormalizedString();
        r = x == y;
        Assert.IsTrue(r, $"failed! {nameof(leftNugetVersion)}.{nameof(leftNugetVersion.ToFullString)} [{x}] != [{y}] {nameof(leftNugetVersion)}.{nameof(leftSemanticVersion.ToNormalizedString)}");

        x = rightSemanticVersion.ToFullString();
        y = rightSemanticVersion.ToNormalizedString();
        r = x == y;
        Assert.IsTrue(r, $"failed! {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToFullString)} [{x}] != [{y}] {nameof(rightSemanticVersion)}.{nameof(rightSemanticVersion.ToNormalizedString)}");

        x = rightNugetVersion.ToFullString();
        y = rightNugetVersion.ToNormalizedString();
        r = x == y;
        Assert.IsTrue(r, $"failed! {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToFullString)} [{x}] != [{y}] {nameof(rightNugetVersion)}.{nameof(rightNugetVersion.ToNormalizedString)}");
    }
}