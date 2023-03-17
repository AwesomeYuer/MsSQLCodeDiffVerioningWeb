namespace Microshaoft;

using System.IO.Compression;
using System.Text;

public static class ZipHelper
{

    public static async Task<Stream>
                                CompressAsync<T>
                                            (
                                                this IEnumerable<T>
                                                            @this
                                                , Func<T, Task<(string, Stream)>>
                                                            onUpdateEntryProcessFuncAsync
                                                , Encoding?
                                                            entryNameEncoding = null
                                                , CompressionLevel
                                                            entryCompressionLevelOnCreate = CompressionLevel.Optimal
                                                , string?
                                                            extractToDirectoryName = null
                                            )
    {
        return
            await
                CompressAsync
                        (
                            @this
                            , async (x) =>
                            {
                                (var entryName, var entryStream) = await onUpdateEntryProcessFuncAsync(x);
                                return (false, true, entryName, entryStream, true);
                            }
                            , entryNameEncoding
                            , entryCompressionLevelOnCreate
                            , extractToDirectoryName
                        );


    }

    public static async Task<Stream>
                                    CompressAsync<T>
                                                (
                                                    this IEnumerable<T>
                                                                @this
                                                    , Func<T, Task<(bool, bool, string, Stream, bool)>>
                                                                onUpdateEntryProcessFuncAsync
                                                    , Encoding?
                                                                entryNameEncoding = null
                                                    , CompressionLevel
                                                                entryCompressionLevelOnCreate = CompressionLevel.Optimal
                                                    , string?
                                                                extractToDirectoryName = null
                                                )
    {
        ZipArchive zipArchive = null!;
        try
        {
            (
                zipArchive
                , Stream zipStream
            )
            = await ZipCompressAsync
                            (
                                @this
                                , onUpdateEntryProcessFuncAsync
                                , entryNameEncoding
                                , entryCompressionLevelOnCreate
                                , extractToDirectoryName
                            );
            return zipStream;
        }
        finally
        {
            zipArchive?.Dispose();
        }
    }

    public static async Task<(ZipArchive, Stream)>
                                ZipCompressAsync<T>
                                            (
                                                this IEnumerable<T>
                                                            @this
                                                , Func<T, Task<(string, Stream)>>
                                                            onUpdateEntryProcessFuncAsync
                                                , Encoding?
                                                            entryNameEncoding = null
                                                , CompressionLevel
                                                            entryCompressionLevelOnCreate = CompressionLevel.Optimal
                                                , string?
                                                            extractToDirectoryName = null
                                            )
    {
        return
            await
                ZipCompressAsync
                    (
                        @this
                        , async (x) =>
                        {
                            (var entryName, var entryStream) = await onUpdateEntryProcessFuncAsync(x);
                            return (false, true, entryName, entryStream, true);
                        }
                        , entryNameEncoding
                        , entryCompressionLevelOnCreate
                        , extractToDirectoryName
                    );
    }


    public static async Task<(ZipArchive, Stream)>
                                    ZipCompressAsync<T>
                                                (
                                                    this IEnumerable<T>
                                                                @this
                                                    , Func<T, Task<(bool, bool, string, Stream, bool)>>
                                                                onUpdateEntryProcessFuncAsync
                                                    , Encoding?
                                                                entryNameEncoding = null
                                                    , CompressionLevel
                                                                entryCompressionLevelOnCreate = CompressionLevel.Optimal
                                                    , string?
                                                                extractToDirectoryName = null

                                                )
    {
        bool compressed = false;
        ZipArchive zipArchive = null!;
        MemoryStream zipStream = null!;
        foreach (var item in @this)
        {

            (
                bool needBreak
                , bool needUpdateEntry
                , string entryName
                , Stream entryStream
                , bool needDisposeEntryStream
            )
            = await onUpdateEntryProcessFuncAsync(item);
            try
            {
                if
                    (
                        needUpdateEntry
                        &&
                        entryStream is not null
                        &&
                        !string.IsNullOrEmpty(entryName)
                        &&
                        !string.IsNullOrWhiteSpace(entryName)
                    )
                {
                    zipStream ??= new MemoryStream();
                    zipArchive ??= new ZipArchive
                                                (
                                                    zipStream
                                                    , ZipArchiveMode.Update
                                                    , true
                                                    , entryNameEncoding
                                                );

                    ZipArchiveEntry entry = zipArchive.GetEntry(entryName)!;

                    entry ??= zipArchive.CreateEntry(entryName, entryCompressionLevelOnCreate);

                    using var entryUpdateStream = entry.Open();
                    await entryStream.CopyToAsync(entryUpdateStream);
                    entryUpdateStream.Close();
                    entry = null!;

                    if (!compressed)
                    {
                        compressed = true;
                    }
                }
                if (needBreak)
                {
                    break;
                }
            }
            finally
            {
                if (needDisposeEntryStream)
                {
                    if (entryStream is not null)
                    {
                        entryStream.Close();
                        entryStream.Dispose();
                        entryStream = null!;
                    }
                }
            }
        }
        if
            (
                compressed
                //&&
                //zipStream is not null
            )
        {
            if
                (
                    !string.IsNullOrEmpty(extractToDirectoryName)
                    &&
                    !string.IsNullOrWhiteSpace(extractToDirectoryName)
                )
            {
                if (!Directory.Exists(extractToDirectoryName))
                {
                    Directory.CreateDirectory(extractToDirectoryName);
                }
                zipArchive.ExtractToDirectory(extractToDirectoryName, true);
            }
            zipStream.Position = 0;
        }
        return
            (zipArchive, zipStream);
    }
}