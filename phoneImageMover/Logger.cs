using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace phoneImageMover
{
    internal class Logger
    {
        private bool verbose;

        public Logger(bool verbose)
        {
            this.verbose = verbose;
        }

        public void log(string msg)
        {
            Console.WriteLine(msg);
        }

        public void logVerbose(string msg)
        {
            if (verbose) Console.WriteLine(msg);
        }
    }
}
