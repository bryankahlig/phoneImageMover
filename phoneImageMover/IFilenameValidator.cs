using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phoneImageMover
{
    public interface IFilenameValidator
    {
        bool IsValidFilenameYear(string year);

        bool IsValidFilenameMonth(string month);
    }
}
