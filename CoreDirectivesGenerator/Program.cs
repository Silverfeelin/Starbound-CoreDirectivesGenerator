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
            if (args.Length != 0)
            {
                WaitAndExit("Please run the Directives Generator without any arguments.");
                return;
            }

            bool running = true;
            while (running)
            {
                Console.WriteLine("First image path:");
                string firstPath = Console.ReadLine();

                if (firstPath == "quit" || firstPath == "exit")
                {
                    Environment.Exit(0);
                }

                Console.WriteLine("Second image path:");
                string secondPath = Console.ReadLine();

                if (secondPath == "quit" || secondPath == "exit")
                    Environment.Exit(0);
                
                string directives = Compare(firstPath, secondPath);
                if (directives != null)
                {
                    SetClipboard(directives);
                    Console.WriteLine("Compared '{0}' to '{1}'. Directives copied to clipboard.", secondPath, firstPath);
                }
            }

            using (FileStream stream = File.OpenRead(args[0]))
            using (Image<Rgba32> image = Image.Load<Rgba32>(stream))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Rgba32 rgba = image[x, y];
                        Console.WriteLine(rgba.ToString());
                    }
                }
            }
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
                Console.Write("Comparing failed! Exception:" + Environment.NewLine + e.ToString() + Environment.NewLine);
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
        /// Copies text to the clipboard, by first writing it to a temporary file.
        /// </summary>
        /// <param name="text">Text to copy.</param>
        static void SetClipboard(string text)
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, text);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    $"type {tempFile} | clip".Bat();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    $"cat \"{tempFile}\" | pbcopy".Bash();
                }
                else
                {
                    Console.WriteLine("Your OS doesn't support copying to clipboard natively!");
                    Console.WriteLine(text);
                }
            }
            finally
            {
                File.Delete(tempFile);
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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
