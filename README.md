# Starbound Core Directives Generator

A .NET Core port of [Silverfeelin/Starbound-DirectiveGenerator](https://github.com/Silverfeelin/Starbound-DirectiveGenerator/).

This tool is used to compare two image files to generate `?replace` directives for use in Starbound. This allows you to work with your preferred image editing software.

## Notes
* Images should be recolored using external software. Some great applications are [Paint.NET](http://www.getpaint.net/index.html) and [GIMP](https://www.gimp.org/).
* A color should only ever be replaced by one other color.
 * In Paint.NET, I recommend the below settings for the Paint Bucket tool.

 ![](https://raw.githubusercontent.com/Silverfeelin/Starbound-DirectiveGenerator/master/readme/pdn-fill.png)

## Installation

On the release page, make sure you download the zip for your platform (i.e. `win-x86`).

### Windows

* Unpack the zip.

You can now run the application simply by opening `CoreDirectivesGenerator.exe`.

### Mac & Linux

* Unpack the zip.
* Open the terminal and navigate to the folder.
* Mark the application as executable with `chmod +x CoreDirectivesGenerator`.

You can now run the application by entering `./CoreDirectivesGenerator` in your terminal (make sure you open your terminal from inside the application folder).

## ImageSharp

This project is powered by ImageSharp, a cross-platform library for working with images:

https://github.com/SixLabors/ImageSharp

ImageSharp is bundled with the releases for every platform, and is licensed under the Apache 2.0 License (which you can find [here](https://github.com/SixLabors/ImageSharp/blob/master/LICENSE)).
