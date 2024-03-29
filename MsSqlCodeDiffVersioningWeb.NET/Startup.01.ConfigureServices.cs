﻿namespace WebApplication.ASPNetCore
{
    using Microshaoft;
    using Microshaoft.Web;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Net.Http.Headers;
    //using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Text;
    using System.Web;
#if NETCOREAPP2_X
    using Microsoft.AspNetCore.Hosting;
#else
#endif
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigurationHelper
                        .Load(Configuration);
            services
                    .AddSingleton<OperationsAuthorizeFilter>();

            #region Logging
            services
             .AddSingleton
                     (
                         GlobalManager.GlobalLoggerFactory
                     );
            services
                    .AddSingleton
                            (
                                GlobalManager.GlobalLogger
                            ); 
            #endregion

            services
                    .AddSingleton<ConfigurationSwitchAuthorizeFilter>();

            #region SwaggerGen
            services
                .AddSwaggerGenDefault(); 
            #endregion

            services
                .Configure<CsvFormatterOptions>
                    (
                        Configuration
                                .GetSection
                                    (
                                        "ExportCsvFormatter"
                                    )
                    );

            services
                .AddMvc
                (

                    (option) =>
                    {
                        option
                            .EnableEndpointRouting = false;
                    }

                )
                .AddNewtonsoftJson()

                ;

            #region ConfigurableActionConstrainedRouteApplicationModelProvider
            // for both NETCOREAPP2_X and NETCOREAPP3_X
            // for Sync or Async Action Selector
            services
                .TryAddEnumerable
                    (
                        ServiceDescriptor
                            .Singleton
                                <
                                    IApplicationModelProvider
                                    , ConfigurableActionConstrainedRouteApplicationModelProvider
                                                                            <ConstrainedRouteAttribute>
                                >
                            (
                                (x) =>
                                {
                                    return
                                        new
                                            ConfigurableActionConstrainedRouteApplicationModelProvider
                                                    <ConstrainedRouteAttribute>
                                                (
                                                    Configuration
                                                    , (attribute) =>
                                                    {
                                                        return
                                                            new ConfigurableActionConstraint
                                                                        <ConstrainedRouteAttribute>
                                                                    (
                                                                        attribute
                                                                    );
                                                    }
                                                );
                                }
                            )
                    );
            #endregion

            services
                    .AddSingleton
                        (
                            GlobalManager
                                .ExecutingCachingStore
                        );

            #region asyncRequestResponseLoggingProcessor
            services
                    .AddSingleton
                        (
                            GlobalManager
                                    .RequestResponseLoggingProcessor
                        );
            //Console.WriteLine($"Startup: {nameof(Thread.CurrentThread.ManagedThreadId)}:{Thread.CurrentThread.ManagedThreadId}");
            GlobalManager
                .RequestResponseLoggingProcessor
                .OnCaughtException += //AsyncRequestResponseLoggingProcessor_OnCaughtException;
                        (
                            sender
                            , exception
                            , newException
                            , innerExceptionMessage
                            , exceptionTime
                            , exceptionSource
                            , traceID
                        )
                            =>
                        {
                            return
                                GlobalManager
                                        .OnCaughtExceptionProcessFunc
                                            (
                                                GlobalManager
                                                        .GlobalLogger
                                               , exception
                                               , newException
                                               , innerExceptionMessage
                                               , exceptionTime
                                               , exceptionSource
                                               , traceID
                                            );
                        };



            var asyncProcessorConfigurationPrefixKeys = $"SingleThreadAsyncDequeueProcessors:AsyncRequestResponseLoggingProcessor";
            int sleepInMilliseconds = Configuration
                                            .GetValue
                                                (
                                                    $"{asyncProcessorConfigurationPrefixKeys}:{nameof(sleepInMilliseconds)}"
                                                    , 1000
                                                );
            int waitOneBatchTimeOutInMilliseconds =
                                    Configuration
                                            .GetValue
                                                (
                                                    $"{asyncProcessorConfigurationPrefixKeys}:{nameof(waitOneBatchTimeOutInMilliseconds)}"
                                                    , 1000
                                                );
            int waitOneBatchMaxDequeuedTimes =
                                    Configuration
                                            .GetValue
                                                (
                                                    $"{asyncProcessorConfigurationPrefixKeys}:{nameof(waitOneBatchMaxDequeuedTimes)}"
                                                    , 100
                                                );
            var requestResponseLoggingExecutor =
                    new MsSqlStoreProceduresExecutor(GlobalManager.ExecutingCachingStore)
                        {
                            CachedParametersDefinitionExpiredInSeconds
                                = Configuration
                                            .GetValue
                                                (
                                                    "CachedParametersDefinitionExpiredInSeconds"
                                                    , 3600
                                                )
                        };
            var connectionID = "c1";
            string connectionString = nameof(connectionString);
            connectionString = Configuration.GetValue<string>($"connections:{connectionID}:{connectionString}")!;
            // there are only one Thread that's DequeueThread write it, so it's security
            var requestResponseLoggingData = new JArray();
            GlobalManager
                .RequestResponseLoggingProcessor
                .StartRunDequeueThreadProcess
                    (
                        (dequeued, batch, indexInBatch, queueElement) =>
                        {
                            //Console.WriteLine($"Dequeue Once: {nameof(Thread.CurrentThread.ManagedThreadId)}:{Thread.CurrentThread.ManagedThreadId}");
                            var (url, Request, Response, Timing, User) = queueElement.Element;
                            var enqueueTimestamp = queueElement
                                                            .Timing
                                                            .EnqueueTimestamp;
                            double? queueTimingInMilliseconds = null;
                            if (enqueueTimestamp.HasValue)
                            {
                                queueTimingInMilliseconds =
                                                enqueueTimestamp
                                                                .Value
                                                                .GetElapsedTimeToNow()
                                                                .TotalMilliseconds;
                            }
                            requestResponseLoggingData
                                .Add
                                    (
                                        new JObject
                                        {
                                              { nameof(queueElement.ID)                             , queueElement.ID                                   }
                                            //======================================================================
                                            // Queue:
                                            , { nameof(queueElement.Timing.EnqueueTime)             , queueElement.Timing.EnqueueTime                   }
                                            , { nameof(queueElement.Timing.DequeueTime)             , queueElement.Timing.DequeueTime                   }
                                            , { nameof(queueTimingInMilliseconds)                   , queueTimingInMilliseconds                         }
                                            //=====================================================================
                                            // common
                                            , { nameof(url.requestTraceID)                          , url.requestTraceID                                }
                                            , { nameof(url.requestUrl)                              , url.requestUrl                                    }
                                            , { nameof(url.requestPath)                             , url.requestPath                                   }
                                            , { nameof(url.requestPathBase)                         , url.requestPathBase                               }
                                            , { nameof(url.requestActionRoutePath)                  , url.requestActionRoutePath                        }
                                            //=====================================================================
                                            // request:
                                            , { nameof(Request.requestHeaders)                      , Request.requestHeaders                            }
                                            , { nameof(Request.requestBody)                         , HttpUtility.UrlDecode(Request.requestBody)        }
                                            , { nameof(Request.requestMethod)                       , Request.requestMethod                             }
                                            , { nameof(Request.requestBeginTime)                    , Request.requestBeginTime                          }
                                            , { nameof(Request.requestContentLength)                , Request.requestContentLength                      }
                                            , { nameof(Request.requestContentType)                  , Request.requestContentType                        }

                                            //======================================================================
                                            // response:
                                            , { nameof(Response.responseHeaders)                    , Response.responseHeaders                          }
                                            , { nameof(Response.responseBody)                       , HttpUtility.UrlDecode(Response.responseBody)      }
                                            , { nameof(Response.responseStatusCode)                 , Response.responseStatusCode                       }
                                            , { nameof(Response.responseStartingTime)               , Response.responseStartingTime                     }
                                            , { nameof(Response.responseContentLength)              , Response.responseContentLength                    }
                                            , { nameof(Response.responseContentType)                , Response.responseContentType                      }
                                            
                                            //======================================================================
                                            // Timing :
                                            , { nameof(Timing.requestResponseTimingInMilliseconds)  , Timing.requestResponseTimingInMilliseconds        }
                                            , { nameof(Timing.dbExecutingTimingInMilliseconds)      , Timing.dbExecutingTimingInMilliseconds            }

                                            //======================================================================
                                            // Location:
                                            , { nameof(User.Location.clientIP)                      , User.Location.clientIP                            }
                                            , { nameof(User.Location.locationLongitude)             , User.Location.locationLongitude                   }
                                            , { nameof(User.Location.locationLatitude)              , User.Location.locationLatitude                    }
                                            
                                            //=======================================================================
                                            // user:
                                            , { nameof(User.userID)                                 , User.userID                                       }
                                            , { nameof(User.roleID)                                 , User.roleID                                       }
                                            , { nameof(User.orgUnitID)                              , User.orgUnitID                                    }
                                            , { nameof(User.Device.deviceID)                        , User.Device.deviceID                              }
                                            , { nameof(User.Device.deviceInfo)                      , User.Device.deviceInfo                            }
                                        }
                                    );
                        }
                        , (dequeued, batch, indexInBatch) =>
                        {
                            //Console.WriteLine($"Dequeue Batch: {nameof(Thread.CurrentThread.ManagedThreadId)}:{Thread.CurrentThread.ManagedThreadId}");
                            // sql Connection should be here avoid cross threads
                            var sqlConnection = new SqlConnection(connectionString);
                            string serverHost = nameof(serverHost);
                            try
                            {
                                requestResponseLoggingExecutor
                                    .ExecuteJsonResults
                                        (
                                            sqlConnection
                                            , "zsp_RequestResponseLogging"
                                            , new JObject
                                                {
                                                      { $"{serverHost}{nameof(GlobalManager.OsPlatformName)}"                               , GlobalManager.OsPlatformName                  }
                                                    , { $"{serverHost}{nameof(GlobalManager.OsVersion)}"                                    , GlobalManager.OsVersion                       }
                                                    , { $"{serverHost}{nameof(GlobalManager.FrameworkDescription)}"                         , GlobalManager.FrameworkDescription            }
                                                    , { $"{serverHost}{nameof(Environment.MachineName)}"                                    , Environment.MachineName                       }
                                                    , { $"{serverHost}ProcessId"                                                            , GlobalManager.CurrentProcess.Id               }
                                                    , { $"{serverHost}{nameof(GlobalManager.CurrentProcess.ProcessName)}"                   , GlobalManager.CurrentProcess.ProcessName      }
                                                    , { $"{serverHost}ProcessStartTime"                                                     , GlobalManager.CurrentProcess.StartTime        }
                                                    , { $"{serverHost}Process{nameof(GlobalManager.CurrentProcess.WorkingSet64)}"           , GlobalManager.CurrentProcess.WorkingSet64     }
                                                    , { $"{serverHost}Process{nameof(GlobalManager.CurrentProcess.PeakWorkingSet64)}"       , GlobalManager.CurrentProcess.PeakWorkingSet64 }
                                                    , { "data"                                                                              , requestResponseLoggingData                    }
                                                }
                                        );
                            }
                            finally
                            {
                                if (sqlConnection.State != ConnectionState.Closed)
                                {
                                    sqlConnection.Close();
                                }
                                //dataTable.Clear();
                                //should be clear correctly!!!!
                                requestResponseLoggingData.Clear();
                            }
                        }
                        , sleepInMilliseconds
                        , waitOneBatchTimeOutInMilliseconds
                        , waitOneBatchMaxDequeuedTimes
                    );
            #endregion

            #region ErrorExceptionLoggingProcessor
            GlobalManager
                        .ErrorExceptionLoggingProcessor
                        .OnCaughtException += //AsyncRequestResponseLoggingProcessor_OnCaughtException;
                                (
                                    sender
                                    , exception
                                    , newException
                                    , innerExceptionMessage
                                    , exceptionTime
                                    , exceptionSource
                                    , traceID
                                )
                                    =>
                                {
                                    return
                                        GlobalManager
                                                .OnCaughtExceptionProcessFunc
                                                    (
                                                        GlobalManager
                                                                .GlobalLogger
                                                       , exception
                                                       , newException
                                                       , innerExceptionMessage
                                                       , exceptionTime
                                                       , exceptionSource
                                                       , traceID
                                                    );
                                };

            var errorExceptionLoggingExecutor =
                    new MsSqlStoreProceduresExecutor(GlobalManager.ExecutingCachingStore)
                    {
                        CachedParametersDefinitionExpiredInSeconds
                                = Configuration
                                            .GetValue
                                                (
                                                    "CachedParametersDefinitionExpiredInSeconds"
                                                    , 3600
                                                )
                    };
            var errorExceptionLoggingData = new JArray();
            GlobalManager
                .ErrorExceptionLoggingProcessor
                .StartRunDequeueThreadProcess
                    (
                        (dequeued, batch, indexInBatch, queueElement) =>
                        {
                            //Console.WriteLine($"Dequeue Once: {nameof(Thread.CurrentThread.ManagedThreadId)}:{Thread.CurrentThread.ManagedThreadId}");
                            var
                                (
                                    errorExceptionTime
                                    , errorExceptionSource
                                    , errorExceptionTraceID
                                    , errorException
                                )
                                = queueElement
                                            .Element;
                            var enqueueTimestamp = queueElement
                                                            .Timing
                                                            .EnqueueTimestamp;
                            double? queueTimingInMilliseconds = null;
                            if (enqueueTimestamp.HasValue)
                            {
                                queueTimingInMilliseconds =
                                                enqueueTimestamp
                                                                .Value
                                                                .GetElapsedTimeToNow()
                                                                .TotalMilliseconds;
                            }
                            errorExceptionLoggingData
                                .Add
                                    (
                                        new JObject
                                        {
                                              { nameof(queueElement.ID)                             , queueElement.ID                                   }
                                            //======================================================================
                                            // Queue:
                                            , { nameof(queueElement.Timing.EnqueueTime)             , queueElement.Timing.EnqueueTime                   }
                                            , { nameof(queueElement.Timing.DequeueTime)             , queueElement.Timing.DequeueTime                   }
                                            , { nameof(queueTimingInMilliseconds)                   , queueTimingInMilliseconds                         }
                                            //=====================================================================
                                            , { nameof(errorExceptionTime)                          , errorExceptionTime                                }
                                            , { nameof(errorExceptionSource)                        , errorExceptionSource                              }
                                            , { nameof(errorExceptionTraceID)                       , errorExceptionTraceID                             }
                                            , { nameof(errorException)                              , errorException                                    }

                                        }
                                    );
                        }
                        , (dequeued, batch, indexInBatch) =>
                        {
                            //Console.WriteLine($"Dequeue Batch: {nameof(Thread.CurrentThread.ManagedThreadId)}:{Thread.CurrentThread.ManagedThreadId}");
                            // sql Connection should be here avoid cross threads
                            var sqlConnection = new SqlConnection(connectionString);
                            string serverHost = nameof(serverHost);
                            try
                            {
                                errorExceptionLoggingExecutor
                                    .ExecuteJsonResults
                                        (
                                            sqlConnection
                                            , "zsp_ErrorExceptionLogging"
                                            , new JObject
                                                {
                                                      { $"{serverHost}{nameof(GlobalManager.OsPlatformName)}"                               , GlobalManager.OsPlatformName                  }
                                                    , { $"{serverHost}{nameof(GlobalManager.OsVersion)}"                                    , GlobalManager.OsVersion                       }
                                                    , { $"{serverHost}{nameof(GlobalManager.FrameworkDescription)}"                         , GlobalManager.FrameworkDescription            }
                                                    , { $"{serverHost}{nameof(Environment.MachineName)}"                                    , Environment.MachineName                       }
                                                    , { $"{serverHost}ProcessId"                                                            , GlobalManager.CurrentProcess.Id               }
                                                    , { $"{serverHost}{nameof(GlobalManager.CurrentProcess.ProcessName)}"                   , GlobalManager.CurrentProcess.ProcessName      }
                                                    , { $"{serverHost}ProcessStartTime"                                                     , GlobalManager.CurrentProcess.StartTime        }
                                                    , { $"{serverHost}Process{nameof(GlobalManager.CurrentProcess.WorkingSet64)}"           , GlobalManager.CurrentProcess.WorkingSet64     }
                                                    , { $"{serverHost}Process{nameof(GlobalManager.CurrentProcess.PeakWorkingSet64)}"       , GlobalManager.CurrentProcess.PeakWorkingSet64 }
                                                    , { "data"                                                                              , errorExceptionLoggingData                     }
                                                }
                                        );
                            }
                            finally
                            {
                                if (sqlConnection.State != ConnectionState.Closed)
                                {
                                    sqlConnection.Close();
                                }
                                //dataTable.Clear();
                                //should be clear correctly!!!!
                                errorExceptionLoggingData.Clear();
                            }
                        }
                        , sleepInMilliseconds
                        , waitOneBatchTimeOutInMilliseconds
                        , waitOneBatchMaxDequeuedTimes
                    ); 
            #endregion

            services
                .AddSingleton
                    <
                        AbstractStoreProceduresService
                        , StoreProceduresExecuteService
                    >
                        ();

            services
                    .AddSingleton
                        (
                            new QueuedObjectsPool<Stopwatch>(16, true)
                        );
            services
                    .AddSingleton(Configuration);

            services
                    .AddSingleton("Inject String");

            #region 跨域策略
            services
                    .Add
                        (
                            ServiceDescriptor
                                .Transient<ICorsService, WildcardCorsService>()
                        );
            services
                .AddCors
                    (
                        (options) =>
                        {
                            options
                                .AddPolicy
                                    (
                                        "SPE"
                                        , (builder) =>
                                        {
                                            builder
                                                .WithOrigins
                                                    (
                                                        "*.microshaoft.com"
                                                    );
                                        }
                                    );
                            // BEGIN02
                            options
                                .AddPolicy
                                    (
                                        "AllowAllAny"
                                        , (builder) =>
                                        {
                                            builder
                                                .AllowAnyOrigin()
                                                .AllowAnyHeader()
                                                .AllowAnyMethod()
                                                .WithExposedHeaders("*");
                                        }
                                    );
                            //options.AddPolicy("AllowAll",
                            //    builder =>
                            //    {
                            //        builder.AllowAnyOrigin()
                            //               .AllowAnyMethod()
                            //               .AllowAnyHeader();
                            //    });
                        }
                  );
            #endregion

            services
                    .AddResponseCaching();

#if NETCOREAPP2_X
            //for NETCOREAPP2_X only
            services
                  .AddSingleton<IActionSelector, SyncOrAsyncActionSelector>();
#endif

            #region Output CsvFormatterOptions
            services
                    .AddMvc
                        (
                            (options) =>
                            {
                                var csvFormatterOptions = new CsvFormatterOptions
                                {
                                    CsvColumnsDelimiter = ",",
                                    IncludeExcelDelimiterHeader = false,
                                    UseSingleLineHeaderInCsv = true
                                };

                                var exportCsvFormatterConfiguration = Configuration
                                            .GetSection
                                                (
                                                    "ExportCsvFormatter"
                                                );

                                if
                                    (
                                        exportCsvFormatterConfiguration != null
                                    )
                                {
                                    csvFormatterOptions
                                            .CsvColumnsDelimiter = exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                        (
                                                                            nameof(csvFormatterOptions.CsvColumnsDelimiter)
                                                                            , ","
                                                                        )!;
                                    csvFormatterOptions
                                                .DateFormat =
                                                            exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.DateFormat)
                                                                                , "yyyy-MM-dd"
                                                                            )!;
                                    csvFormatterOptions
                                            .DateTimeFormat = exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.DateTimeFormat)
                                                                                , "yyyy-MM-ddTHH:mm:ss.fffff"
                                                                            )!;

                                    csvFormatterOptions
                                            .DigitsTextSuffix = exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.DigitsTextSuffix)
                                                                                , "\t"
                                                                            )!;
                                    var encodingName = Encoding.UTF8.EncodingName;
                                    encodingName = exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.Encoding)
                                                                                , encodingName
                                                                            );

                                    csvFormatterOptions
                                                .Encoding = Encoding.GetEncoding(encodingName!);


                                    csvFormatterOptions
                                            .IncludeExcelDelimiterHeader = exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.IncludeExcelDelimiterHeader)
                                                                                , false
                                                                            );
                                    csvFormatterOptions
                                            .UseSingleLineHeaderInCsv =
                                                        exportCsvFormatterConfiguration
                                                                    .GetValue
                                                                            (
                                                                                nameof(csvFormatterOptions.UseSingleLineHeaderInCsv)
                                                                                , true
                                                                            );
                                }
                            
                                //options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
                                options
                                    .OutputFormatters
                                    .Add
                                        (
                                            new CsvOutputFormatter()
                                        );
                                options
                                    .FormatterMappings
                                    .SetMediaTypeMappingForFormat
                                        (
                                            "csv"
                                            , MediaTypeHeaderValue
                                                    .Parse
                                                        (
                                                            "text/csv"
                                                        )
                                        );
                            }
                        ); 
            #endregion

            services
                    .Configure<KestrelServerOptions>
                        (
                            (options) =>
                            {
                                options
                                        .AllowSynchronousIO = true;
                            }
                        );

            #region JwtBearer Authentication
            services
                    .AddAuthentication
                        (
                            //(options) =>
                            //{
                            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            //}
                            JwtBearerDefaults
                                        .AuthenticationScheme
                        )
                    .AddJwtBearer
                        (
                            (options) =>
                            {
                                options
                                    .TokenValidationParameters =
                                        new TokenValidationParameters()
                                        {
                                            //ValidIssuer = jwtSecurityToken.Issuer
                                            ValidateIssuer = false
                                            //, ValidAudiences = jwtSecurityToken.Audiences
                                            , ValidateAudience = false
                                            , IssuerSigningKey = GlobalManager.jwtSymmetricSecurityKey
                                            , ValidateIssuerSigningKey = true
                                            , ValidateLifetime = false
                                            //, ClockSkew = TimeSpan.FromSeconds(clockSkewInSeconds)
                                        };
                            }
                        ); 
            #endregion
        }
    }
}