using System;
using System.IO;
using System.Text;

namespace Util
{
    public class FileReader
    {
        public static String Load(string fileName)
        {
            var result = new StringBuilder();
            try
            {
                string line;
                // Create a new StreamReader, tell it which file to read and what encoding the file
                // was saved as
                using (var theReader = new StreamReader(fileName, Encoding.Default))
                {
                    // While there's lines left in the text file, do this:
                    do
                    {
                        line = theReader.ReadLine();
                     
                        if (line != null)
                        {
                            result.Append(line);
                        }
                    }
                    while (line != null);
                }
            }
            // If anything broke in the try block, we throw an exception with information
            // on what didn't work
            catch (Exception e)
            {
                Console.WriteLine("{0}\n", e.Message);
            }

            return result.ToString();
        }
    }
}