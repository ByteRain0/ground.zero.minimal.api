using System;
using System.Collections.Generic;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Nuke.Common;
using Nuke.Common.Tools.Docker;
using Serilog;

namespace Infrastructure.BuildComponents;

[ParameterPrefix("Docker")]
public interface IDockerBuild : IBaseBuild
{
    [Parameter("Docker repositories url"), Required] 
    Uri RepositoriesUrl => this.GetValue(() => RepositoriesUrl);

    [Parameter("Docker repository name"), Required] 
    string RepositoryName => this.GetValue(() => RepositoryName);

    [Parameter("DOCKERUSERNAME"),Required]
    string UserName => this.GetValue(() => UserName);

    [Parameter("DOCKERPASSWORD"), Required]
    string Password => this.GetValue(() => Password);

    /// <summary>
    /// Path for nuget packages artifacts
    /// </summary>
    const string DockerContainerArtifactsPath = "/app/artifacts";

    IReadOnlyList<DockerImageInfo> DockerImages { get; }

    string GetDockerImageTag(string dockerImageName) =>
        $"{RepositoriesUrl.Authority}/{RepositoryName}/{dockerImageName}:{Version.FullVersion}";

    /// <summary>
    /// Dockerfile processing pipeline: build -> create container -> copy artifacts -> remove container
    /// </summary>
    Target BuildDockerfileWithArtifacts => _ => _
        .Executes(() =>
        {
            SetupLogging();

            foreach (var dockerImageInfo in DockerImages)
            {
                this.BuildDockerfile(dockerImageInfo);

                var containerId = this.CreateDockerContainer(dockerImageInfo.DockerImageName);

                //TODO: uncomment if we have any nuget packages to deploy.
                //this.CopyArtifactsFromContainer(containerId);

                this.RemoveDockerContainer(containerId);
            }
        });

    Target DockerLogIn => _ => _
        .Before(PushDockerArtifacts)
        .Executes(() =>
        {
            DockerTasks.DockerLogin(settings => settings
                .SetPassword(Password)
                .SetUsername(UserName));
        });


    /// <summary>
    /// Push Docker image to the repository
    /// </summary>
    Target PushDockerArtifacts => _ => _
        .TryDependsOn<IIntegrationTestsBuild>(x => x.RunIntegrationTests)
        .Requires(() => RepositoriesUrl)
        .After(DockerLogIn)
        .Executes(() =>
        {
            SetupLogging();
            foreach (var dockerImageInfo in DockerImages)
            {
                DockerTasks.DockerPush(settings =>
                    settings.SetName(GetDockerImageTag(dockerImageInfo.DockerImageName)));
                Log.Information("Docker image {DockerImageName} was pushed to {Url}",
                    dockerImageInfo.DockerImageName, GetDockerImageTag(dockerImageInfo.DockerImageName));
            }
        });

    static void SetupLogging() => DockerTasks.DockerLogger = (_, s) =>
    {
        if (s.Contains("[build-error]"))
        {
            Log.Error(s);
        }
        else
        {
            Log.Debug(s);
        }
    };
}