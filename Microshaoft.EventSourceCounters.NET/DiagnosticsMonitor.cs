using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Microshaoft;

class DiagnosticsMonitor
{
    static void Main(string[] args)
    {
        // Find the process containing the target EventSource.
        var targetProcess = DiagnosticsClient
                                        .GetPublishedProcesses()
                                        .Select(Process.GetProcessById)
                                        .FirstOrDefault
                                            (
                                                (x) =>
                                                {
                                                    return
                                                        x.ProcessName.Contains(args[0], StringComparison.OrdinalIgnoreCase);
                                                }
                                            );

        if (targetProcess == null)
        {
            Console.WriteLine($"No process named '*{args[0]}*' found. Exiting.");
            return;
        }

        // Define what EventSource and events to listen to.
        var providers = new List<EventPipeProvider>()
        {
            new EventPipeProvider
                    (
                          args[1]
                        , EventLevel.Verbose
                        , arguments: new Dictionary<string, string>
                                        {
                                            { "EventCounterIntervalSec", "1" }
                                        }
                    )
        };

        var i = 1;
        var countersCursorsTops = new Dictionary<string, (int Id, int CursorTop)>(StringComparer.InvariantCultureIgnoreCase)
        {
              { "TotalDurationInMs-Duration"                                                        , (i ++, 1  )       }

            , { "QueryGalleryLiveOnlyItemsAsync-Duration"                                           , (i ++, 3  )       }
            , { "QueryGalleryPreviewOnlyItemsAsync-Duration"                                        , (i ++, 4  )       }

            , { "CompressUploadProcessAsync-Duration"                                               , (i ++, 6  )       }

            , { "AzureApplicationsRawOfferTranslationHandler.UpdateSasLinksAsync-Duration"          , (i ++, 7  )       }
            , { "CoreVMRawOfferTranslationHandler.UpdateSasLinksAsync-Duration"                     , (i ++, 8  )       }
            , { "CustomServiceRawOfferTranslationHandler.UpdateSasLinksAsync-Duration"              , (i ++, 9  )       }

            , { "UpdateSasLinkAsync.Inner.StorageClient.GetBlobMD5Twice-Duration"                   , (i ++, 11 )       }
            , { "UpdateSasLinkAsync.Inner.HttpOrStorageClient.Download-Duration"                    , (i ++, 12 )       }
            , { "UpdateSasLinkAsync.Inner.StorageClient.UploadBlob-Duration"                        , (i ++, 13 )       }

            , { "CompressUploadProcessAsync.Inner.ZipHelper.CompressAsync-Duration"                 , (i ++, 15 )       }
            , { "CompressAsync.Inner.HttpClient.GetAsync-Duration"                                  , (i ++, 16 )       }
            , { "CompressUploadProcessAsync.Inner.StorageClient.UploadBlob-Duration"                , (i ++, 17 )       }
        };

        // Start listening session
        var client = new DiagnosticsClient(targetProcess.Id);
        using var session = client.StartEventPipeSession(providers, false);
        using var source = new EventPipeEventSource(session.EventStream);

        var logsDirectory = @".\logs";
        if (!Directory.Exists(logsDirectory))
        { 
            Directory.CreateDirectory(logsDirectory);
        }

        var logsFilePath = Path.Combine(logsDirectory, $"counters.{DateTime.Now:yyyy-MM-dd.HH-mm-ss}.log.txt");

        using var fileStream = new FileStream(logsFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var streamWriter = new StreamWriter(fileStream)
        { 
             AutoFlush = true
        };

        // Set up output writer
        source.Dynamic.All += traceEvent =>
        {
            if (traceEvent.EventName == "EventCounters")
            {
                var payloadValue = (IDictionary<string, object>)traceEvent.PayloadValue(0);
                IDictionary<string, object> payloadKeyValuePairs = (IDictionary<string, object>)payloadValue["Payload"];

                if (payloadKeyValuePairs.TryGetValue("Count", out var o))
                {
                    var count = (int) o;
                    if (count <= 0)
                    {
                        return;
                    }
                }

                var counterName = (string)payloadKeyValuePairs["Name"];
                var counterDisplayName = (string)payloadKeyValuePairs["DisplayName"];
                var counterType = (string)payloadKeyValuePairs["CounterType"];
                var displayUnits = (string)payloadKeyValuePairs["DisplayUnits"];
                counterDisplayName = $"{counterName}";

                (int Id, int CursorTop) e;

                var id = -1;
                var cursorTop = -1;

                if (!countersCursorsTops.TryGetValue(counterDisplayName, out e))
                {
                    cursorTop = countersCursorsTops
                                            .Max
                                                (
                                                    (x) =>
                                                    {
                                                        return
                                                            x.Value.CursorTop;
                                                    }
                                                );

                    id = countersCursorsTops
                                            .Max
                                                (
                                                    (x) =>
                                                    {
                                                        return
                                                            x.Value.Id;
                                                    }
                                                );

                    countersCursorsTops.Add(counterDisplayName, (++id, ++cursorTop));
                }
                else
                { 
                    id = e.Id;
                    cursorTop = e.CursorTop;
                }

                double @value;
                var counterValue = string.Empty;
                if
                    (
                        counterType == "Mean"
                        &&
                        displayUnits == "ms/op"
                    )
                {
                    @value = (double) payloadKeyValuePairs["Mean"];
                }
                else if
                    (
                        counterType == "Sum"
                        &&
                        displayUnits == "ops/sec"
                    )
                {
                    @value = (double) payloadKeyValuePairs["Increment"];
                }
                else
                {
                    @value = (double) payloadKeyValuePairs["Max"];
                }

                {
                    var rightPad = Console.WindowWidth - 120;
                    var counterLabel = $"({id:00}).{counterDisplayName}:";
                    var counterValueText = $"{@value:#,##0.000,00}\t{displayUnits} @ {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffff}";

                    streamWriter.WriteLine($"{counterLabel}\t{counterValueText}");
                    streamWriter.Flush();

                    Console.SetCursorPosition(0, cursorTop);
                    Console.Write("\r".PadLeft(Console.WindowWidth - Console.CursorLeft - 1));
                    Console.Write(counterLabel);
                    Console.CursorLeft = rightPad - counterValueText.Length;
                    Console.Write(counterValueText);
                    Console.CursorLeft = rightPad - "yyyy-MM-dd HH:mm:ss.fffff".Length - 15;
                }
            }
            else
            {
                Console.WriteLine($"{traceEvent.ProviderName}: {traceEvent.EventName}");
            }
        };

        try
        {
            source.Process();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error encountered while processing events");
            Console.WriteLine(e.ToString());
        }
        Console.ReadKey();
    }
}
