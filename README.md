CSharpCodeProfiler
======================
CSharpCodeProfiler is a profiler class to write directly into your C# code.

Build Environment
------
You need Microsoft Visual Studio Commumity 2013 to build the code.

Usage
------
(1) Add CSharpCodeProfiler.dll or CodeProfiler.cs into your project.
(2) Write the code like TestProject.

```csharp
using (CodeProfiler prof = new CodeProfiler())
{
    TestMethod();

    prof.Stop();
    prof.OutputResultToFile(@"C:\Temp\test.xml");
}
```

(3) Open "C:\Temp\test.xml" from ProfViewer.exe.


License
----------
MIT License.
