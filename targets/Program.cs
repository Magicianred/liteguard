using System;
using System.Threading.Tasks;
using SimpleExec;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static Task Main(string[] args)
    {
        Target("build", () => RunAsync("dotnet", "build --configuration Release --nologo --verbosity quiet"));

        Target(
            "pack",
            DependsOn("build"),
            ForEach("LiteGuard.nuspec", "LiteGuard.Source.nuspec"),
            async nuspec => await RunAsync(
                "dotnet",
                "pack LiteGuard --configuration Release --no-build --nologo",
                configureEnvironment: env => env.Add("NUSPEC_FILE", nuspec)));

        Target(
            "test",
            DependsOn("build"),
            () => RunAsync("dotnet", "test --configuration Release --no-build --nologo"));

        Target("default", DependsOn("pack", "test"));

        return RunTargetsAndExitAsync(args, ex => ex is NonZeroExitCodeException);
    }
}
