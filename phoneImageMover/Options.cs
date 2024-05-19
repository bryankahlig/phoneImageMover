using CommandLine;

namespace phoneImageMover
{
    public class Options
    {
        [Option('v', "verbose", Default = false, Required = false, HelpText = "See all the output")]
        public bool Verbose { get; set; }

        [Option('l', "livemode", Default = false, Required = false, HelpText = "Run in test mode. Default is true. No modifications are made when true.")]
        public bool LiveMode { get; set; }

        [Option('s', "sourcePath", Required = true, HelpText = "The path to the source of the files.")]
        public string? SourcePath { get; set; }

        [Option('d', "destinationPath", Default = "D:\\Pictures\\", Required = false, HelpText = "The path to the destination where the files will be saved. A full yyyy\\mm path will be added based on the date of the file being moved.")]
        public string? DestinationPath { get; set; }

        [Option('e', "extensions", Default = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".mp4", ".mpg", ".avi" }, Required = false, HelpText = "The file extensions to move.")]
        public string[]? Extensions { get; set; }
    }
}
