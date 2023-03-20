namespace UnitTests;

using Microshaoft;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using NUnit.Framework;
using NAssert = NUnit.Framework.Assert;

using Xunit;
using xAssert = Xunit.Assert;
//using xTheoryAttribute = Xunit.TheoryAttribute;


using UnitTests.Utilities;

[TestClass]
public class ZipHelperUnitTests
{
    [Fact]
    [TestCase]
    [TestMethod]
    public async Task TestMethod1Async()
    {
        await
            ZipHelperUnitTestsUtility
                                .ZipCompressProcessAsync
                                    (
                                        $@"00.ZipHelper.UnitTests\SourceFiles"
                                        , "Temp"
                                        , $"TempFile.{nameof(ZipHelperUnitTests)}"
                                        , (x) =>
                                        {
                                            MAssert
                                               .IsTrue
                                                    (
                                                        !x.ZipStream.CanRead
                                                        &&
                                                        !x.ZipStream.CanWrite
                                                        &&
                                                        !x.ZipStream.CanSeek
                                                        &&
                                                        !x.ZipStream.CanTimeout
                                                    );
                                            NAssert
                                               .IsTrue
                                                    (
                                                        !x.ZipStream.CanRead
                                                        &&
                                                        !x.ZipStream.CanWrite
                                                        &&
                                                        !x.ZipStream.CanSeek
                                                        &&
                                                        !x.ZipStream.CanTimeout
                                                    );
                                            xAssert
                                               .True
                                                    (
                                                        !x.ZipStream.CanRead
                                                        &&
                                                        !x.ZipStream.CanWrite
                                                        &&
                                                        !x.ZipStream.CanSeek
                                                        &&
                                                        !x.ZipStream.CanTimeout
                                                    );
                                            bool r = false;
                                            try
                                            {
                                                x.ZipStream.Position = 0;
                                            }
                                            catch (ObjectDisposedException)
                                            {
                                                r = true;
                                            }

                                            MAssert.IsTrue(r);
                                            NAssert.IsTrue(r);
                                            xAssert.True(r);

                                            x.ZipStream = null!;

                                            MAssert.IsNull(x.ZipStream);
                                            NAssert.IsNull(x.ZipStream);
                                            xAssert.Null(x.ZipStream);

                                            r = x.EntriesStreams
                                                                .All
                                                                    (
                                                                        (xx) =>
                                                                        {
                                                                            return 
                                                                                    xx
                                                                                        .EntryStream
                                                                                        .CheckDisposed
                                                                                                <Stream>
                                                                                                    (
                                                                                                        (xxx) =>
                                                                                                        {
                                                                                                            xxx.Position = xxx.Position;
                                                                                                        }
                                                                                                    );
                                                                        }
                                                                    );

                                            MAssert.IsTrue(r);
                                            NAssert.IsTrue(r);
                                            xAssert.True(r);


                                            r = Directory
                                                        .GetFiles
                                                            (
                                                                x.ExtractToDirectoryPath
                                                                , "*.*"
                                                                , SearchOption.AllDirectories
                                                            )
                                                        .All
                                                            (
                                                                (filePath) =>
                                                                {
                                                                    filePath = filePath[x.ExtractToDirectoryPath.Length..]
                                                                                        .TrimStart
                                                                                                (
                                                                                                    Path.DirectorySeparatorChar
                                                                                                    , Path.AltDirectorySeparatorChar
                                                                                                );
                                                                    var rr = File.Exists(filePath);
                                                                    return rr;
                                                                }
                                                            );
                                            MAssert.IsTrue(r);
                                            NAssert.IsTrue(r);
                                            xAssert.True(r);
                                            //Directory.Delete(@"zip", true);

                                        }
                                    );
    }
}