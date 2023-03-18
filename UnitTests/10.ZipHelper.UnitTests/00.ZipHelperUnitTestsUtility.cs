using Microshaoft;

namespace UnitTests.Utilities;

public static class ZipHelperUnitTestsUtility
{

    public static async Task ZipCompressProcessAsync
                                (
                                    string sourceDirectory
                                    , string targetRootDirectory
                                    , string zipFileName
                                    , Action
                                            <
                                                (
                                                    string TargetRootDirectory
                                                    , Stream ZipStream
                                                    , string ExtractToDirectoryPath
                                                    , string ZipFilePath
                                                    , List<(string EntryName, Stream EntryStream)> EntriesStreams
                                                )
                                            >
                                                onAssertProcess
                                )
    {

        var extractToDirectoryPath = Path.Combine(targetRootDirectory , "zip", $"{zipFileName}.zip.extracted");

        var zipFilePath = Path.Combine(targetRootDirectory, "zip", $"{zipFileName}.zip");

        List<(string EntryName, Stream EntryStream)> entriesStreams = new();

        Stream? zipStream = null;
        try
        {
            zipStream = await Directory
                                    .GetFiles
                                        (
                                            sourceDirectory
                                            , "*.json"
                                            , new EnumerationOptions()
                                            {
                                                RecurseSubdirectories = true
                                                , MaxRecursionDepth = 3
                                            }
                                        )
                                    .CompressAsync
                                        (
                                            async (filePath) =>
                                            {
                                                var entryName = filePath;
                                                Stream entryStream = File.OpenRead(filePath);
                                                var result = (entryName, entryStream);
                                                entriesStreams.Add(result);
                                                await Task.CompletedTask;
                                                return
                                                    result;
                                            }
                                            , extractToDirectoryName: extractToDirectoryPath
                                        );
            zipStream!.Position = 0;
            
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            var tempFileDirectoryPath = Path.GetDirectoryName(zipFilePath);
            if (!Directory.Exists(tempFileDirectoryPath))
            {
                Directory.CreateDirectory(tempFileDirectoryPath!);
            }
            using var zipFileStream = File.OpenWrite(zipFilePath);
            await zipStream.CopyToAsync(zipFileStream);
            zipStream.Position = 0;
        }
        finally
        {
            zipStream?.Close();
            zipStream?.Dispose();
        }

        onAssertProcess
                (
                    (
                          targetRootDirectory
                        , zipStream
                        , extractToDirectoryPath
                        , zipFilePath
                        , entriesStreams

                    )
                );
    }

    public static void Test()
    { 
    
    
    }


}
