using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game2DFramework
{
    public static class Log
    {
        private const string LogFileName = "log.txt";

        public static void Write(string data)
        {
            using (TextWriter writer = new StreamWriter(LogFileName, true))
            {
                writer.WriteLine("{0}: {1}", DateTime.Now.ToString("G"), data);
            }
        }

        public static void WriteException(Exception ex)
        {
            Write(string.Format("Exception occured: {0}\n\tStacktrace: {1}", ex.Message, ex.StackTrace));
        }
    }
}
