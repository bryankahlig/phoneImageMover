using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace phoneImageMover
{
    public class FilenameValidator : IFilenameValidator
    {
        public bool IsValidFilenameMonth(string month)
        {
            return Regex.IsMatch(month, "^(0[1-9]|1[0-2])$");
        }

        public bool IsValidFilenameYear(string year)
        {
            return Regex.Match(year, "^(19|20)\\d{2}$").Success;
        }
    }
}
