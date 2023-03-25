# MsSQLCodeDiffVerioningWeb

## 以下介绍由 ChatGPT 生成:

MsSQL Code Diff Verioning Web 是一个实用 Web 门户工具，用于跟踪和比较不同历史版本的 Microsoft SQL Server 数据库对象 (如: 表、存储过程、视图、函数 等)代码，可检测和对比代码差异，方便管理数据库，有效提升代码开发和维护工作的效率。

## The following is generated by ChatGPT:

MsSQL Code Difference Versioning Web is an website for quickly and easily creating applications that allow users to tracking and compare the different history versions of Microsoft SQL Server database object's (table, store procedure, view, function) code within a Microsoft SQL Server database. It provides a user-friendly interface to quickly identify differences and helps keep track of changes while ensuring data accuracy and integrity.

## Run/debug entry program/project
```
MsSQLCodeDiffVerioningWeb\MsSqlCodeDiffVersioningWeb.NET\MsSqlCodeDiffVersioningWeb.NET.Windows.csproj
```

## DataBase preparation
* Get Microsoft SQL Server Scripts location and then run them on your SQL Server Database
```
MsSQLCodeDiffVerioningWeb\MsSqlCodeDiffVersioningWeb.NET\00.DataBase
```
* Config DataBase connection string in secrets.json for `MsSqlCodeDiffVersioningWeb.NET.Windows`
    - userSecretsID location
    ```
    MsSQLCodeDiffVerioningWeb\MsSqlCodeDiffVersioningWeb.NET\usersSecretsID.json
    ```
    * secrets.json
        * location
        ```
        The <user_secrets_id> as below must in above "MsSQLCodeDiffVerioningWeb\MsSqlCodeDiffVersioningWeb.NET\usersSecretsID.json" 
        windows: "%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json"
        linux/macOS: ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
        ```
        * content ,modify it for your environment
        ```json
        {
          "NeedAutoRefreshExecutedTimeForSlideExpire": "False",
          "Kestrel:Certificates:Development:Password": "50b2c562-3eda-4e30-b6a9-d8066b897a8c",
          "CachedParametersDefinitionExpiredInSeconds": "10",


          "Connections:c1:dataBasetype": "mssql",
          "Connections:c1:connectionString": "Initial Catalog=test;Data Source=.\\sql2019,11433;User=sa;Password=w!th0utp@$$w0rd",
          "Connections:c1:commandtimeoutInSeconds": "102",
          "Connections:c1:enableStatistics": "True",

        }
        ```
## Usage

1. Modify some code of database object for multiple times by using `alter/create` sql statement
    ```sql
    CREATE procedure [dbo].[zsp_001]
    as
    begin
    select 1
    end
    
    go

    alter procedure [dbo].[zsp_001]
    as
    begin
    select 1
    end
    
    go

    alter procedure [dbo].[zsp_001]
    as
    begin
    select 3
    end
    ```

1. Run/debug entry program/project
    ```
    MsSQLCodeDiffVerioningWeb\MsSqlCodeDiffVersioningWeb.NET\MsSqlCodeDiffVersioningWeb.NET.Windows.csproj
    ```

1.  Portal entry
    ```
    # sql code version tracking and compare, click search button
    https://localhost:7283/index.html

    # sql online query
    https://localhost:7283/datatable.html
    ```



