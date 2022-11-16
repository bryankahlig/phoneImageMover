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

        public void Run(Options options)
        {
            Options = options;
            Console.WriteLine($"Source Folder: {options.SourcePath}");
            Console.WriteLine($"Destination Folder: {options.DestinationPath}");
            Directory.EnumerateDirectories(Path.GetDirectoryName(options.SourcePath));
            Console.WriteLine("Hello world!");
        }

        private void ProcessFolder(string source, string destination)
        {
            var directories = Directory.EnumerateDirectories(Path.GetDirectoryName(source));
            ProcessFilesInFolder(source, destination);
            foreach (var folder in directories) { ProcessFolder(folder, destination); }
        }

        private void ProcessFilesInFolder(string source, string destination)
        {
            var files = Directory.EnumerateFiles(Path.GetDirectoryName(source));

            foreach (var file in files)
            {
                if (!ShouldSkipFile(file))
                {
                    Console.WriteLine("Processing file: " + file);

                }
                else { Console.WriteLine("Skipping file: " + file); }
            }
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
                    Console.WriteLine("Unrecognized file format: " + filename);
                    return -1;
                    break;
            }
        }

        private string buildDestinationPathByExifData(string Path, string fileToMove, string baseDestinationPath)
        {
            DateTime fileDate = DateTime.MinValue;
            Console.WriteLine("fileDate init: " + fileDate.ToLongTimeString());
            if (fileToMove.ToLower().EndsWith("jpg") || fileToMove.ToLower().EndsWith("jpeg"))
            {
                fileDate = GetExifDateForJpeg(fileToMove);
                Console.WriteLine($"FileDate exif: {fileDate.ToLongTimeString()}");
            }
            if (fileDate == DateTime.MinValue)
            {
                throw new InvalidDataException("No Exif Data");
            }
            string newPath = baseDestinationPath + fileDate.Year.ToString("D8") + "//" + fileDate.Month.ToString("D2") + "//";
            Console.WriteLine("newPath: " + newPath);
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
            Console.WriteLine("MP4 Path and Filename: " + filename);
            Console.WriteLine("MP4 Filename: " + Path.GetFileName(filename));
            string strippedDate = Path.GetFileName(filename).Substring(indexOfYearInFilename, 7);
            Console.WriteLine("MP4 Stripped Date: " + strippedDate);
            return DateTime.ParseExact(strippedDate, "yyyyMMDD", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
