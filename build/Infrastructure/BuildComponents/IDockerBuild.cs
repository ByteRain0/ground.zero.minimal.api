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

    [Parameter] [Secret]
    string DockerUserName => this.GetValue(() => DockerUserName);

    [Parameter] [Secret]
    string DockerPassword => this.GetValue(() => DockerPassword);

    /// <summary>
    /// Path for nuget packages artifacts
    /// </summary>
    const string DockerContainerArtifactsPath = "/app/artifacts";

    IReadOnlyList<DockerImageInfo> DockerImages { get; }
    
    string GetDockerImageTag(string dockerImageName) => $"{RepositoryName}/{dockerImageName}:{Version.FullVersion}";
        //$"{RepositoriesUrl.Authority}/{RepositoryName}/{dockerImageName}:{Version.FullVersion}"; //GitHub image registry setup.
    
        
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
            
            DockerTasks.DockerLogin(settings => settings
                .SetPassword(DockerPassword)
                .SetUsername(DockerUserName));
            
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