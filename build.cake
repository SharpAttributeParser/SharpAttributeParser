#addin "nuget:?package=Polly&version=7.2.3"

#tool dotnet:?package=GitVersion.Tool&version=5.11.1
#tool dotnet:?package=GitReleaseManager.Tool&version=0.13.0

#load "./build/BuildParameters.cake"

using Polly;

Setup<BuildParameters>((context) =>
{
    ArgumentNullException.ThrowIfNull(context);

    var parameters = new BuildParameters(context);

    Information($"Building SharpAttributeParser {parameters.Version.NuGet} ({parameters.Configuration}, {parameters.Target}).");

    return parameters;
});

Task("Clean")
    .Does<BuildParameters>((context, parameters) =>
    {
        CleanDirectory(parameters.Paths.Artifacts);

        DotNetCleanSettings settings = new()
        {
            Verbosity = DotNetVerbosity.Minimal,
            Configuration = parameters.Configuration
        };

        DotNetClean(parameters.SolutionPath, settings);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetRestoreSettings restoreSettings = new()
        {
            Verbosity = DotNetVerbosity.Minimal
        };

        DotNetRestore(parameters.SolutionPath, restoreSettings);
    });

Task("Format")
    .IsDependentOn("Restore")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetFormatSettings settings = new()
        {
            NoRestore = true,
            VerifyNoChanges = true
        };

        DotNetFormat(parameters.SolutionPath, settings);
    });

Task("Build")
    .IsDependentOn("Format")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetMSBuildSettings buildSettings = new()
        {
            Version = parameters.Version.MajorMinorPatch,
            AssemblyVersion = parameters.Version.MajorMinorPatch,
            FileVersion = parameters.Version.MajorMinorPatch
        };

        DotNetBuildSettings settings = new()
        {
            Configuration = parameters.Configuration,
            NoRestore = true,
            NoIncremental = true,
            MSBuildSettings = buildSettings
        };

        DotNetBuild(parameters.SolutionPath, settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .DoesForEach<BuildParameters, FilePath>(() => GetFiles("./tests/**/*.csproj"), (parameters, project, context) =>
    {
        FilePath testResultsPath = MakeAbsolute(parameters.Paths.TestResults
            .CombineWithFilePath($"{project.GetFilenameWithoutExtension()}_TestResults.xml"));

        DotNetTestSettings settings = new()
        {
            Framework = parameters.TestFramework,
            Configuration = parameters.Configuration,
            NoRestore = true,
            NoBuild = true
        };

        DotNetTest(project.FullPath, settings);
    })
    .DeferOnError();

Task("Pack")
    .IsDependentOn("Test")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetMSBuildSettings msBuildSettings = new()
        {
            PackageVersion = parameters.Version.NuGet
        };

        DotNetPackSettings packSettings = new()
        {
            Configuration = parameters.Configuration,
            NoRestore = true,
            NoBuild = true,
            IncludeSymbols = true,
            SymbolPackageFormat = "snupkg",
            OutputDirectory = parameters.Paths.NuGet,
            MSBuildSettings = msBuildSettings
        };

        var projects = GetFiles("./src/**/*.csproj");

        foreach(var project in projects)
        {
            DotNetPack(project.FullPath, packSettings);
        }
    });

Task("Publish-GitHub-Release")
    .IsDependentOn("Pack")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnGitHubActions, "Build is not running on GitHub Actions.")
    .Does<BuildParameters>((context, parameters) =>
    {
        GitReleaseManagerCreateSettings createSettings = new()
        {
            ArgumentCustomization = (args) => args.Append("--allowEmpty"),
            Debug = true,
            NoLogo = true,
            Name = parameters.Version.Release
        };

        GitReleaseManagerAddAssetsSettings addAssetsSettings = new()
        {
            Debug = true,
            NoLogo = true
        };

        GitReleaseManagerPublishSettings publishSettings = new()
        {
            Debug = true,
            NoLogo = true
        };

        GitReleaseManagerCreate(parameters.Publish.GitHubKey, parameters.Owner, parameters.Repository, createSettings);

        var assets = GetFiles($"{parameters.Paths.NuGet}/*");

        var assetList = string.Join(',', assets.Select(static (asset) => asset.FullPath));

        Policy
            .Handle<Exception>()
            .WaitAndRetry(5, (attempt) => TimeSpan.FromSeconds(attempt * 5))
            .Execute(() => {
                GitReleaseManagerAddAssets(parameters.Publish.GitHubKey, parameters.Owner, parameters.Repository, parameters.Version.Release, assetList, addAssetsSettings);
            });

        GitReleaseManagerPublish(parameters.Publish.GitHubKey, parameters.Owner, parameters.Repository, parameters.Version.Release, publishSettings);
    });

Task("Publish-GitHub-Packages")
    .IsDependentOn("Publish-GitHub-Release")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnGitHubActions, "Build is not running on GitHub Actions.")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetNuGetPushSettings settings = new()
        {
            ApiKey = parameters.Publish.GitHubKey,
            Source = parameters.Publish.GitHubPackagesSource
        };

        var packages = GetFiles($"{parameters.Paths.NuGet}/*.nupkg");

        foreach(var package in packages)
        {
            DotNetNuGetPush(package, settings);
        }
    });

Task("Publish-NuGet")
    .IsDependentOn("Publish-GitHub-Release")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnGitHubActions, "Build is not running on GitHub Actions.")
    .Does<BuildParameters>((context, parameters) =>
    {
        DotNetNuGetPushSettings settings = new()
        {
            ApiKey = parameters.Publish.NuGetKey,
            Source = parameters.Publish.NuGetSource
        };

        var packages = GetFiles($"{parameters.Paths.NuGet}/*.nupkg");

        foreach(var package in packages)
        {
            DotNetNuGetPush(package, settings);
        }
    });

Task("Publish")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-GitHub-Packages")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(Argument("target", "Default"));