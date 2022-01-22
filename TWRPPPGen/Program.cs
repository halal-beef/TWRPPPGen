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
            PrepareEnvironment.GetAIK();
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
            PrepareEnvironment.CopyImage(imageLocation, recoveryImg, bootImg);
            #endregion

            #region Parse Prop File.

            List<string> props = PrepareEnvironment.GetPropList();

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
                "ro.product.vendor.model",
                "ro.product.vendor.brand",
                "ro.product.vendor.device",
                "ro.board.platform",
                "ro.bionic.arch"
            };
            List<string> propValues = new();
            propValues.AddRange(neededProps);

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
                        Thread thread = new(() => propValues[i] = PropParser.LineSearcher(neededProps[i], props));
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

                string generatedTreeFolder = $"{Environment.CurrentDirectory}\\Generated-Tree\\";
                string treeFolder = generatedTreeFolder + $"\\device\\{propValues[1]}\\{propValues[2]}\\";
                
                //List of folders that should be created
                List<string> neededFolders = new()
                {
                    generatedTreeFolder,
                    treeFolder,
                    $"{treeFolder}\\prebuilt\\",
                    $"{treeFolder}\\recovery\\root\\"
                };

                //Make folders to contain the tree!
                PrepareEnvironment.CreateFolders(neededFolders);

                if (Directory.Exists(Data.PathToAIK + @"\ramdisk\system"))
                {
                    //Image is NOT SAR
                    Directory.CreateDirectory($"{treeFolder}\\recovery\\root\\system\\");
                    Directory.CreateDirectory($"{treeFolder}\\recovery\\root\\system\\etc\\");
                }
                else
                {
                    //Image is SAR
                    Directory.CreateDirectory($"{treeFolder}\\recovery\\root\\etc\\");
                }

                //Get SoC ABIs.
                string SoCABIList = PropParser.LineSearcher("ro.product.cpu.abilist", props);

                string[] arch = SoCABIList.Split('-');

                //If it failed to get arch normally with the prop ro.bionic.arch it will use the first value from the abi list
                if (propValues[4] == "Prop Not Found.")
                {
                    propValues[4] = arch[0];
                }

                //Check if MTK or QCOM
                if (!propValues[3].Contains("mt"))
                {
                    if(PropParser.LineSearcher("ro.hardware.wlan.vendor", props) == "qcom")
                    {
                        propValues[3] = "qcom";
                    }
                }


                ctx.Status("Copying Files");

                MakeFile.CopyFiles(treeFolder, propValues[3]);

                ctx.Status("Creating Files");

                if (propValues[4].Equals("arm64")) 
                {
                    TemplateParser.SetupTemplate(SoCABIList, props, true);
                } 
                else if (propValues[4].Equals("arm"))
                {
                    TemplateParser.SetupTemplate(SoCABIList, props, false);
                }
            });
            #endregion
        }
    }
}
