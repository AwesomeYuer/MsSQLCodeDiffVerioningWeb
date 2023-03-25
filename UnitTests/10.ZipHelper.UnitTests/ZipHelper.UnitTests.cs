namespace UnitTests;

using Castle.Components.DictionaryAdapter.Xml;
using Microshaoft;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using UnitTests.Utilities;

[TestClass]
public class ZipHelperUnitTests
{
    private class c1
    {
        public int F1 = 0;
    }
    [Fact]
    [TestCase]
    [TestMethod]
    public async Task TestMethod1Async()
    {


        var tempFileName = $"TempFile.{nameof(ZipHelperUnitTests)}";
        await
            ZipHelperUnitTestsUtility
                                .ZipCompressProcessAsync
                                    (
                                        $@"00.ZipHelper.UnitTests\SourceFiles"
                                        , "Temp"
                                        , tempFileName
                                        , async (x) =>
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
                                                x.ZipStream.Position = x.ZipStream.Position;
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



                                            var zipArchive = new ZipArchive(x.MemoryZipStream);
                                            var entries = zipArchive
                                                                .Entries
                                                                .ForEachAsIAsyncEnumerableSyncAsync
                                                                    (
                                                                        (i, entry) =>
                                                                        { 
                                                                            var needYield = true;
                                                                            var needBreak = false;
                                                                            if (i > 5)
                                                                            {
                                                                                needYield = true;
                                                                                needBreak = true;
                                                                            }
                                                                            return
                                                                                (needYield, needBreak, entry);
                                                                        }
                                                                    );

                                            await foreach (var entry in entries)
                                            {
                                                //entry.Result.F1 = 10;
                                                var filePath = Path.Combine
                                                                        (
                                                                            x.ExtractToDirectoryPath
                                                                            , entry.Source.FullName
                                                                        );

                                                r = File.Exists(filePath);

                                                Console.WriteLine(entry.Source.FullName);
                                                MAssert.IsTrue(r);
                                                NAssert.IsTrue(r);
                                                xAssert.True(r);
                                                using var entryStream = entry.Source.Open();
                                                using var textReader = new StreamReader(entryStream);
                                                var json = textReader.ReadToEnd();
                                                try
                                                {
                                                    _ = JToken.Parse(json);
                                                    r = true;
                                                }
                                                catch
                                                {
                                                    r = false;
                                                }

                                                MAssert.IsTrue(r);
                                                NAssert.IsTrue(r);
                                                xAssert.True(r);
                                            }

                                            var entries2 = entries.AsIEnumerable();

                                            r = entries2
                                                        .All
                                                            (
                                                                (entry) =>
                                                                {
                                                                    var filePath = Path.Combine
                                                                            (
                                                                                x.ExtractToDirectoryPath
                                                                                , entry.Source.FullName
                                                                            );
                                                                    var r = File.Exists(filePath);
                                                                    Console.WriteLine(entry.Source.FullName);
                                                                    using var entryStream = entry.Source.Open();
                                                                    using var textReader = new StreamReader(entryStream);
                                                                    var json = textReader.ReadToEnd();
                                                                    try
                                                                    {
                                                                        _ = JToken.Parse(json);
                                                                        r = true;
                                                                    }
                                                                    catch
                                                                    {
                                                                        r = false;
                                                                    }
                                                                    return r;
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