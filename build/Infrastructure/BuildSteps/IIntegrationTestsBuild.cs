using Nuke.Common;
using Nuke.Common.Tools.DotNet;

namespace Infrastructure.BuildSteps;

public interface IIntegrationTestsBuild : IBaseBuild
{
    bool ExecuteIntegrationTests => false;

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