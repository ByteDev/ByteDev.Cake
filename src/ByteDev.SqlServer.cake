void DropSqlLocalDb(string sqlDataSource, string sqlDbName)
{
	var connectionString = $"Data Source={sqlDataSource};Integrated Security=true";

	Information($"Dropping DB: {sqlDbName} using: {connectionString}");
	DropDatabase(connectionString, sqlDbName);
}

void DropSqlAzureDb(string azureSqlConnString, string azureSqlDatabaseName)
{
	Information($"Dropping DB: {azureSqlDatabaseName} using: {azureSqlConnString}");

	try
	{
		DropDatabase(azureSqlConnString, azureSqlDatabaseName);
	}
	catch
	{
		// Azure will often throw an exception back but will delete DB anyway
	}
}

void SqlPackagePublish(FilePath localDacpacFile, string connString)
{
	var sqlPackagePath = GetSqlPackagePath();
	var sqlPackageArgs = $"/Action:Publish /SourceFile:{localDacpacFile} /TargetConnectionString:\"{connString}\"";

	var settings = new ProcessSettings
	{
		Arguments = sqlPackageArgs
	};

	Information($"Deploying DACPAC: {localDacpacFile.FullPath}...");
	Information(sqlPackagePath);
	Information(sqlPackageArgs);

	using(var process = StartAndReturnProcess(sqlPackagePath, settings))
	{
		process.WaitForExit();
	}
}

string GetSqlPackagePath()
{
    var sqlPackagePaths = new[]
    {
        @"C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe",
        @"C:\Program Files\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe",
        @"C:\Microsoft.Data.Tools.Msbuild.10.0.61804.210\lib\net46\SqlPackage.exe"
    };

    foreach (var path in sqlPackagePaths)
    {
        if (FileExists(path))
		{
			Information($"Found SqlPackage.exe: {path}");
            return path;
		}
    }

    throw new Exception("SqlPackage.exe could not be found.");
}

void RunSqlUnitTests(FilePathCollection dllFiles)
{
	var settings = new MSTestSettings
	{
		NoIsolation = false,
		ToolPath = GetMsTestPath()
		//ResultsFile = "./sqlunittests-results.xml"
	};
	
	foreach(var dllFile in dllFiles)
	{
		MSTest(dllFile.FullPath, settings);
	}
}

string GetMsTestPath()
{
	var msTestPaths = new[]
	{
		@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\MSTest.exe",
		@"C:\Program Files\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\MSTest.exe",
		@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe",
		@"C:\Program Files\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe",
		@"C:\Program Files (x86)\Microsoft Visual Studio\2017\TestAgent\Common7\IDE\MSTest.exe"
	};
		
	foreach (var path in msTestPaths)
    {
        if (FileExists(path))
        {
			Information($"Found MSTest.exe: {path}");
			return path;
		}
    }

    throw new Exception("MSTest.exe could not be found.");
}