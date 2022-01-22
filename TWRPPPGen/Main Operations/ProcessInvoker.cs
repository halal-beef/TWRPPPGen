namespace TWRPPPGen
{
    internal struct CmdOut
    {
        /// <summary>
        /// Redirected Stardard Output
        /// </summary>
        public StringBuilder CmdOutPut { get; set; } = new();
        /// <summary>
        /// The program's exit code
        /// </summary>
        public int ExitCode { get; set; }
    }
    internal class ProcessInvoker
    {
        /// <summary>
        /// Invoke a System Shell and use it to run a Batch script, application or anything that can be executed! (Made to run AIK.)
        /// </summary>
        /// <param name="PathToFile">Path to the batch or executable</param>
        /// <param name="shouldRedirectOutput">should redirect Normal output? </param>
        /// <returns>CmdOut containing result data.</returns>
        public static CmdOut InvokeCMD(string PathToFile, string arguments, bool shouldRedirectOutput, bool isAIK = false)
        {
            CmdOut cOut = new();
            Process proc = new();
            ProcessStartInfo sinfo = new();

            sinfo.FileName = PathToFile;
            sinfo.Arguments = arguments;
            sinfo.WindowStyle = ProcessWindowStyle.Hidden;
            sinfo.CreateNoWindow = true;
            sinfo.UseShellExecute = false;
            
            if (shouldRedirectOutput)
                sinfo.RedirectStandardOutput = true;

            Thread x = new(
            () => {
                StreamReader sr = proc.StandardOutput;
                while (true)
                {
                    //Avoid pointing to something null.
                    string a = ". ";
                    a += sr.ReadLine();
                    if (a.Contains("Done"))
                    {
                        Thread.Sleep(1000);
                        proc.Kill();
                        break;
                    }
                }
            });

            proc.StartInfo = sinfo;
            proc.Start();

            //Wait some time...
            Thread.Sleep(2 * 1000);

            if (isAIK)
            {
                x.Start();
            }

            proc.WaitForExit();

            if (shouldRedirectOutput)
                cOut.CmdOutPut.Append(proc.StandardOutput);

            cOut.ExitCode = proc.ExitCode;

            return cOut;
        }
    }
}
