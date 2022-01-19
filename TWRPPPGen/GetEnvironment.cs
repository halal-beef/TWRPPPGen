namespace TWRPPPGen
{
    internal class GetEnvironment
    {
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
