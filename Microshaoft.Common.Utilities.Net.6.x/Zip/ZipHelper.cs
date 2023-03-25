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
                                        , Func
                                            <
                                                T
                                                , Task
                                                    <
                                                        (
                                                            string EntryFullName
                                                            , Stream EntryStream
                                                        )
                                                    >
                                            >
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
                                (var entryFullName, var entryStream) = 
                                                await onUpdateEntryProcessFuncAsync(x);
                                return (false, true, entryFullName, entryStream, true);
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
                                        , Func
                                            <
                                                T
                                                , Task
                                                    <
                                                        (
                                                              bool NeedBreak
                                                            , bool NeedUpdateEntry
                                                            , string entryFullName
                                                            , Stream EntryStream
                                                            , bool NeedDisposeEntryStream
                                                        )
                                                    >
                                            >
                                                    onUpdateEntryProcessFuncAsync
                                        , Encoding?
                                                    entryNameEncoding = null
                                        , CompressionLevel
                                                    entryCompressionLevelOnCreate =
                                                                        CompressionLevel.Optimal
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
                                        , Func
                                            <
                                                T
                                                , Task
                                                    <
                                                        (
                                                            string EntryFullName
                                                            , Stream EntryStream
                                                        )
                                                    >
                                            >
                                                    onUpdateEntryProcessFuncAsync
                                        , Encoding?
                                                    entryNameEncoding = null
                                        , CompressionLevel
                                                    entryCompressionLevelOnCreate =
                                                                        CompressionLevel.Optimal
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
                            (var entryFullName, var entryStream) =
                                                await onUpdateEntryProcessFuncAsync(x);
                            return (false, true, entryFullName, entryStream, true);
                        }
                        , entryNameEncoding
                        , entryCompressionLevelOnCreate
                        , extractToDirectoryName
                    );
    }


    public static async Task<(ZipArchive, Stream)>
                                    ZipCompressAsync2<T>
                                                (
                                                    this IAsyncEnumerable<T>
                                                                @this
                                                    , Func
                                                        <
                                                            T
                                                            , Task
                                                                <
                                                                    (
                                                                          bool NeedBreak
                                                                        , bool NeedUpdateEntry
                                                                        , string EntryFullName
                                                                        , Stream EntryStream
                                                                        , bool NeedDisposeEntryStream
                                                                    )
                                                                >
                                                        >
                                                                onUpdateEntryProcessFuncAsync
                                                    , Encoding?
                                                                entryNameEncoding = null
                                                    , CompressionLevel
                                                                entryCompressionLevelOnCreate =
                                                                                CompressionLevel.Optimal
                                                    , string?
                                                                extractToDirectoryName = null

                                                )
    {
        bool compressed = false;
        ZipArchive zipArchive = null!;
        Stream zipStream = null!;
        await foreach (var item in @this)
        {
            (
                  bool needBreak
                , compressed
                , zipArchive
                , zipStream
            )
                = await eachProcessAsync
                                (
                                    item
                                    , zipArchive
                                    , zipStream
                                    , onUpdateEntryProcessFuncAsync
                                    , entryNameEncoding
                                    , entryCompressionLevelOnCreate
                                );
            if (needBreak)
            {
                break;
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

    public static async Task<(ZipArchive, Stream)>
                                    ZipCompressAsync<T>
                                        (
                                            this IEnumerable<T>
                                                        @this
                                            , Func
                                                <
                                                    T
                                                    , Task
                                                        <
                                                            (
                                                                bool NeedBreak
                                                                , bool NeedUpdateEntry
                                                                , string entryFullName
                                                                , Stream EntryStream
                                                                , bool NeedDisposeEntryStream
                                                            )
                                                        >
                                                >
                                                        onUpdateEntryProcessFuncAsync
                                            , Encoding?
                                                        entryNameEncoding = null
                                            , CompressionLevel
                                                        entryCompressionLevelOnCreate =
                                                                        CompressionLevel.Optimal
                                            , string?
                                                        extractToDirectoryName = null
                                        )
    {
        bool compressed = false;
        ZipArchive zipArchive = null!;
        Stream zipStream = null!;
        
        foreach (var item in @this)
        {
            (
                bool needBreak
                , compressed
                , zipArchive
                , zipStream
            ) 
                = await eachProcessAsync
                                (
                                    item
                                    , zipArchive
                                    , zipStream
                                    , onUpdateEntryProcessFuncAsync
                                    , entryNameEncoding
                                    , entryCompressionLevelOnCreate
                                );
            if (needBreak)
            {
                break;
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

    private static async Task
                            <
                                (
                                      bool NeedBreak
                                    , bool Compressed
                                    , ZipArchive ZipArchive
                                    , Stream ZipStream
                                )
                            >
                        eachProcessAsync<T>
                            (
                                T item
                                , ZipArchive zipArchive
                                , Stream zipStream
                                , Func
                                    <
                                        T
                                        , Task
                                                <
                                                    (
                                                          bool NeedBreak
                                                        , bool NeedUpdateEntry
                                                        , string EntryFullName
                                                        , Stream EntryStream
                                                        , bool NeedDisposeEntryStream
                                                    )
                                                >
                                    >
                                                onUpdateEntryProcessFuncAsync
                                    , Encoding?
                                                entryNameEncoding = null
                                    , CompressionLevel
                                                entryCompressionLevelOnCreate =
                                                                    CompressionLevel.Optimal
                            )
    {

        (
              bool needBreak
            , bool needUpdateEntry
            , string entryFullName
            , Stream entryStream
            , bool needDisposeEntryStream
        )
        = await onUpdateEntryProcessFuncAsync(item);
        try
        {
            bool compressed = false;
            if
                (
                    needUpdateEntry
                    &&
                    entryStream is not null
                    &&
                    !string.IsNullOrEmpty(entryFullName)
                    &&
                    !string.IsNullOrWhiteSpace(entryFullName)
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

                ZipArchiveEntry entry = zipArchive.GetEntry(entryFullName)!;

                entry ??= zipArchive.CreateEntry(entryFullName, entryCompressionLevelOnCreate);

                using var entryUpdateStream = entry.Open();
                await entryStream.CopyToAsync(entryUpdateStream);
                entryUpdateStream.Close();
                entry = null!;
                compressed = true;
            }
            return
                (needBreak, compressed, zipArchive, zipStream);
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
}