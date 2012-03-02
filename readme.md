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
A bit of googling led me to [this thread](https://groups.google.com/forum/#!msg/mono-cecil/AGq0LfBdqjo/jiG6aSNxlUcJ) 
on the Mono.Cecil Google group. The guys there had done all the leg work, all I needed to do was fork 
the Mono.Cecil project, make the changes to implement Florian's suggestion, and create a simple 
console app that lets you use the build-independent hash when comparing files.

## Building it
1. Clone the project
1. Run `git submodule init`
1. Run `git submodule update`
1. Run `build.bat`. 

If you want to build it from Visual Studio, make sure you use one of the non-default build configirations, e.g. `net_4_0_release`

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

And you're done.

## Note
Beyond Compare will now *always* use `asmcomp` to compare EXEs and DLLs. 
This is safe because when the executable is not a .NET Assembly the tool will fallback to doing an MD5 hash of the entire file.
However, it does mean that you lose Beyond Compare's informative comparison view for executables, which lists the metadata. 
If you only want to use `asmcomp` on an infrequent basis the best thing to do is to adjust the priority of the format - 
go to Tools | File Formats... and just move the format so that it is *below* the default Executable format. Beyond Compare will now work normally. When you want to use asmcomp to compare some assemblies you can just bump the asmcomp File Format back up again.

Happy comparing!


