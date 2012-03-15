# Assembly Comparer
This is a very simple utility that helps you to compare two .NET assemblies and determining if they were 
built from the same source code or not.

## Why?
Sometimes people need to establish whether a given assembly was compiled from a given codebase. 
Typically they recompile the code and do a binary comparison of the output. 
Unfortunately, the .NET assemblies in the build output will **always** differ because the 
compiler bakes in some unique tokens (e.g. a timestamp) every time it does a build. 
This tool gets around this by ignoring those unique tokens when producing a hash of the assembly.

## How?
The tool, which is a simple console app, uses ILDASM to decompile each binary, and then uses regular expression 
replacements to canonicalise some build-specific tokens before computing the MD5 hash of the decompiled output. 
The MD5 hashes of two assemblies can then easily be compared.

I originally tried to modify the binaries in-memory before computing a hash of them. I was using Mono.Cecil 
(inspired by [this thread](https://groups.google.com/forum/#!msg/mono-cecil/AGq0LfBdqjo/jiG6aSNxlUcJ)) and it seemed
to work really well. Unfortunately, builds of the same source code on different machines can result in
quite unpredictable differences in the compiled output. It is probably not impossible to get this tool 
working with the original binary comparison approach, but it seemed like an increasingly daunting task.

The downside of the new, decompilation-based approach is that it is **A LOT** slower. On the upside, it now works! :)

## Building it
1. Clone the project
1. Run `build.bat`. 

If you want to build it from Visual Studio, make sure you use one of the non-default build configurations, e.g. `net_4_0_release`

## Using it
This rudimentary implementation is designed to be used with a diff tool like [Beyond Compare](http://www.scootersoftware.com/index.php). 
As such, it doesn't actually perform a comparison; it justs creates a file containing the build-independant 
MD5 hash of the assembly. In future I may add the obvious feature, which is to compare two assemblies!

### Calling it from the command line
The executable `asmcomp.exe` takes two parameters: the path of the assembly you want to hash 
and the path of the file to save the hash to.

e.g.

`c:\> asmcomp.exe mytest.dll mytest.dll.hash`

### Plugging it into Beyond Compare

1. In Beyond Compare go to **Tools | File Formats...**
1. Choose **New...**
1. Select "Data Format".
1. Name it ".NET Assemblies" or something similar.
1. Set the Mask: `*.exe; *.dll`
1. Choose the **Conversion** tab.
1. Select **External program (Unicode filenames)** from the dropdown.
1. In the **Loading** textbox put the full path to the asmcomp.exe file, followed by `%s %t` so that Beyond Compare will pass the source and destimation paths as the first and second parameters.
e.g. `d:\utilities\AssemblyComparer\asmcomp.exe %s %t`
1. Check the **Disable editing** checkbox.
1. Choose the **Type** tab.
1. Choose **Fixed** and leave the the other fields as they are.
1. Make sure that this file format is *above* any others with a *.exe or *.dll mask. 
1. Choose **Save**

When comparing you need to select "**Rule-based comparison**".

And you're done.

## Note
Beyond Compare will now *always* use `asmcomp` to compare EXEs and DLLs. 
This is safe because when the executable is not a .NET Assembly the tool will fallback to doing an MD5 hash of the entire file.
However, it does mean that you lose Beyond Compare's informative comparison view for executables, which lists the metadata. 
If you only want to use `asmcomp` on an infrequent basis the best thing to do is to adjust the priority of the format - 
go to Tools | File Formats... and just move the format so that it is *below* the default Executable format. Beyond Compare will now work normally. When you want to use asmcomp to compare some assemblies you can just bump the asmcomp File Format back up again.

Happy comparing!


