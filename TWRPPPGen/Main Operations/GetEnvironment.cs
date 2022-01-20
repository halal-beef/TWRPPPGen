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
        /// <returns>True if AIK Folder, unpackimg.bat and repackimg.bat are present, else false.</returns>
        public static bool VerifyAIK()
        {
            if (!Directory.Exists(Data.PathToAIK))
            {
                return false;
            }
            else if (Directory.Exists(Data.PathToAIK))
            {
                if (File.Exists(Data.PathToAIK + @"\unpackimg.bat") && File.Exists(Data.PathToAIK + @"\repackimg.bat"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
