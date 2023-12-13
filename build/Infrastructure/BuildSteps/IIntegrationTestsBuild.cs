using Nuke.Common;
using Nuke.Common.Tools.DotNet;

namespace Infrastructure.BuildSteps;

public interface IIntegrationTestsBuild : IBaseBuild
{
    bool ExecuteIntegrationTests => false;

    // A good alternative would be running the integration tests inside the Dockerfile
    // or have this step depend on a simple Build step not on DockerBuild.
    Target RunIntegrationTests => _ => _
        .TryDependsOn<IDockerBuild>(x => x.BuildDockerfileWithArtifacts)
        .OnlyWhenDynamic(() => ExecuteIntegrationTests)
        .WhenSkipped(DependencyBehavior.Execute)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(settings =>
                settings
                    .SetProjectFile(Solution)
                    .SetConfiguration("Release")
                    .SetVerbosity(DotNetVerbosity.Quiet)
                    .EnableNoLogo());
            
            return DotNetTasks.DotNetTest(settings =>
                settings
                    .SetProjectFile(Solution)
                    .SetConfiguration("Release")
                    .SetVerbosity(DotNetVerbosity.Normal)
                    .SetLoggers("console;verbosity=normal")
                    .EnableNoLogo()
                    .EnableNoRestore()
                    .EnableNoBuild());
        });
}
