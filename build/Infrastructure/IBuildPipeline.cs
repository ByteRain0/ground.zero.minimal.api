using Infrastructure.BuildSteps;
using Nuke.Common;

namespace Infrastructure;

public interface IBuildPipeline :
    IDockerBuild,
    IIntegrationTestsBuild,
    IReleaseBuild
{
}