using ExifLib;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace phoneImageMover
{
    public class Processor
    {
        public Processor()
        {}

        private Options Options;
        private Logger Logger;
        private FileNameOperations FileNameOperations;

        public void Run(Options options)
        {
            Logger = new Logger(options.Verbose);
            Options = options;
            FileNameOperations = new FileNameOperations(Logger);
            Console.WriteLine($"Source Folder: {options.SourcePath}");
            Console.WriteLine($"Destination Folder: {options.DestinationPath}");
            var directories = Directory.EnumerateDirectories(Path.GetDirectoryName(options.SourcePath));
            foreach (var directory in directories) ProcessFolder(directory, Options.DestinationPath);
        }

        private void ProcessFolder(string source, string destination)
        {
            Logger.logVerbose($"Processing folder: {source}");
            var directories = Directory.EnumerateDirectories(Path.GetDirectoryName(source));
            ProcessFilesInFolder(source, destination);
            foreach (var folder in directories) { ProcessFolder(folder, destination); }
        }

        private void ProcessFilesInFolder(string source, string destination)
        {
            Logger.logVerbose($"Processing files in folder: {source}");
            var files = Directory.EnumerateFiles(Path.GetDirectoryName(source));

            foreach (var file in files)
            {
                if (!ShouldSkipFile(file))
                {
                    Logger.logVerbose("Processing file: " + file);
                    WorkFile(file, destination);
                }
                else { Logger.log("Skipping file: " + file); }
            }
        }

        private void WorkFile(string source, string destinationRoot)
        {
            Logger.logVerbose($"Working on file {source}");
            string destinationFilePath = createDestinationFilePath(source, destinationRoot);
            if (!string.IsNullOrEmpty(destinationFilePath))
            {
                // can work file
                string destination = destinationFilePath + Path.GetFileName(source);
                try
                {
                    if (Options.Testrun)
                    {
                        Logger.log($"TESTRUN: NOT Moving {source} to {destination}");
                    }
                    else
                    {
                        Logger.logVerbose($"Moving {source} to {destination}");
                        Directory.CreateDirectory(destinationFilePath);
                        File.Move(source, destination);
                    }
                } catch (Exception ex)
                {
                    Logger.log($"Unexpected error encountered moving file {source} to {destinationFilePath}/n{ex.Message}");
                }
            }
            else Logger.log($"Unable to move file {source}");
        }

        private string createDestinationFilePath(string source, string destinationRoot)
        {
            string destinationFilePath = string.Empty;
            try
            {
                destinationFilePath = buildDestinationPathByExifData(Path.GetDirectoryName(source), Path.GetFileName(source), destinationRoot);
            }
            catch (InvalidDataException ide)
            {
                Logger.logVerbose($"No exif data for {source}");
            }
            if (string.IsNullOrEmpty(destinationFilePath))
            {
                try
                {
                    var indexOfYearInFilename = FileNameOperations.getIndexOfYearInFilename(source);
                    Logger.logVerbose($"Using filename for date for {source} with year index {indexOfYearInFilename}");
                    destinationFilePath = FileNameOperations.buildDestinationPathByFilename(source, destinationRoot);
                }
                catch (InvalidDataException ide)
                {
                    Logger.logVerbose($"Unable to find year in filename for {source}");
                }
            }
            return destinationFilePath;
        }

        private bool ShouldSkipFile(string filename)
        {
            return (filename.StartsWith(".") || filename.ToLower() == "thumbs.db" || FileNameOperations.getIndexOfYearInFilename(filename) == -1);
        }

        private string buildDestinationPathByExifData(string Path, string fileToMove, string baseDestinationPath)
        {
            DateTime fileDate = DateTime.MinValue;
            Logger.logVerbose("fileDate init: " + fileDate.ToLongTimeString());
            if (fileToMove.ToLower().EndsWith("jpg") || fileToMove.ToLower().EndsWith("jpeg"))
            {
                fileDate = GetExifDateForJpeg(fileToMove);
                Logger.logVerbose($"FileDate exif: {fileDate.ToLongTimeString()}");
            }
            if (fileDate == DateTime.MinValue)
            {
                throw new InvalidDataException("No Exif Data");
            }
            string newPath = baseDestinationPath + fileDate.Year.ToString("D8") + "//" + fileDate.Month.ToString("D2") + "//";
            Logger.logVerbose("newPath: " + newPath);
            Directory.CreateDirectory(newPath);
            return newPath + fileToMove;
        }

        private DateTime GetExifDateForJpeg(string filename)
        {
            using (ExifReader reader = new ExifReader(filename))
            {
                DateTime dtOfPicture;
                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out dtOfPicture))
                {
                    return dtOfPicture;
                }
                return DateTime.MinValue;
            }
        }


    }
}
