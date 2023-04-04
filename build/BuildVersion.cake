public class BuildVersion
{
    public string Release { get; }
    public string NuGet { get; }

    public string MajorMinorPatch { get; }

    private BuildVersion(string release, string nuget, string majorMinorPatch)
    {
        Release = release;
        NuGet = nuget;

        MajorMinorPatch = majorMinorPatch;
    }

    public static BuildVersion ExtractVersion(ICakeContext context)
    {
        var gitVersionSettings = new GitVersionSettings
        {
            NoFetch = true,
            UpdateAssemblyInfo = true
        };

        var gitVersion = context.GitVersion(gitVersionSettings);

        return new($"v{gitVersion.SemVer}", gitVersion.NuGetVersionV2, gitVersion.MajorMinorPatch);
    }
}