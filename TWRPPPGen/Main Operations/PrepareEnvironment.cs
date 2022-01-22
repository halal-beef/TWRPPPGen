namespace TWRPPPGen
{
    internal class PrepareEnvironment
    {
        /// <summary>
        /// Creates folders to the paths specified.
        /// </summary>
        /// <param name="folders">Paths to create.</param>
        public static void CreateFolders(List<string> folders)
        {
            for (int i = 0; i < folders.Count; i++)
            {
                Directory.CreateDirectory(folders[i]);
            }
        }
        /// <summary>
        /// Gets AIK from the internet and saves it to it's location!
        /// </summary>
        public static void GetAIK()
        {
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
            else if(Data.CurrentOS.Equals(OSPlatform.Linux)) //idc about signing i am just testing
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
                        hrm = Data.client.GetAsync("https://github.com/TWRPPGen/AIK-Linux/raw/main/AIK.zip").GetAwaiter().GetResult();
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
        }
        /// <summary>
        /// Copy the recovery image or boot image
        /// </summary>
        /// <param name="imageLocation">The image location.</param>
        /// <param name="recoveryImg">Is it a recovery image?</param>
        /// <param name="bootImg">Is it a boot image?</param>
        public static void CopyImage(StringBuilder imageLocation, bool recoveryImg, bool bootImg)
        {
            //try
            //{
                //if (GetEnvironment.VerifyAIK())
                //{
                if(Data.CurrentOS.Equals(OSPlatform.Linux))
                {
                    if (recoveryImg)
                    {
                        File.Copy(imageLocation.ToString(), Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/recovery.img", true);
                        ProcessInvoker.InvokeCMD(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/unpackimg.sh", "recovery.img", true, true);
                    }
                    else if (bootImg)
                    {
                        File.Copy(imageLocation.ToString(), Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/boot.img", true);
                        ProcessInvoker.InvokeCMD(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/unpackimg.sh", "boot.img", true, true);
                    }
                }
                else if(Data.CurrentOS.Equals(OSPlatform.Windows))
                {
                    if (recoveryImg)
                    {
                        File.Copy(imageLocation.ToString(), Data.PathToAIK + @"\recovery.img", true);
                        ProcessInvoker.InvokeCMD(Data.PathToAIK + @"\unpackimg.bat", "recovery.img", true, true);
                    }
                    else if (bootImg)
                    {
                        File.Copy(imageLocation.ToString(), Data.PathToAIK + @"\boot.img", true);
                        ProcessInvoker.InvokeCMD(Data.PathToAIK + @"\unpackimg.bat", "boot.img", true, true);
                    }
                }
                //}
            //}
            //catch
            //{
             //   AnsiConsole.MarkupLine("[maroon]\t- The supplied path to the image is invalid![/]");
            //}
        }
        /// <summary>
        /// This class shows progress, and gets the list that contains all props extracted from prop.default
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPropList()
        {
            if(Data.CurrentOS.Equals(OSPlatform.Windows))
            {
            List<string> props = new();
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
                        props = PropParser.ParseFile(Data.PathToAIK + @"\ramdisk\prop.default");
                    }
                    else if (File.Exists(Data.PathToAIK + @"\ramdisk\default.prop"))
                    {
                        props = PropParser.ParseFile(Data.PathToAIK + @"\ramdisk\default.prop");
                    }
                    else
                    {
                        foundProp = false;
                    }
                });
            if (!foundProp)
            {
                AnsiConsole.MarkupLine(
                    "[maroon]\t- Was the image supplied a A Only device boot image?" +
                    "\t- If so, try fetching the recovery.img for your device");
                Thread.Sleep(5 * 1000);
                Environment.Exit(0);
            }
            return props;
            }
            else if(Data.CurrentOS.Equals(OSPlatform.Linux))
            {
                List<string> props = new();
            bool foundProp = true;

            AnsiConsole.Status()
                    .Spinner(Spinner.Known.Ascii)
                    .Start("Reading files",
                ctx =>
                {
                    ctx.Status($"Parsing props...");

                    //Get prop.default/default.prop and parse it
                    if (File.Exists(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/ramdisk/prop.default"))
                    {
                        props = PropParser.ParseFile(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/ramdisk/prop.default");
                    }
                    else if (File.Exists(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/ramdisk/default.prop"))
                    {
                        props = PropParser.ParseFile(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/ramdisk/default.prop");
                    }
                    else
                    {
                        foundProp = false;
                    }
                });
            if (!foundProp)
            {
                AnsiConsole.MarkupLine(
                    "[maroon]\t- Was the image supplied a A Only device boot image?" +
                    "\t- If so, try fetching the recovery.img for your device");
                Thread.Sleep(5 * 1000);
                Environment.Exit(0);
            }
            return props;
            }
            return props;
        }
    }
}
