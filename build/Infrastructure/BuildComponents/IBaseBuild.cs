using Infrastructure.Common;
using Infrastructure.Extensions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Infrastructure.BuildComponents;


public interface IBaseBuild : INukeBuild
{
    [Solution]
    Solution Solution => this.GetValue(() => Solution);

    /// <summary>
    /// Hardcoded for GitHub integration the runId.
    /// </summary>
    [Parameter("Build counter"), Required]
    string BuildCounter
    {
        get
        {
            if (GitHubActions.Instance != null)
            {
                return GitHubActions.Instance.RunId.ToString();
            }

            return this.GetValue(() => BuildCounter);
        }
    }

    [Parameter("Branch name"), Required]
    string Branch => this.GetValue(() => Branch);

    string ServiceName { get; }

    ApplicationVersion Version { get; }

    AbsolutePath ArtifactsPath => RootDirectory / ".artifacts";

    AbsolutePath BuildPath => RootDirectory / "build" / "Build";
}
