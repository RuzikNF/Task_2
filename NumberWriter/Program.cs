using System;
using System.IO;
using System.Security;
using System.Text;

class Program
{
    private const string FILE_PATH = @"../../../Numbers.bin";
    private const int LOWER_LIMIT = 1;
    private const int UPPER_LIMIT = 100;
    private const uint TOTAL_NUMS = 10;
    static Random random = new Random();
    static void Main()
    {
        try
        {
            FileStream fs = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8);
            for (int i = 0; i < TOTAL_NUMS; i++)
                writer.Write(random.Next(LOWER_LIMIT, UPPER_LIMIT + 1));
            writer.Flush();

            Console.Out.WriteLine($"Successfully wrote {writer.BaseStream.Length} bytes in file \"{FILE_PATH}\".");

            writer.Close();
            fs.Close();
        }
        catch (IOException)
        {
            Console.Error.WriteLine("io: Unable to create file stream due to an i/o error, exiting...");
        }
        catch (SecurityException)
        {
            Console.Error.WriteLine("security: Not enough permissions to open stream on this file, exiting...");
        }
        catch (ArgumentException)
        {
            Console.Error.WriteLine("env: Output file path given incorrectly, exiting...");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"error: {e.Message}");
        }
    }
}