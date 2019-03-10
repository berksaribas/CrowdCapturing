using System;
using System.IO;
using System.Text;

namespace Util
{
    public class FileReader
    {
        public static String Load(string fileName)
        {
            string result = "";
            try
            {
                string line;
                // Create a new StreamReader, tell it which file to read and what encoding the file
                // was saved as
                StreamReader theReader = new StreamReader(fileName, Encoding.Default);
                // Immediately clean up the reader after this block of code is done.
                // You generally use the "using" statement for potentially memory-intensive objects
                // instead of relying on garbage collection.
                // (Do not confuse this with the using directive for namespace at the 
                // beginning of a class!)
                using (theReader)
                {
                    // While there's lines left in the text file, do this:
                    do
                    {
                        line = theReader.ReadLine();
                     
                        if (line != null)
                        {
                            result += line;
                        }
                    }
                    while (line != null);
                    // Done reading, close the reader and return true to broadcast success    
                    theReader.Close();
                }
            }
            // If anything broke in the try block, we throw an exception with information
            // on what didn't work
            catch (Exception e)
            {
                Console.WriteLine("{0}\n", e.Message);
            }

            return result;
        }
    }
}