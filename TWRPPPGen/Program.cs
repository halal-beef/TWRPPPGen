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
            if(!recoveryImg && !bootImg || imageLocation.ToString() == "")
            {
                AnsiConsole.MarkupLine("[maroon]\t- You didn't indicate a recovery image or boot image to work with![/]");
                Environment.Exit(-1);
            }
            Data.CurrentOS = GetEnvironment.VerifyOS();
            
            //lets do some trolling
            if(Data.CurrentOS.Equals(OSPlatform.Windows) && !GetEnvironment.VerifyAIK())
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
                        ded=true;
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
            else
            {
                
            }
        }
    }
}
