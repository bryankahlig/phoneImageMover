using CommandLine;

namespace phoneImageMover
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "See all the output")]
        public bool Verbose { get; set; }

        [Option('t', "testrun", Default = true, Required = false, HelpText = "Run in test mode. Default is true. No modifications are made when true.")]
        public bool Testrun { get; set; }

        [Option('s', "sourcePath", Required = true, HelpText = "The path to the source of the files.")]
        public string? SourcePath { get; set; }

        [Option('d', "destinationPath", Default = "D:\\Pictures\\", Required = false, HelpText = "The path to the destination where the files will be saved. A full yyyy\\mm path will be added based on the date of the file being moved.")]
        public string? DestinationPath { get; set; }
    }
}
