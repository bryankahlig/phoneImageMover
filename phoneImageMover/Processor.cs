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

        public void Run(Options options)
        {
            Logger = new Logger(options.Verbose);
            Options = options;
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
                    var indexOfYearInFilename = getIndexOfYearInFilename(source);
                    Logger.logVerbose($"Using filename for date for {source} with year index {indexOfYearInFilename}");
                    destinationFilePath = buildDestinationPathByFilename(source, destinationRoot);
                }
                catch (InvalidDataException ide)
                {
                    Logger.logVerbose($"Unable to find year in filename for {source}");
                }
            }
            return destinationFilePath;
        }

        private string buildDestinationPathByFilename(string fileToMove, string destinationPathRoot)
        {
            int indexOfYearInFilename = getIndexOfYearInFilename(fileToMove);
            if (indexOfYearInFilename < 0) throw new InvalidDataException($"File {fileToMove} does not have a valid year index");
            string fileYear = fileToMove.Substring(indexOfYearInFilename, 4);
            string fileMonth = fileToMove.Substring(indexOfYearInFilename + 4, 2);
            string destinationPath = destinationPathRoot + fileYear + "//" + fileMonth + "//";
            Logger.logVerbose($"Destination path for file {fileToMove} is {destinationPath}");
            return destinationPath;
        }

        private bool ShouldSkipFile(string filename)
        {
            return (filename.StartsWith(".") || filename.ToLower() == "thumbs.db" || getIndexOfYearInFilename(filename) == -1);
        }

        private int getIndexOfYearInFilename(string filename)
        {
            switch (filename.ToUpper().Substring(0, 3))
            {
                case "IMG": return 4;
                case "PHO": return 6;
                case "VID": return filename.IndexOf("_") + 1;
                default:
                    Logger.log("Unrecognized file format: " + filename);
                    return -1;
            }
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

        private DateTime GetDateForMp4Filename(string filename, int indexOfYearInFilename)
        {
            Logger.logVerbose("MP4 Path and Filename: " + filename);
            Logger.logVerbose("MP4 Filename: " + Path.GetFileName(filename));
            string strippedDate = Path.GetFileName(filename).Substring(indexOfYearInFilename, 7);
            Logger.logVerbose("MP4 Stripped Date: " + strippedDate);
            return DateTime.ParseExact(strippedDate, "yyyyMMDD", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
