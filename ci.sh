echo $buildConfiguration
# dotnet build MsSQLCodeDiffVerioningWeb.sln --configuration $buildConfiguration

# mkdir -p MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/wwwroot/
# cp -rf MsSqlCodeDiffVersioningWeb.NET.6.x/wwwroot/* MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/wwwroot/

# mkdir -p MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/RoutesConfig/
# cp -rf MsSqlCodeDiffVersioningWeb.NET.6.x/RoutesConfig/* MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/RoutesConfig/

mkdir -p MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/Plugins/
cp -rf Plugins.StoreProcedureExecutors/MsSQL.StoreProcedureExecutor.Plugin.NET.6.x/bin/$buildConfiguration/net6.0/*Plugin* MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/Plugins/
cp -rf Plugins.StoreProcedureExecutors/MySQL.StoreProcedureExecutor.Plugin.NET.6.x/bin/$buildConfiguration/net6.0/*Plugin* MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/Plugins/
cp -rf Plugins.RequestJTokenValidators/JTokenValidatorSamplePlugin.NET.6.x/bin/$buildConfiguration/net6.0/*Plugin* MsSqlCodeDiffVersioningWeb.NET.6.x/bin/$buildConfiguration/net6.0/Plugins/