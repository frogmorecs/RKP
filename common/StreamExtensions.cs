using System;
using System.IO;
using System.Linq;
using System.Text;

namespace common
{
    public static class StreamExtensions
    {
        public static void WriteASCII(this Stream stream, string text)
        {
            if (text.Any(c => c > 127))
            {
                throw new ApplicationException("Only ASCII characters allowed in printer names.");    
            }

            var data = Encoding.ASCII.GetBytes(text);
            stream.Write(data, 0, data.Length);
        }
    }
}