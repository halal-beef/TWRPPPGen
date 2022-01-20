namespace TWRPPPGen
{
    internal class GetEnvironment
    {
        /// <summary>
        /// Verifies if the OS is Windows or Linux.
        /// </summary>
        /// <returns>OSPlatform struct containing OS type.</returns>
        public static OSPlatform VerifyOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            } 
            else
            {
                return OSPlatform.FreeBSD;
            }
        }
        /// <summary>
        /// Checks if AIK is available on a directory.
        /// </summary>
        public static void VerifyAIK()
        {
            
            if (!Directory.Exists(Data.PathToAIK))
            {
                throw new Exception("No AIK Folder.");
            } 
            else if (Directory.Exists(Data.PathToAIK))
            {
                if (File.Exists(Environment.CurrentDirectory + @"/AIK/unpackimg.bat") && File.Exists(Environment.CurrentDirectory + @"/AIK/repackimg.bat"))
                {

                }
            }
        }
    }
}
