using Nuke.Common;
using Serilog;

namespace Infrastructure.BuildSteps;

public interface IReleaseBuild : IBaseBuild
{
    Target CreateRelease => _ => _
        .TryDependsOn<IDockerBuild>(x => x.PushDockerArtifacts)
        .Executes(() =>
        {
            //TODO: Put here any logic you need to create release using your CI/CD system API

            Log.Information("Release {ReleaseName} was successfully created for service {ServiceName}",
                Version.FullVersion, ServiceName);
        });
}