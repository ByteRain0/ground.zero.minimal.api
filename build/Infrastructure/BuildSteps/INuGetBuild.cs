using System;
using System.Linq;
using Infrastructure.Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace Infrastructure.BuildSteps;

public interface INuGetBuild: IBaseBuild
{
    [Parameter("NuGet url"), Required]
    Uri NugetUrl => this.GetValue(() => NugetUrl);

    [Parameter("NuGet feed name"), Required]
    string NugetFeedName => this.GetValue(() => NugetFeedName);

    [Parameter("NuGet API key"), Required]
    //[Secret]
    string NugetApiKey => this.GetValue(() => NugetApiKey);

    AbsolutePath NuGetArtifactsPath => ArtifactsPath / "nuget";
    
    Target PushNuGetArtifacts => _ => _
        .TryDependsOn<IIntegrationTestsBuild>(x=> x.RunIntegrationTests)
        .Executes(() =>
        {
            // Here is the place to publish NuGet artifacts to your NuGet feed
            /*
            var nuGetPushUrl = Url.Combine($"nuget/{FeedName}/packages");

            DotNetNuGetPush(settings =>
                settings
                    .SetTargetPath(NuGetArtifactsPath / "*.nupkg")
                    .SetSource(nuGetPushUrl.ToString())
                    .SetApiKey(ApiKey)
                    .EnableSkipDuplicate()
                    .EnableForceEnglishOutput());s
            */

            var pushedArtifacts = NuGetArtifactsPath.GetFiles("*.nupkg")
                .Select(x => x.Name);
            
            Log.Information("Nuget artifacts were successfully pushed: {Artifacts}", pushedArtifacts);
        });
}