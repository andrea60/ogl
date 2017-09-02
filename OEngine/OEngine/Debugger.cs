using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    public enum Severity
    {
        Warning,
        Information,
        Error,
        Fatal
    }
    public static class Debugger
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        private static bool ConsoleLog = true;
        private static StreamWriter Writer = null;
        private static object WriterLock = new object();

        public static void Init(bool consoleLog = true)
        {
            ConsoleLog = consoleLog;
            Writer = new StreamWriter($"{AssemblyDirectory}\\opengl.log");
        }
        public static void Log(string message, Severity severity = Severity.Information)
        {
            if (string.IsNullOrEmpty(message))
                return;
            lock (WriterLock)
            {
                var dataOra = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]";
 
                Writer.WriteLine($"{dataOra} - {message}");
                Writer.Flush();
                if (ConsoleLog)
                {
                    Console.Write($"{dataOra} ");
                    ConsoleColor color = ConsoleColor.White;
                    if (severity == Severity.Warning)
                        color = ConsoleColor.Yellow;
                    else if (severity == Severity.Error)
                        color = ConsoleColor.Red;
                    else if (severity == Severity.Fatal)
                        color = ConsoleColor.Magenta;
                    ConsoleWrite($"[{severity.ToString().ToUpper()}] - ", color);
                    Console.WriteLine(message);
                }
            }
        }

        private static void ConsoleWrite(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        private static void ConsoleWriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
