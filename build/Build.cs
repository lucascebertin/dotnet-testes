using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";

    Project WebApp => Solution.GetProject("Sample.Testes.App").NotNull();
    Project UnitTestsProject => Solution.GetProject("Sample.Testes.Unidade").NotNull();
    Project IntegrationTestsProject => Solution.GetProject("Sample.Testes.Integracao").NotNull();
    Project AcceptanceTestsProject => Solution.GetProject("Sample.Testes.Aceitacao").NotNull();

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target UnitTest => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                 .SetProjectFile(UnitTestsProject)
                 .SetConfiguration(Configuration)
                 .SetNoBuild(InvokedTargets.Contains(Compile))
                 .ResetVerbosity()
                 .SetResultsDirectory(TestResultDirectory)
                 .SetProperty("CollectCoverage", "true")
                 .SetProperty("CoverletOutputFormat", "opencover"));
        });

    Target AcceptanceTest => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                 .SetProjectFile(AcceptanceTestsProject)
                 .SetConfiguration(Configuration)
                 .SetNoBuild(InvokedTargets.Contains(Compile))
                 .ResetVerbosity()
                 .SetResultsDirectory(TestResultDirectory)
                 .SetProperty("CollectCoverage", "true")
                 .SetProperty("CoverletOutputFormat", "opencover"));
        });


    Target IntegrationTest => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                 .SetProjectFile(IntegrationTestsProject)
                 .SetConfiguration(Configuration)
                 .SetNoBuild(InvokedTargets.Contains(Compile))
                 .ResetVerbosity()
                 .SetResultsDirectory(TestResultDirectory)
                 .SetProperty("CollectCoverage", "true")
                 .SetProperty("CoverletOutputFormat", "opencover"));
        });

    Target AllTests => _ => _
        .Executes(UnitTest)
        .Executes(IntegrationTest)
        .Executes(AcceptanceTest);

    Target Publish => _ => _
        .DependsOn(AllTests)
        .Executes(() =>
        {
            var publishSettings = new DotNetPublishSettings()
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetOutput(OutputDirectory);

            DotNetPublish(s => publishSettings
                .SetProject(WebApp));
        });

}
