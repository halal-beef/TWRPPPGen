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
        /// Invoke a System Shell and use it to run a Batch script, application or anything that can be executed!
        /// </summary>
        /// <param name="PathToFile">Path to the batch or executable</param>
        /// <param name="shouldRedirectOutput">should redirect Normal output? </param>
        /// <returns>CmdOut containing result data.</returns>
        public static CmdOut InvokeCMD(string PathToFile, string arguments, bool shouldRedirectOutput)
        {
            CmdOut cOut = new();
            Process proc = new();
            ProcessStartInfo sinfo = new();

            sinfo.FileName = PathToFile;
            sinfo.Arguments = arguments;
            sinfo.WindowStyle = ProcessWindowStyle.Hidden;
            sinfo.CreateNoWindow = true;
            sinfo.UseShellExecute = true;
            
            if(shouldRedirectOutput)
                sinfo.RedirectStandardOutput = true;

            proc.StartInfo = sinfo;
            proc.Start();
            proc.WaitForExit();

            if (shouldRedirectOutput)
                cOut.CmdOutPut.Append(proc.StandardOutput);

            cOut.ExitCode = proc.ExitCode;

            return cOut;
        }
    }
}
