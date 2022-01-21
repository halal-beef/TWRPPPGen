namespace TWRPPPGen
{
    internal class MakeFile
    {
        /// <summary>
        /// Copy .rc files to ./device/{brand}/{codename}/recovery/root/
        /// </summary>
        public static void CopyFiles(string targetFolder)
        {
            string[] dotRCFiles = Directory.GetFiles(Data.PathToAIK + @"\ramdisk\", "*.rc");

            for (int i = 0; i < dotRCFiles.Length; i++)
            {
                string[] part2 = dotRCFiles[i].Split('\\');
                
                File.Copy(dotRCFiles[i], targetFolder + part2.Last(), true);
            }
        }
    }
}
