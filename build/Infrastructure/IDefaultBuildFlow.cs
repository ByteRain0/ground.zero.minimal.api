using Infrastructure.BuildSteps;
using Nuke.Common;

namespace Infrastructure;

public interface IDefaultBuildFlow :
    IDockerBuild,
    IIntegrationTestsBuild,
    IReleaseBuild
{
    Target Default => _ => _
        .TryDependsOn<IReleaseBuild>(x => x.CreateRelease);
}