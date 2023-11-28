using System;
using System.Collections.Generic;
using System.ComponentModel;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Nuke.Common;
using Nuke.Common.Tools.Docker;
using Serilog;

namespace Infrastructure.BuildSteps;

public interface IDockerBuild : IBaseBuild
{
    [Parameter("Docker repositories url"), Required] 
    Uri DockerRepositoriesUrl => this.GetValue(() => DockerRepositoriesUrl);

    [Parameter("Docker repository name"), Required] 
    string DockerRepositoryName => this.GetValue(() => DockerRepositoryName);

    [Parameter("Docker user name")] [Secret]
    string DockerUserName => this.GetValue(() => DockerUserName);

    [Parameter("Docker password")] [Secret]
    string DockerPassword => this.GetValue(() => DockerPassword);
    
    const string DockerContainerArtifactsPath = "/app/artifacts";

    IReadOnlyList<DockerImageInfo> DockerImages { get; }
    
    string GetDockerImageTag(string dockerImageName, ContainerRegistryType containerRegistryType = ContainerRegistryType.PublicDockerHub)
    {
        return containerRegistryType switch
        {
            ContainerRegistryType.PublicDockerHub => $"{DockerRepositoryName}/{dockerImageName}:{Version.FullVersion}",
            ContainerRegistryType.GitHubContainerRegistry => $"{DockerRepositoriesUrl.Authority}/{DockerRepositoryName}/{dockerImageName}:{Version.FullVersion}",
            _ => throw new InvalidEnumArgumentException("Invalid docker container registry passed")
        };
    }
    
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
        .Requires(() => DockerRepositoriesUrl)
        .Executes(() =>
        {
            SetupLogging();
            
            DockerTasks.DockerLogin(settings => settings
                .SetPassword(this.DockerPassword)
                .SetUsername(this.DockerUserName));
            
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