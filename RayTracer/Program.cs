using System.CommandLine;
using RayTracer;
using RayTracer.Core;
using RayTracer.Diagnostics;
using RayTracer.Files.PixelMap;
using RayTracer.Files.Wavefront;
using RayTracer.Image;
using RayTracer.Material;
using RayTracer.Scene;
using RayTracer.Scene.Object;
using Spectre.Console;
using Color = RayTracer.Core.Color;

var rootCommand = new RootCommand("Reads a Wavefront .obj file and renders a scene with that object.");

var fileArgument = new Argument<FileInfo>(
    "file",
    description: "The Wavefront .obj file to read.",
    parse: result =>
    {
        var filePath = result.Tokens.Single().Value;
        if (File.Exists(filePath)) return new FileInfo(filePath);

        result.ErrorMessage = "File does not exist";
        return null!;
    });
rootCommand.AddArgument(fileArgument);

var verboseDiagnosticsOption = new Option<bool>(
    "--verbose",
    "Enables verbose diagnostic information to be printed to the console.");
verboseDiagnosticsOption.AddAlias("-V");
rootCommand.AddOption(verboseDiagnosticsOption);

var disableBoundingVolumeOption = new Option<bool>(
    "--no-bounding-volumes",
    "Disables bounding volume optimization for the render.");
disableBoundingVolumeOption.AddAlias("-B");
rootCommand.AddOption(disableBoundingVolumeOption);

var disableSubmeshVolumeOption = new Option<bool>(
    "--no-submesh-volumes",
    "Disables sub-mesh octree bounding volume optimization for the render.");
disableSubmeshVolumeOption.AddAlias("-M");
rootCommand.AddOption(disableSubmeshVolumeOption);

var singleThreadedOption = new Option<bool>(
    "--single-threaded",
    "Forces the render to run on a single thread.");
singleThreadedOption.AddAlias("-S");
rootCommand.AddOption(singleThreadedOption);

rootCommand.SetHandler(Program, fileArgument, verboseDiagnosticsOption, disableBoundingVolumeOption,
    disableSubmeshVolumeOption, singleThreadedOption);

return await rootCommand.InvokeAsync(args);

void Program(FileInfo file, bool verboseDiagnostics, bool disableBoundingVolumes, bool disableSubmeshVolumes,
    bool singleThreaded)
{
    AppSettings.EnableDiagnostics = verboseDiagnostics;
    AppSettings.UseBoundingVolumes = !disableBoundingVolumes;
    AppSettings.UseSubMeshBoundingVolumes = !disableSubmeshVolumes;

    var grayMirror = new Metal(new Color(0.5, 0.5, 0.5), 0);
    var purpleMetal = new Metal(new Color(0.3, 0.3, 0.6), 0.5f);
    var glass = new Dielectric(1.5f);
    var greenDiffuse = new Lambertian(new Color(0.2, 0.5, 0.1));
    var redDiffuse = new Lambertian(new Color(0.8, 0.1, 0.1));

    var wavefrontFactory = new WavefrontFactory();
    var obj = wavefrontFactory.CreateObject(file);

    var scene = new Scene
    {
        Camera = new Camera
        {
            Origin = new Point3(7, 1, 5),
            VerticalFov = 15
        },
        RenderableObjects = new RenderableObjectList
        {
            new Sphere(new Point3(0, -100.5f, 0), 100, grayMirror),
            new Sphere(new Point3(-1f, 0.5f, -1.2f), 1, purpleMetal),
            new Sphere(new Point3(0, -0.3f, 1.2f), 0.2f, glass),
            new Sphere(new Point3(1f, 0, -1), 0.5f, greenDiffuse),
            new Sphere(new Point3(-1.5f, 0.3f, 0.7f), 0.8f, glass),
            new Sphere(new Point3(-1.5f, 0.3f, 0.7f), -0.6f, glass),
            new Sphere(new Point3(-1.5f, 0.1f, 0.7f), 0.4f, redDiffuse),
            obj
        }
    };

    Task<Render> renderTask;
    Action<ProgressContext> progressContext;
    if (singleThreaded)
    {
        float progress = 0;
        renderTask = Task.Run(() => scene.Render(ref progress));

        progressContext = ctx =>
        {
            var task = ctx.AddTask("[green]Rendering Image[/]");

            while (!renderTask.IsCompleted)
            {
                task.Value = progress * 100;
                Thread.Sleep(100);
            }

            task.Value = 100;
        };
    }
    else
    {
        renderTask = scene.RenderAsync(out var tasks);

        progressContext = ctx =>
        {
            var task = ctx.AddTask("[green]Rendering Image[/]");

            while (!renderTask.IsCompleted)
            {
                task.Value = tasks.GetProgress() * 100;
                Thread.Sleep(100);
            }

            task.Value = 100;
        };
    }

    AnsiConsole.Progress()
        .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new SpinnerColumn(),
            new ElapsedTimeColumn())
        .Start(progressContext);

    var render = renderTask.Result;

    render = PostProcessing.GammaCorrect(render, 2);
    render.SavePixelMap("output.ppm");

    if (AppSettings.EnableDiagnostics) TraceInfo.PrintDiagnostics();
}