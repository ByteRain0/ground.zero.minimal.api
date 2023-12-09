using Infrastructure.BuildSteps;
using Nuke.Common;

namespace Infrastructure;

public interface IBuildPipeline :
    IDockerBuild,
    IIntegrationTestsBuild,
    IReleaseBuild
{
    Target Default => _ => _
        .TryDependsOn<IReleaseBuild>(x => x.CreateRelease);
}