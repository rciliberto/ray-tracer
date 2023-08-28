# Ray Tracer
A ray-tracing engine written by Robby Ciliberto in C# / .NET 7 for CS 5360 Non-Interactive Computer Graphics At Northeastern University.

Additional resources used to assist in the creation of this project include [Ray Tracing in One Weekend](https://raytracing.github.io) by Peter Shirley and ScratchAPixel's [Ray Tracing Minibooks](https://www.scratchapixel.com/index.html).

## Video
[![Multithreading and Octree Optimization in a Ray Tracer](https://img.youtube.com/vi/mH4jevXVyGM/0.jpg)](https://www.youtube.com/watch?v=mH4jevXVyGM "Multithreading and Octree Optimization in a Ray Tracer")

### Results
```
On my machine:
Apple M2 Max (8 performance and 4 efficiency cores)

No optimizations:
Render Time    : 00:29:20.1646423
Percent Change : 0.0%
Speed Up       : 0x

Just MultiThreaded:
Render Time    : 00:03:08.1406913
Percent Change : 89.3% Decrease
Speed Up       : 9.3x

Just octree:
Render Time    : 00:02:16.7476687
Percent Change : 92.2% Decrease
Speed Up       : 12.8x

Both threading and octree:
Render Time    : 00:00:18.5429079
Percent Change : 98.9% Decrease
Speed Up       : 90.9x
```


## Build

### Prerequisites

Install [.NET Core 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Publish
Navigate to the RayTracer project folder in your terminal and run the following command:

```shell
dotnet publish -c Release -o .
```

## Run

Run the generated program

```
./RayTracer <file> [options]
```

```
❯ ./RayTracer -h
Description:
  Reads a Wavefront .obj file and renders a scene with that object.

Usage:
  RayTracer <file> [options]

Arguments:
  <file>  The Wavefront .obj file to read.

Options:
  -V, --verbose              Enables verbose diagnostic information to be printed to the console.
  -B, --no-bounding-volumes  Disables bounding volume optimization for the render.
  -M, --no-submesh-volumes   Disables sub-mesh octree bounding volume optimization for the render.
  -S, --single-threaded      Forces the render to run on a single thread.
  --version                  Show version information
  -?, -h, --help             Show help and usage information
```

Example output:
```
❯ ./RayTracer ../../common/objects/bunny_centered.obj -V
                                                                        
Rendering Image ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   00:00:18
                                                                        
Render Time                   : 00:00:18.4976643
Rays Cast                     : 25110673
Bounding Volume Intersections : 74112806
Object Intersections          : 27629569

```
