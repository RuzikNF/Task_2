using System;
using System.IO;
using System.Security;
using System.Text;

class Program
{
    private const string FILE_PATH = @"../../../Numbers.bin";
    private const int LOWER_LIMIT = 1;
    private const int UPPER_LIMIT = 100;
    
    private static FileStream fs;
    private static BinaryReader reader = null;
    private static BinaryWriter writer = null;
    
    /// <summary>
    /// Opens the stream and binary readers/writers.
    /// </summary>
    /// <returns>Whether the operation succeeded.</returns>
    static bool OpenStream()
    {
        try
        {
            fs = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
            reader = new BinaryReader(fs, Encoding.UTF8);
            writer = new BinaryWriter(fs, Encoding.UTF8);
            return true;
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

        return false;
    }
    
    /// <summary>
    /// Closes all streams.
    /// </summary>
    static void CloseStream()
    {
        try
        {
            writer?.Flush();
            fs?.Flush();
            
            writer?.Close();
            fs?.Close();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"There was an error closing streams: {e.Message}");
        }
    }

    /// <summary>
    /// Gives a prompt to modify numbers in the file.
    /// </summary>
    static void ModifyNumbers()
    {
        Console.Out.WriteLine();
        Console.Out.Write("Do you want to edit the numbers? [Y/n] ");
        if (Console.ReadLine().ToLower() == "n")
            return;
        
        fs.Seek(0, SeekOrigin.Begin);
        
        for(int i = 1;;i++)
            try
            {
                int value = reader.ReadInt32();
                Console.Out.WriteLine($"Element #{i}: {value}");
                int newValue = value;
                while (true)
                {
                    Console.Out.Write("New value (empty for unmodified): ");
                    string input = Console.ReadLine();
                    double newFloat = value;
                    // Input processing.
                    if (input.Trim() != "" && !double.TryParse(input, out newFloat))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Out.WriteLine("The given value was not an Int32.");
                        Console.ResetColor();
                    }
                    else
                    {
                        int approxValue = (int)Math.Round(newFloat);
                        if (LOWER_LIMIT <= approxValue && approxValue <= UPPER_LIMIT)
                            newValue = approxValue;
                        break;
                    }
                }

                // In order to rewrite the number we need to seek a little earlier.
                fs.Seek(-4, SeekOrigin.Current);
                writer.Write(newValue);
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine();
                break;
            }
            catch (IOException)
            {
                Console.Error.WriteLine("An I/O error occurred.");
                return;
            }

        writer.Flush();

    }
    
    /// <summary>
    /// Displays the current state of the file.
    /// </summary>
    /// <param name="areNew">Whether the contents are new.</param>
    static void DisplayNumbers(bool areNew = false)
    {
        fs.Seek(0, SeekOrigin.Begin);
        
        if(areNew)
            Console.Out.WriteLine("New numbers:");
        else
            Console.Out.WriteLine($"Current numbers in \"{FILE_PATH}\":");
        
        while (true)
            try
            {
                int num = reader.ReadInt32();
                Console.Out.Write($"{num} ");
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine();
                break;
            }
            catch (IOException)
            {
                Console.Error.WriteLine("An I/O error occurred.");
                return;
            }
    }

    /// <summary>
    /// Entry point.
    /// </summary>
    static void Main()
    {
        do
        {
            Console.Clear();
            if (OpenStream())
            {
                DisplayNumbers();
                ModifyNumbers();
                DisplayNumbers(true);
            }
            CloseStream();
            Console.Write("Press ENTER to repeat");
        } while (Console.ReadKey(true).Key == ConsoleKey.Enter);
    }
}