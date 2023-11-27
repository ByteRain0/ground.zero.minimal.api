using System;
using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "nuke-default-pipeline",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(RunBuild) },
    ImportSecrets = new []{ "DockerUserName", "DockerPassword"})]
class Build : NukeBuild, IDefaultBuildFlow
{
    public string ServiceName => "HabitTracker";

    public ApplicationVersion Version => this.UseSemanticVersion(major: 1, minor: 0);

    public bool ExecuteIntegrationTests => true;
    
    public IReadOnlyList<DockerImageInfo> DockerImages { get; } = new[]
    {
        new DockerImageInfo(DockerImageName: "habit-tracker-api", DockerfileName: "Dockerfile"),
    };

    private Target RunBuild => _ => _
        .DependsOn<IDefaultBuildFlow>(x => x.Default)
        .Executes(() =>
        {
        });

    public static int Main()
        => Execute<Build>(x => x.RunBuild);
}