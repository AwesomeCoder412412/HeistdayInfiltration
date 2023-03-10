using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NiceIO;

public static partial class BuildUtils
{
    public static bool CallShell(string exe, string args, out string output, out string error)
    {
        var start = new ProcessStartInfo("xcode-select", args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
        };
        string o = "";
        string er = "";
        bool success;
        using (var process = Process.Start(start))
        {
            process.OutputDataReceived += (sender, e) => { o += e.Data; };
            process.ErrorDataReceived += (sender, e) => { er += e.Data; };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(); //? doesn't work correctly if time interval is set
            success = process.ExitCode == 0;
        }

        output = o;
        error = er;
        return success;
    }
}
