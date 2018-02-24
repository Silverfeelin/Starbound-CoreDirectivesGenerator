using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CoreDirectivesGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

            if (!Directory.Exists(outputPath))
            {
                try
                {
                    Directory.CreateDirectory(outputPath);
                }
                catch (Exception e)
                {
                    WaitAndExit("Couldn't create output directory. Exception: {0}", e.ToString());
                    return;
                }
            }

            if (args.Length != 0)
            {
                WaitAndExit("Please run the Directives Generator without any arguments.");
                return;
            }

            bool running = true;
            while (running)
            {
                WriteColoredLine(ConsoleColor.Cyan, "= Directives Generator (\"quit\" to exit)");

                // Get first image
                Console.WriteLine("First image path:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string firstPath = Console.ReadLine().Trim();
                Console.ResetColor();

                if (firstPath == "quit" || firstPath == "exit")
                    Environment.Exit(0);

                // Get second image
                Console.WriteLine("Second image path:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string secondPath = Console.ReadLine().Trim();
                Console.ResetColor();

                if (secondPath == "quit" || secondPath == "exit")
                    Environment.Exit(0);

                // Compare
                string directives = Compare(firstPath, secondPath);
                if (directives != null)
                {
                    // Save valid results
                    string file = string.Format("{0}-{1}-{2}.txt",
                        Path.GetFileNameWithoutExtension(firstPath),
                        Path.GetFileNameWithoutExtension(secondPath),
                        DateTime.Now.ToString("HHmmss"));

                    string path = Path.Combine(outputPath, file);

                    Save(path, directives, true);

                    Console.WriteLine("Directives saved and copied to clipboard!");
                    WriteColoredLine(ConsoleColor.DarkGray, Path.GetFullPath(path));
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Writes colored text, while keeping the original ForegroundColor.
        /// </summary>
        /// <param name="fg">Text color.</param>
        /// <param name="text">Text to write.</param>
        /// <param name="args">Format arguments.</param>
        static void WriteColored(ConsoleColor fg, string text, params object[] args)
        {
            ConsoleColor ofg = Console.ForegroundColor;
            Console.ForegroundColor = fg;
            Console.Write(text, args);
            Console.ForegroundColor = ofg;
        }

        /// <summary>
        /// Writes a colored line, while keeping the original ForegroundColor.
        /// </summary>
        /// <param name="fg">Text color.</param>
        /// <param name="text">Text to write.</param>
        /// <param name="args">Format arguments.</param>
        static void WriteColoredLine(ConsoleColor fg, string text, params object[] args)
        {
            WriteColored(fg, text + Environment.NewLine, args);
        }

        /// <summary>
        /// Compares two images, and returns replace directives.
        /// </summary>
        /// <param name="firstPath">First image path.</param>
        /// <param name="secondPath">Second image path.</param>
        /// <returns></returns>
        static string Compare(string firstPath, string secondPath)
        {
            try
            {
                using (FileStream streamA = File.OpenRead(firstPath))
                using (FileStream streamB = File.OpenRead(secondPath))
                using (Image<Rgba32> imageA = Image.Load<Rgba32>(streamA))
                using (Image<Rgba32> imageB = Image.Load<Rgba32>(streamB))
                {
                    if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
                    {
                        Console.WriteLine("Image sizes differ. Please use two images with the same dimensions.");
                        return null;
                    }

                    Dictionary<Rgba32, Rgba32> conversions = new Dictionary<Rgba32, Rgba32>();

                    for (int y = 0; y < imageA.Height; y++)
                    {
                        for (int x = 0; x < imageA.Width; x++)
                        {
                            Rgba32 a = imageA[x, y],
                                b = imageB[x, y];

                            if (a != b)
                            {
                                conversions[a] = b;
                            }
                        }
                    }

                    string directives = "?replace";

                    foreach (var c in conversions)
                    {
                        directives += string.Format(";{0}={1}", ColorToString(c.Key), ColorToString(c.Value));
                    }

                    return directives;
                }
            }
            catch (Exception e)
            {
                WriteColoredLine(ConsoleColor.Red, "Comparing failed! Exception: {0}{1}", Environment.NewLine, e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Converts a color to hex, formatted "RRGGBBAA".
        /// </summary>
        /// <param name="c">Color to convert.</param>
        /// <returns>Hexadecimal color.</returns>
        static string ColorToString(Rgba32 c)
        {
            string r = c.R.ToString("X2"),
                    g = c.G.ToString("X2"),
                    b = c.B.ToString("X2"),
                    a = c.A.ToString("X2");

            return (r + g + b + a).ToLower();
        }

        /// <summary>
        /// Saves text to a file, and optionally copies the text to the clipboard.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="text">Text to copy.</param>
        /// <param name="copy">Copy to clipboard as well?</param>
        static void Save(string fileName, string text, bool copy = true)
        {
            try
            {
                File.WriteAllText(fileName, text);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    $"type {fileName} | clip".Bat();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    $"cat \"{fileName}\" | pbcopy".Bash();
                }
                else
                {
                    WriteColoredLine(ConsoleColor.Red, "Your OS doesn't support copying to clipboard natively!");
                    Console.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                WriteColoredLine(ConsoleColor.Red, "Copying to clipboard failed! Exception: {0}", e.ToString());
            }
        }

        /// <summary>
        /// Closes the application after displaying a message, followed by "Press any key to exit...".
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="args">Message arguments.</param>
        static void WaitAndExit(string message = null, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message, args);
            }

            WriteColoredLine(ConsoleColor.Cyan, "Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
