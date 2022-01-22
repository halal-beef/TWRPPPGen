namespace TWRPPPGen
{
    internal class MakeFile
    {
        /// <summary>
        /// Copy needed files to ./device/{brand}/{codename}
        /// </summary>
        public static void CopyFiles(string targetFolder, string hware)
        {
            if(Data.CurrentOS.Equals(OSPlatform.Windows))
            {
            // List all .rc files in the extracted Ramdisk root folder.
            string[] dotRCFiles = Directory.GetFiles(Data.PathToAIK + @"\ramdisk\", "*.rc");

            for (int i = 0; i < dotRCFiles.Length; i++)
            {
                string[] part2 = dotRCFiles[i].Split('\\');

                // Copy the file if it maches.
                if (part2.Last().Contains($"init.recovery.{hware}")
                 || part2.Last().Contains($"init.recovery.{hware}")
                 || part2.Last().Contains($"ueventd.{hware}")
                 || part2.Last().Contains($"ueventd.{hware}"))
                {
                    File.Copy(dotRCFiles[i], targetFolder + @"\recovery\root\" + part2.Last(), true);
                }
            }

            // NO SAR
            if (Directory.Exists(Data.PathToAIK + @"\ramdisk\system\"))
            {
                if (Directory.Exists(Data.PathToAIK + @"\ramdisk\system\etc"))
                {
                    if (File.Exists(Data.PathToAIK + @"\ramdisk\system\etc\recovery.fstab"))
                    {
                        File.Copy(Data.PathToAIK + @"\ramdisk\system\etc\recovery.fstab", targetFolder + @"\recovery\root\system\etc\recovery.fstab", true);
                    }
                }
            } 
            // SAR
            else if (Directory.Exists(Data.PathToAIK + @"\ramdisk\etc\"))
            {
                if (File.Exists(Data.PathToAIK + @"\ramdisk\etc\recovery.fstab"))
                {
                    File.Copy(Data.PathToAIK + @"\ramdisk\etc\recovery.fstab", targetFolder + @"\recovery\root\etc\recovery.fstab", true);
                }
            }

            // Copy kernel -> zImage
            if (File.Exists(Data.PathToAIK + @"\split_img\boot.img-kernel"))
            {
                File.Copy(Data.PathToAIK + @"\split_img\boot.img-kernel", targetFolder + @"\prebuilt\zImage", true);
            }
            else if (File.Exists(Data.PathToAIK + @"\split_img\recovery.img-kernel"))
            {
                File.Copy(Data.PathToAIK + @"\split_img\recovery.img-kernel", targetFolder + @"\prebuilt\zImage", true);
            }
          }
            else if(Data.CurrentOS.Equals(OSPlatform.Linux))
            {
                            // List all .rc files in the extracted Ramdisk root folder.
                //disable permissions
                ProcessStartInfo deitz = new();
                deitz.FileName = "sudo";
                deitz.Arguments = $" chmod ugo+rwx "'{Environment.CurrentDirectory}/Android Image Kitchen/ramdisk/*'"}";
                Process.Start(deitz);
            string[] dotRCFiles = Directory.GetFiles(Environment.CurrentDirectory + @"/Android Image Kitchen" + @"/ramdisk/", "*.rc");

            for (int i = 0; i < dotRCFiles.Length; i++)
            {
                string[] part2 = dotRCFiles[i].Split(@"//");

                // Copy the file if it maches.
                if (part2.Last().Contains($"init.recovery.{hware}")
                 || part2.Last().Contains($"init.recovery.{hware}")
                 || part2.Last().Contains($"ueventd.{hware}")
                 || part2.Last().Contains($"ueventd.{hware}"))
                {
                    File.Copy(dotRCFiles[i], targetFolder + @"/recovery/root/" + part2.Last(), true);
                }
            }

            // NO SAR
            if (Directory.Exists(Data.PathToAIK + @"/ramdisk/system/"))
            {
                if (Directory.Exists(Data.PathToAIK + @"/ramdisk/system/etc"))
                {
                    if (File.Exists(Data.PathToAIK + @"/ramdisk/system/etc/recovery.fstab"))
                    {
                        File.Copy(Data.PathToAIK + @"/ramdisk/system/etc/recovery.fstab", targetFolder + @"/recovery/root/system/etc/recovery.fstab", true);
                    }
                }
            } 
            // SAR
            else if (Directory.Exists(Data.PathToAIK + @"/ramdisk/etc/"))
            {
                if (File.Exists(Data.PathToAIK + @"/ramdisk/etc/recovery.fstab"))
                {
                    File.Copy(Data.PathToAIK + @"/ramdisk/etc/recovery.fstab", targetFolder + @"/recovery/root/etc/recovery.fstab", true);
                }
            }

            // Copy kernel -> zImage
            if (File.Exists(Data.PathToAIK + @"/split_img/boot.img-kernel"))
            {
                File.Copy(Data.PathToAIK + @"/split_img/boot.img-kernel", targetFolder + @"/prebuilt/zImage", true);
            }
            else if (File.Exists(Data.PathToAIK + @"/split_img/recovery.img-kernel"))
            {
                File.Copy(Data.PathToAIK + @"/split_img/recovery.img-kernel", targetFolder + @"/prebuilt/zImage", true);
            }
            }
        }
    }
}
