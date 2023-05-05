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
            ProcessFilesInFolder(options.SourcePath, options.DestinationPath);
            var directories = Directory.EnumerateDirectories(Path.GetDirectoryName(options.SourcePath));
            foreach (var directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    ProcessFolder(directory, Options.DestinationPath);
                }
            }
        }

        private void ProcessFolder(string sourceFolder, string destination)
        {
            Logger.logVerbose($"Processing folder: {sourceFolder}");
            var directories = Directory.EnumerateDirectories(sourceFolder);
            ProcessFilesInFolder(sourceFolder, destination);
            foreach (var folder in directories)
            {
                if (!folder.StartsWith(".")) ProcessFolder(folder, destination);
            }
        }

        private void ProcessFilesInFolder(string sourceFolder, string destination)
        {
            Logger.logVerbose($"Processing files in folder: {sourceFolder}");
            var files = Directory.EnumerateFiles(sourceFolder);

            foreach (var pathAndFilename in files)
            {
                if (!ShouldSkipFile(pathAndFilename))
                {
                    Logger.logVerbose("Processing file: " + pathAndFilename);
                    WorkFile(pathAndFilename, destination);
                }
                else { Logger.log("Skipping file: " + pathAndFilename); }
            }
        }

        private void WorkFile(string sourcePathAndFilename, string destinationRoot)
        {
            var filename = Path.GetFileName(sourcePathAndFilename);
            Logger.logVerbose($"Working on file {sourcePathAndFilename}");
            string destinationFilePath = createDestinationFilePath(sourcePathAndFilename, destinationRoot);
            if (!string.IsNullOrEmpty(destinationFilePath))
            {
                // can work file
                string destination = destinationFilePath;
                try
                {
                    if (!Options.LiveMode)
                    {
                        Logger.log($"TESTRUN: NOT Moving {sourcePathAndFilename} to {destination}");
                    }
                    else
                    {
                        Logger.logVerbose($"Moving {sourcePathAndFilename} to {destination}");
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                        File.Move(sourcePathAndFilename, destination);
                    }
                } catch (Exception ex)
                {
                    Logger.log($"Unexpected error encountered moving file {sourcePathAndFilename} to {destinationFilePath}/n{ex.Message}");
                }
            }
            else Logger.log($"Unable to move file {sourcePathAndFilename}");
        }

        private string createDestinationFilePath(string sourcePathAndFilename, string destinationRoot)
        {
            string destinationFilePath = string.Empty;
            try
            {
                destinationFilePath = buildDestinationPathByExifData(sourcePathAndFilename, destinationRoot);
            }
            catch (InvalidDataException ide)
            {
                Logger.logVerbose($"No exif data for {sourcePathAndFilename}");
            }
            if (string.IsNullOrEmpty(destinationFilePath))
            {
                var filename = Path.GetFileName(sourcePathAndFilename);
                try
                {
                    var indexOfYearInFilename = FileNameOperations.getIndexOfYearInFilename(filename);
                    Logger.logVerbose($"Using filename for date for {sourcePathAndFilename} with year index {indexOfYearInFilename}");
                    destinationFilePath = FileNameOperations.buildDestinationPathByFilename(filename, destinationRoot);
                }
                catch (InvalidDataException ide)
                {
                    Logger.logVerbose($"Unable to find year in filename for {sourcePathAndFilename}");
                }
            }
            return destinationFilePath;
        }

        private bool ShouldSkipFile(string pathAndFilename)
        {
            var filename = Path.GetFileName(pathAndFilename);
            return (filename.StartsWith(".")
                || filename.StartsWith("~")
                || filename.ToLower() == "thumbs.db"
                || FileNameOperations.getIndexOfYearInFilename(filename) == -1);
        }

        private string buildDestinationPathByExifData(string sourcePathAndFilename, string baseDestinationPath)
        {
            DateTime fileDate = DateTime.MinValue;
            Logger.logVerbose("fileDate init: " + fileDate.ToShortDateString());
            if (sourcePathAndFilename.ToLower().EndsWith("jpg") || sourcePathAndFilename.ToLower().EndsWith("jpeg"))
            {
                fileDate = GetExifDateForJpeg(sourcePathAndFilename);
                Logger.logVerbose($"FileDate exif: {fileDate.ToShortDateString()}");
            }
            if (fileDate == DateTime.MinValue)
            {
                throw new InvalidDataException("No Exif Data");
            }
            string newPath = baseDestinationPath + fileDate.Year.ToString("D4") + "\\" + fileDate.Month.ToString("D2") + "\\";
            Logger.logVerbose("newPath: " + newPath);
            return newPath + Path.GetFileName(sourcePathAndFilename);
        }

        private DateTime GetExifDateForJpeg(string sourcePathAndFilename)
        {
            try
            {
                using (ExifReader reader = new ExifReader(sourcePathAndFilename))
                {
                        DateTime dtOfPicture;
                        if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out dtOfPicture))
                        {
                            return dtOfPicture;
                        }
                        return DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }


    }
}
