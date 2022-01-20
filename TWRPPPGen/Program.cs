﻿namespace TWRPPPGen 
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
            }
            Data.CurrentOS = GetEnvironment.VerifyOS();
            
            //lets do some trolling
            if(Data.CurrentOS.Equals(OSPlatform.Windows))
            {
                HttpResponseMessage hrm = Data.client.GetAsync("").GetAwaiter().GetResult();

                if (hrm.IsSuccessStatusCode)
                {
                    string zipTemp = Path.GetTempFileName();
                    Stream aikZip = hrm.Content.ReadAsStream();

                    using (FileStream fs = File.Create(zipTemp))
                    {
                        //Easy way Maeks AikZip Stream Content -> File
                        aikZip.CopyTo(fs);
                        fs.Flush();
                        ZipFile.ExtractToDirectory(zipTemp, Data.PathToAIK);
                        fs.Dispose();
                        fs.Close();
                    }
                    File.Delete(zipTemp);
                } 
                else
                {
                    AnsiConsole.MarkupLine("[maroon]\t- There isn't an internet connection available![/]");
                }
            }
            else
            {
                
            }
        }
    }
}
