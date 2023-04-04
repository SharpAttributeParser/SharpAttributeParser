public class BuildPaths
{
    public DirectoryPath Artifacts { get; }
    public DirectoryPath TestResults { get; }
    public DirectoryPath NuGet { get; }

    public BuildPaths(DirectoryPath artifacts, DirectoryPath testResults, DirectoryPath nuget)
    {
        Artifacts = artifacts;
        TestResults = testResults;
        NuGet = nuget;
    }

    public static BuildPaths ExtractPaths(ICakeContext context)
    {
        var artifacts = (DirectoryPath)(context.Directory("./artifacts"));
        var testResults = artifacts.Combine("test-results");
        var nuget = artifacts.Combine("nuget");

        return new(artifacts, testResults, nuget);
    }
}