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

    const string DockerContainerArtifactsPath = "/app/artifacts";

    IReadOnlyList<DockerImageInfo> DockerImages { get; }

    string GetDockerImageTag(string dockerImageName) => $"{RepositoriesUrl.Authority}/{RepositoryName}/{dockerImageName}:{Version.FullVersion}";

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

    /// <summary>
    /// Push Docker image to the repository
    /// </summary>
    Target PushDockerArtifacts => _ => _
        .TryDependsOn<IIntegrationTestsBuild>(x => x.RunIntegrationTests)
        .Requires(() => RepositoriesUrl)
        .Executes(() =>
        {
            SetupLogging();
            
            foreach (var dockerImageInfo in DockerImages)
            {
                // DockerPush(settings => settings.SetName(GetDockerImageNameTag(dockerImageInfo.DockerImageName)));
                Log.Information("Docker image {DockerImageName} was pushed to {Url}", 
                    dockerImageInfo.DockerImageName, GetDockerImageTag(dockerImageInfo.DockerImageName));
            }
        });

    // // Workaround for logging issue in Nuke with Docker tasks
    // // See more details here: https://nuke.build/faq
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