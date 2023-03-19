namespace UnitTests.xUnitTest;

using Microshaoft;
using UnitTests.Utilities;
using Xunit;


public class ZipHelperXUnitTests
{
    [Fact]
    public async Task TestMethod1Async()
    {
        await
            ZipHelperUnitTestsUtility
                                .ZipCompressProcessAsync
                                    (
                                        $@"00.ZipHelper.UnitTests\SourceFiles"
                                        , "Temp"
                                        , $"TempFile.{nameof(ZipHelperXUnitTests)}"
                                        , (x) =>
                                        {
                                            Assert
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
                                            Assert.True(r);

                                            x.ZipStream = null!;
                                            Assert.Null(x.ZipStream);

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
                                            Assert.True(r);


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
                                            Assert.True(r);
                                            //Directory.Delete(@"zip", true);

                                        }
                                    );
    }
}