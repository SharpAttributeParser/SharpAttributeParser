public class PublishData
{
    public string NuGetSource { get; } = "https://api.nuget.org/v3/index.json";
    public string GitHubPackagesSource { get; } = "https://nuget.pkg.github.com/ErikWe/index.json";

    public string NuGetKey { get; }
    public string GitHubKey { get; }

    private PublishData(string nugetKey, string githubKey)
    {
        NuGetKey = nugetKey;
        GitHubKey = githubKey;
    }

    public static PublishData ExtractPublishData(ICakeContext context)
    {
        var nugetKey = context.EnvironmentVariable("NUGET_API_KEY");
        var githubKey = context.EnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(nugetKey))
        {
            throw new InvalidOperationException("Could not resolve NuGet API key.");
        }

        if (string.IsNullOrEmpty(githubKey))
        {
            throw new InvalidOperationException("Could not resolve GitHub token.");
        }

        return new(nugetKey, githubKey);
    }
}