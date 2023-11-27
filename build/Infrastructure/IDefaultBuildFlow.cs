using Infrastructure.BuildComponents;
using Nuke.Common;

namespace Infrastructure;


/// <summary>
/// Default build flow
/// </summary>
public interface IDefaultBuildFlow :
    IDockerBuild,
    IIntegrationTestsBuild,
    //INuGetBuild,
    IReleaseBuild
{
    Target Default => _ => _
        .TryDependsOn<IReleaseBuild>(x => x.CreateRelease);
}