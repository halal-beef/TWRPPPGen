namespace TWRPPPGen
{
    internal class MakeFile
    {
        /// <summary>
        /// Copy needed files to ./device/{brand}/{codename}/recovery/root/
        /// </summary>
        public static void CopyFiles(string targetFolder)
        {
            //List all .rc files in the extracted Ramdisk root folder.
            string[] dotRCFiles = Directory.GetFiles(Data.PathToAIK + @"\ramdisk\", "*.rc");

            for (int i = 0; i < dotRCFiles.Length; i++)
            {
                string[] part2 = dotRCFiles[i].Split('\\');
                
                File.Copy(dotRCFiles[i], targetFolder + part2.Last(), true);
            }

            if (Directory.Exists(Data.PathToAIK + @"\ramdisk\system\etc"))
            {
                if (File.Exists(Data.PathToAIK + @"\ramdisk\system\etc\ueventd.rc"))
                {
                    File.Copy(Data.PathToAIK + @"\ramdisk\system\etc\ueventd.rc", targetFolder + "\\ueventd.rc", true);
                }
                if (File.Exists(Data.PathToAIK + @"\ramdisk\system\etc\recovery.fstab"))
                {
                    File.Copy(Data.PathToAIK + @"\ramdisk\system\etc\recovery.fstab", targetFolder + "\\etc\\recovery.fstab", true);
                }
            }
        }
    }
}
