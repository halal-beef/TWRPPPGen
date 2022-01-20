namespace TWRPPPGen 
{
    public class Program 
    {
        /// <summary>
        /// Program's Main Entry Point!
        /// </summary>
        /// <param name="args">Arguments given at startup.</param>
        public static void Main(string[] args)
        {
            StringBuilder imageLocation = new();
            bool recoveryImg = false,
                 bootImg = false;
            //Parse args.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().Contains("--recovery_image"))
                {
                    imageLocation.Append(args[i + 1]);
                    recoveryImg = true;
                    break;
                }
                else if (args[i].ToLower().Contains("--boot_image"))
                {
                    imageLocation.Append(args[i + 1]);
                    bootImg = true;
                    break;
                }
            }

            //Both parameters were false...
            if (!recoveryImg && !bootImg || imageLocation.ToString() == "")
            {
                AnsiConsole.MarkupLine("[maroon]\t- You didn't indicate a recovery image or boot image to work with![/]");
                Environment.Exit(-1);
            }

            Data.CurrentOS = GetEnvironment.VerifyOS();

            if (Data.CurrentOS.Equals(OSPlatform.Windows) && !GetEnvironment.VerifyAIK())
            {
                bool internet = true;
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Ascii)
                    .Start("Downloading [red]AIK[/]",
                ctx =>
                {
                    bool ded = false;
                    HttpResponseMessage hrm = new();
                    try
                    {
                        hrm = Data.client.GetAsync("https://forum.xda-developers.com/attachments/android-image-kitchen-v3-8-win32-zip.5300919/").GetAwaiter().GetResult();
                    }
                    catch
                    {
                        ded = true;
                    }

                    if (!ded && hrm.IsSuccessStatusCode)
                    {
                        string zipTemp = Path.GetTempFileName();
                        Stream aikZip = hrm.Content.ReadAsStream();

                        using (FileStream fs = File.Create(zipTemp))
                        {
                            //Easy way Maeks AikZip Stream Content -> File
                            aikZip.CopyTo(fs);
                            fs.Flush();
                            fs.Dispose();
                            fs.Close();
                        }
                        ctx.Status("Unpacking [red]AIK[/]");
                        ZipFile.ExtractToDirectory(zipTemp, Environment.CurrentDirectory);
                        ctx.Status("Deleting temporary files");
                        File.Delete(zipTemp);
                        ctx.Status("Starting [lime]AIK[/]");
                    }
                    else
                    {
                        internet = false;
                    }

                });

                if (!internet)
                {
                    AnsiConsole.MarkupLine(
                            "[maroon]" +
                            "\t- There isn't an internet connection available!\n" +
                           $"\t- If you have an AIK zip with all it's dependencies and it's scripts, unzip it into \"{Data.PathToAIK}\"" +
                            "[/]");

                    Thread.Sleep(5 * 1000);
                    Environment.Exit(0);
                }
            }

            try
            {
                File.Copy(imageLocation.ToString(), Data.PathToAIK + @"\recovery.img", true);
            }
            catch
            {
                AnsiConsole.MarkupLine("[maroon]\t- The supplied path to the image is invalid![/]");
            }

            //Unpack Image to AIK folder.
            if (GetEnvironment.VerifyAIK())
                ProcessInvoker.InvokeCMD(Data.PathToAIK + @"\unpackimg.bat", "recovery.img", true, true);

            List<string> vals = new();
            bool foundProp = true;
            AnsiConsole.Status()
                    .Spinner(Spinner.Known.Ascii)
                    .Start("Reading files",
                ctx =>
                {
                    ctx.Status($"Parsing props...");
                    //Get prop.default/default.prop and parse it
                    if (File.Exists(Data.PathToAIK + @"\ramdisk\prop.default"))
                    {
                        vals = PropParser.ParseFile(Data.PathToAIK + @"\ramdisk\prop.default");
                    }
                    else if (File.Exists(Data.PathToAIK + @"\ramdisk\default.prop"))
                    {
                        vals = PropParser.ParseFile(Data.PathToAIK + @"\ramdisk\default.prop");
                    }
                    else
                    {
                        foundProp = false;
                    }
                });
            if (foundProp)
            {
                AnsiConsole.MarkupLine(
                    "[maroon]\t- Was the image supplied a A Only device boot image?" +
                    "\t- If so, try fetching the recovery.img for your device");
                Thread.Sleep(5 * 1000);
                Environment.Exit(0);
            }
        }
    }
}
