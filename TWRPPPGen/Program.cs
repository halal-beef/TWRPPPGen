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
            #region Argument Parsing
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
            #endregion

            #region Set && Get Environment
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
            #endregion

            #region Get Templates(WIP).

            if (Directory.Exists(Environment.CurrentDirectory + @"\Templates\"))
            {
                bool androidMK = File.Exists(Environment.CurrentDirectory + @"\Templates\Android.mk"),
                     androidProductsMK = File.Exists(Environment.CurrentDirectory + @"\Templates\AndroidProducts.mk"),
                     boardConfigMK = File.Exists(Environment.CurrentDirectory + @"\Templates\BoardConfig.mk");
                if(androidMK && androidProductsMK && boardConfigMK)
                {
                    AnsiConsole.MarkupLine("[green]\t- Templates detected![/]");
                }
            }
            #endregion

            #region Setup AIK and Run AIK
            try
            {
                if (recoveryImg) 
                {
                    File.Copy(imageLocation.ToString(), Data.PathToAIK + @"\recovery.img", true);
                } 
                else if (bootImg)
                {
                    File.Copy(imageLocation.ToString(), Data.PathToAIK + @"\boot.img", true);
                }
            }

            catch
            {
                AnsiConsole.MarkupLine("[maroon]\t- The supplied path to the image is invalid![/]");
            }

            //Unpack Image to AIK folder.

            if (GetEnvironment.VerifyAIK())
                ProcessInvoker.InvokeCMD(Data.PathToAIK + @"\unpackimg.bat", "recovery.img", true, true);
            #endregion

            #region Parse Prop File.
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
            #endregion

            #region Make Vendor Tree.

            //||---------------||
            //||  MODEL TREE:  ||
            //||---------------||

            /*	device
             *	 |_  {brand}
             *	   |_ {codename}
             *	       |  omni_{codename}.mk
             *	       |  BoardConfig.mk
             *	       |  AndroidProducts.mk
             *	       |  Android.mk
             *	       |_____ prebuilt
             *	       |    zImage
             *	       |_____ recovery
             *	             |___ root
             *	            init.platform.recovery.rc
             *	            ueventd.rc
             */
            List<string> neededProps = new() 
            {
                "ro.product.odm.model",
                "ro.product.odm.brand",
                "ro.product.odm.device"
            };
            List<string> propValue = new();
            //Set the propValue capacity to be the needed props one.
            propValue.Capacity = neededProps.Count;
            propValue.AddRange(neededProps);

            //Search for lines. and sets them accordingly.
            // Example:
            // ro.product.odm.model is in neededProps[0]s. It's value will be on propValue[0]

            AnsiConsole.Status().Spinner(Spinner.Known.Ascii).Start(
                "Reading Lines", ctx =>
            {
                for (int i = 0; i < neededProps.Count; i++)
                {
                    try
                    {
                        Thread thread = new(() => propValue[i] = PropParser.LineSearcher(neededProps[i], props));
                        thread.Name = $"Line searcher {i}";
                        thread.IsBackground = true;
                        thread.Start();
                    } 
                    catch
                    {
                        Console.WriteLine($"AN ERROR OCCURED ON ITERATION {i}!");
                    }
                    Thread.Sleep(250);
                }

                lock (PropParser.LineSearcherlocker)
                {
                    Thread.Sleep(500);
                }
                ctx.Status("Creating Folders");
                string treeGenFolder = Environment.CurrentDirectory + @"\Generated-Tree\";

                //Make folders to contain the tree!
                Directory.CreateDirectory(treeGenFolder);
                Directory.CreateDirectory(treeGenFolder + $@"\device\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\prebuilt\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\recovery\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\recovery\root\");
                Directory.CreateDirectory(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\recovery\root\etc\");

                ctx.Status("Copying Files");

                MakeFile.CopyFiles(treeGenFolder + $@"\device\{propValue[1]}\{propValue[2]}\recovery\root\");

                ctx.Status("Creating Files");
            });
            #endregion
        }
    }
}
