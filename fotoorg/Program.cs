using System;
using CommandLine;
using System.Text;

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
                DisplayUsage();
                Console.ReadKey();
                return;
            }

            var fotoorg = new fotoorg(options.SourcePath, options.TargetPath);
            fotoorg.OnBeforeFileCopy += (e, o) => Console.Write(e);
            fotoorg.OnAfterFileCopy += (e, o) => Console.WriteLine(e);

            fotoorg.Start(options.Move, cleanEmptyDir:options.Clean);
        }

        static void DisplayUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Foto Org Options");
            sb.AppendLine("");
            sb.AppendLine("Required Arguments:");
            sb.AppendLine("The source path containing the photos/videos to process.");
            sb.AppendLine("\t-s, -source {SourcePath}");
            sb.AppendLine("The target path to put the processed files into.");
            sb.AppendLine("\t-t, -target {TargetPath}");
            sb.AppendLine("");
            sb.AppendLine("Optional Arguments:");
            sb.AppendLine("Move Files - The files path in the source path will be removed after being it is processed.");
            sb.AppendLine("\t-m, -move");
            sb.AppendLine("Date Fix - The earlier date of the two (Created Date or Last Write Date) will be use as the date for the all target files.");
            sb.AppendLine("\t-d, -datefix");
            sb.AppendLine("");
            Console.WriteLine(sb.ToString());
        }
    }
}
