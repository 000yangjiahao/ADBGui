using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace ADBGui.ViewModels
{
    public class LogViewModel:Conductor<object>
    {
        public static string relativePath = @"..\..\ADB\platform-tools\adb.exe";
        string adbPath = Path.Combine(Environment.CurrentDirectory, relativePath);
        public string adbCommand { get; set; } =null; 
        private string _windowsPath;
        private string result;
        private int num { get; set; } = 1;
        public string WindowsPath
        {
            get { return _windowsPath; }
            set { _windowsPath = value; NotifyOfPropertyChange(nameof(WindowsPath)); }
        }
        public void ExportLogs()
        {
            if (adbCommand == null)
            {
                adbCommand = $"(logcat -d > device_log.txt) || (echo. > device_log.txt && logcat -d > device_log.txt {WindowsPath})";
                Task.Run(() =>
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = adbCommand,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                    };
                    using (Process process = new Process())
                    {
                        process.StartInfo = processStartInfo;
                        process.Start();
                        result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        process.Close();
                    }
                });
                MessageBox.Show(result);
            }
            else
            {
                Task.Run(() =>
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = adbCommand,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                    };
                    using (Process process = new Process())
                    {
                        process.StartInfo = processStartInfo;
                        process.Start();
                        result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        process.Close();
                    }
                });
                MessageBox.Show(result);
            }

        }
        public void StopExport()
        {
            num++;
        }
    }
}
