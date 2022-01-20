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
                AnsiConsole.MarkupLine("[maroon]\tYou didn't indicate a recovery image or boot image to work with![/]");
            }
            Data.IsOSWindows = GetEnvironment.VerifyOS();
            
            //lets do some trolling
            if(Data.IsOSWindows == true)
            {
                //new System.Net.WebClient().DownloadFile("https://github.com/osm0sis/Android-Image-Kitchen/archive/refs/heads/master.zip","AIK.zip");
                
            }
            else
            {
                
            }
        }
    }
}
