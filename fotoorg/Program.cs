using System;
using CommandLine;

namespace fotoorg
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new CmdOptions();
            var isValid = Parser.Default.ParseArguments(args, options);

            if(!isValid)
            {
                Console.WriteLine("Missing required source and target paths.");
                return;
            }

            var fotoorg = new fotoorg(options.SourcePath, options.TargetPath);
            fotoorg.OnBeforeFileCopy += (e, o) => Console.Write(e);
            fotoorg.OnAfterFileCopy += (e, o) => Console.WriteLine(e);

            fotoorg.Start(options.Move);
        }
    }
}
