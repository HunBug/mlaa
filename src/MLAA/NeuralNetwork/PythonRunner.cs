using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    internal class PythonRunner
    {
        private readonly ILogger<PythonRunner> logger;
        public string PythonExecutablePath { get; private set; } = "python";
        public string PythonScriptPath { get; private set; } = "";
        public PythonRunner(string pythonExecutablePath, string pythonScriptPath, ILogger<PythonRunner>? logger = null)
        {
            PythonExecutablePath = pythonExecutablePath;
            PythonScriptPath = pythonScriptPath;
            if (logger != null)
            {
                this.logger = logger;
            }
            else
            {
                this.logger = new LoggerFactory().CreateLogger<PythonRunner>();
            }
            
        }

        public void Run(IDictionary<string, string> args)
        {
            var arguments = new List<string>();
            foreach (var arg in args)
            {
                arguments.Add($"--{arg.Key}");
                arguments.Add($"{arg.Value}");
            }
            Run(arguments.ToArray());
        }

        public void Run(object args)
        {
            var arguments = new Dictionary<string, string>();
            foreach (var arg in args.GetType().GetProperties())
            {
                var argValue = arg.GetValue(args)?.ToString();
                if (argValue != null)
                {
                    arguments.Add(arg.Name, argValue);
                }
                else
                {
                    logger.LogWarning("Argument {ArgName} is null", arg.Name);
                }
            }
            Run(arguments);
        }

        public void Run(string[] args)
        {
            var arguments = new StringBuilder();
            foreach (var arg in args)
            {
                arguments.Append($"\"{arg}\" ");
            }
            var process = new Process();
            process.StartInfo.FileName = PythonExecutablePath;
            process.StartInfo.Arguments = $"\"{PythonScriptPath}\" {arguments}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            //create event handler
            // Reading synchronously from the output,error stream of the process can cause the process to hang.
            process.OutputDataReceived += new DataReceivedEventHandler(
                (s, e) =>
                {
                    //do something with the output data 'e.Data'
                    logger.LogInformation("Python output: {PythonOutput}", e.Data);
                }
            );
            process.ErrorDataReceived += new DataReceivedEventHandler(
                (s, e) =>
                {
                    logger.LogError("Python error: {ErrorData}", e.Data);
                }
            );
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                logger.LogError("Python process exited with code {ExitCode}", process.ExitCode);
                throw new ApplicationException($"Python script exited with error code {process.ExitCode}. See output for more information.");
            }
        }
    }

}
