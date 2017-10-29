using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using XDropsWater.CrossCutting.Win;

namespace XDropsWater.CrossCutting.WinProcess
{
    /// <summary>
    /// 
    /// </summary>
    public static class CmdContainer
    {
        /// <summary>
        /// 执行cmd命令
        /// </summary>
        public static void ExecuteUseCmd(string cmdStr)
        {
            ExecuteUseCmd(new string[] { cmdStr });
        }

        /// <summary>
        /// 执行cmd命令
        /// </summary>
        public static void ExecuteUseCmd(string[] cmdStrArr)
        {
            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                foreach (string cmd in cmdStrArr)
                {
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.Flush();
                }
                p.StandardInput.WriteLine("Exit");
                p.WaitForExit();
            }
        }


        /// <summary>
        /// 执行cmd命令 带输出参数
        /// </summary>
        public static void ExecuteUseCmd(string cmdStr, DataReceivedEventHandler cmdOutputHandler,
            DataReceivedEventHandler cmdErrorOutputHandler)
        {
            ExecuteUseCmd(new string[] { cmdStr }, cmdOutputHandler, cmdErrorOutputHandler, false);
        }

        /// <summary>
        /// 执行cmd命令 带输出参数 是否异步
        /// </summary>
        public static void ExecuteUseCmd(string[] cmdStrArr, DataReceivedEventHandler cmdOutputHandler,
            DataReceivedEventHandler cmdErrorOutputHandler, bool asyn)
        {
            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += cmdOutputHandler;
                p.ErrorDataReceived += cmdErrorOutputHandler;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                foreach (string cmd in cmdStrArr)
                {
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.Flush();
                }
                p.StandardInput.WriteLine("Exit");
                if (!asyn)
                {
                    p.WaitForExit();
                }
            }
        }

        /// <summary>
        /// 执行cmd命令 带输出参数
        /// </summary>
        public static void ExecuteUseCmd(string[] cmdStrArr, DataReceivedEventHandler cmdOutputHandler,
            DataReceivedEventHandler cmdErrorOutputHandler, ref int pid)
        {
            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += cmdOutputHandler;
                p.ErrorDataReceived += cmdErrorOutputHandler;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                pid = p.Id;
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                foreach (string cmd in cmdStrArr)
                {
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.Flush();
                }
                p.StandardInput.WriteLine("Exit");
                p.WaitForExit();
            }
        }

        /// <summary>
        /// 启动控制台进程
        /// </summary>
        public static void ExecuteUserConsoleApp(string fileName, string args, DataReceivedEventHandler cmdOutputHandler,
            DataReceivedEventHandler cmdErrorOutputHandler, ref int pid)
        {
            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += cmdOutputHandler;
                p.ErrorDataReceived += cmdErrorOutputHandler;
                p.StartInfo.Arguments = args;
                p.StartInfo.FileName = Path.GetFullPath(fileName);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                pid = p.Id;
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }


        public static Process AsyncExecuteUserConsoleApp(string fileName, string args)
        {
            using (Process p = new Process())
            {
                p.StartInfo.Arguments = args;
                p.StartInfo.FileName = Path.GetFullPath(fileName);
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                return p;
            }
        }

        /// <summary>
        /// ctrl+c关闭cmd命令
        /// </summary>
        /// <returns></returns>
        public static bool CTRLCExecCmd(int cmdProcessID)
        {
            try
            {

                Process processEntity = Process.GetProcessById(cmdProcessID);
                if (processEntity == null) return false;
                bool result = API.GenerateConsoleCtrlEvent(0, 0);
                processEntity.WaitForExit();

                return result;
            }
            catch { return false; }
        }

        /// <summary>
        /// 注释cmd非法参数
        /// </summary>
        public static string EscapeIllegalCmdArg(string data)
        {
            return string.Format("\"{0}\"", data.Replace(@"\", @"\\").Replace("\"", "\\\""));
        }
    }
}
