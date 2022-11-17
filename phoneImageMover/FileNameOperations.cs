﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phoneImageMover
{
    public class FileNameOperations
    {
        private Logger logger;

        public FileNameOperations(Logger l)
        {
            this.logger = l;
        }

        public string buildDestinationPathByFilename(string fileToMove, string destinationPathRoot)
        {
            int indexOfYearInFilename = getIndexOfYearInFilename(fileToMove);
            if (indexOfYearInFilename < 0) throw new InvalidDataException($"File {fileToMove} does not have a valid year index");
            string fileYear = fileToMove.Substring(indexOfYearInFilename, 4);
            string fileMonth = fileToMove.Substring(indexOfYearInFilename + 4, 2);
            string destinationPath = destinationPathRoot + fileYear + "\\" + fileMonth + "\\";
            logger.logVerbose($"Destination path for file {fileToMove} is {destinationPath}");
            return destinationPath;
        }

        public int getIndexOfYearInFilename(string filename)
        {
            switch (filename.ToUpper().Substring(0, 3))
            {
                case "IMG": return 4;
                case "PHO": return 6;
                case "VID": return filename.IndexOf("_") + 1;
                default:
                    logger.log("Unrecognized file format: " + filename);
                    return -1;
            }
        }

        public DateTime GetDateForMp4Filename(string filename, int indexOfYearInFilename)
        {
            logger.logVerbose("MP4 Path and Filename: " + filename);
            logger.logVerbose("MP4 Filename: " + Path.GetFileName(filename));
            string strippedDate = Path.GetFileName(filename).Substring(indexOfYearInFilename, 8);
            logger.logVerbose("MP4 Stripped Date: " + strippedDate);
            return DateTime.ParseExact(strippedDate, "yyyyMMDD", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
