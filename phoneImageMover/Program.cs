using CommandLine;
using ExifLib;
using phoneImageMover;
using System.IO.Enumeration;
using System.Reflection;

partial class Program
{
    static int Main(string[] args)
    {
        var processor = new Processor();
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(processor.Run)
            .WithNotParsed(HandleParseError);

        return 0;
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
    }
}
