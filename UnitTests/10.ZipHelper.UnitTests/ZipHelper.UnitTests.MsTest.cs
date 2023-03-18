namespace UnitTests.MsTest;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Utilities;

[TestClass]
public class ZipHelperMsTests
{
    [TestMethod]
    public async Task TestMethod1Async()
    {
        await
            ZipHelperUnitTestsUtility
                                .ZipCompressProcessAsync
                                    (
                                        $@"00.ZipHelper.UnitTests\SourceFiles"
                                        , "Temp"
                                        , $"TempFile.{nameof(ZipHelperMsTests)}"
                                        , (x) =>
                                        {
                                            Assert
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
                                            bool r = false;
                                            try
                                            {
                                                x.ZipStream.Position = 0;
                                            }
                                            catch (ObjectDisposedException)
                                            {
                                                r = true;
                                            }
                                            Assert.IsTrue(r);

                                            x.ZipStream = null!;
                                            Assert.IsNull(x.ZipStream);

                                            r = x.EntriesStreams
                                                            .All
                                                                (
                                                                    (xx) =>
                                                                    {
                                                                        try
                                                                        {
                                                                            xx.EntryStream.Position = 0;
                                                                        }
                                                                        catch (ObjectDisposedException)
                                                                        {
                                                                            return true;
                                                                        }
                                                                        return false;
                                                                    }
                                                                );
                                            Assert.IsTrue(r);

                                            
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
                                            Assert.IsTrue(r);
                                            //Directory.Delete(@"zip", true);

                                        }
                                    );
    }
}