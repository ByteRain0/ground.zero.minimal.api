using System;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "hiroshima",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(PublishTestResults) })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.PublishTestResults);

    static readonly string TestResultsDirectory = "TestResults";


    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(TestResultsDirectory);
            DotNetTasks.DotNetClean();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild();
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(_ => _
                .EnableNoRestore()
                .EnableNoBuild()
                .SetResultsDirectory(TestResultsDirectory));
        });

    Target PublishTestResults => _ => _
        .DependsOn(Test)
        .Produces(TestResultsDirectory + "/*.trx")
        .Executes(() =>
        {
            // PublishTestResults target implementation
            // This target can be used to publish test results to GitHub Actions
            Console.WriteLine($"##vso[results.publish type=VSTest;mergeFiles={TestResultsDirectory}/*.trx]");
        });

    private static void EnsureCleanDirectory(string directory)
    {
        if (System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.Delete(directory, true);
        }

        System.IO.Directory.CreateDirectory(directory);
    }
}