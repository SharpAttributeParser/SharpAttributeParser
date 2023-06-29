#load "./BuildVersion.cake"
#load "./BuildPaths.cake"
#load "./PublishData.cake"

public class BuildParameters
{
    public string Owner { get; } = "ErikWe";
    public string Repository { get; } = "sharp-attribute-parser";

    public bool IsRunningOnGitHubActions { get; }

    public string Target { get; }
    public string Configuration { get; }
    public string Framework { get; } = "netstandard2.0";
    public string TestFramework { get; } = "net7.0";

    public string SolutionPath { get; } = ".";

    public BuildVersion Version { get; }
    public BuildPaths Paths { get; }

    public PublishData Publish { get; }

    public BuildParameters(ISetupContext context)
    {
        IsRunningOnGitHubActions = context.BuildSystem().GitHubActions.IsRunningOnGitHubActions;

        Target = context.TargetTask.Name;
        Configuration = context.Argument("configuration", "Release");

        Version = BuildVersion.ExtractVersion(context);
        Paths = BuildPaths.ExtractPaths(context);
        Publish = PublishData.ExtractPublishData(context);
    }
}