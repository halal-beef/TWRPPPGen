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
            bool recoveryImg = false, 
                 bootImg = false;
            //Parse args.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().Contains("--recovery_image"))
                {
                    recoveryImg = true;
                } 
                else if (args[i].ToLower().Contains("--boot_image"))
                {
                    bootImg = true;
                }
            }
            if (recoveryImg)
            {

            }
        }
    }
}