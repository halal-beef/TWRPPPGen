namespace TWRPPPGen
{
    internal class GetEnvironment
    {
        /// <summary>
        /// Verifies if the OS is Windows or Linux.
        /// </summary>
        /// <returns>True if the OS is windows. False if it is other.</returns>
        public static bool VerifyOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return true;
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if AIK is available on a directory.
        /// </summary>
        public static void VerifyAIK()
        {
            string currentDir = Environment.CurrentDirectory;
            
            if (!Directory.Exists(currentDir + @"/AIK/"))
            {
                throw new Exception("No AIK Folder.");
            } 
            else if (Directory.Exists(currentDir + @"/AIK/"))
            {
                if (File.Exists(currentDir + @"/AIK/unpackimg.bat") && File.Exists(currentDir + @"/AIK/repackimg.bat"))
                {

                }
            }
        }
    }
}
