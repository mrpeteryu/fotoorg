using CommandLine;

namespace fotoorg
{
    class CmdOptions
    {
        [Option('s', "source", Required = true,
            HelpText = "Source folder containing the photos/videos.")]
        public string SourcePath { get; set; }

        [Option('t', "target", Required = true,
         HelpText = "Target folder to receive the photos/videos.")]
        public string TargetPath { get; set; }

        [Option('m', "move", 
         HelpText = "Performs a move of the file instead of copy.")]
        public bool Move { get; set; }
    }
}
