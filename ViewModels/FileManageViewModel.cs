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
    public class FileManageViewModel : Conductor<object>
    {
        private string _phonePath;
        private string _windowsPath;
        private string result;
        public static string relativePath = @"..\..\ADB\platform-tools\adb.exe";
        string adbPath = Path.Combine(Environment.CurrentDirectory, relativePath);
        public string PhonePath
        {
            get { return _phonePath; }
            set { _phonePath = value; NotifyOfPropertyChange(nameof(PhonePath)); }
        }
        public string WindowsPath
        {
            get { return _windowsPath; }
            set { _windowsPath = value; NotifyOfPropertyChange(nameof(WindowsPath)); }
        }
        public void Push()
        {
            string adbCommand = $"push {WindowsPath} {PhonePath}";
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
        public void Pull()
        {
            string adbCommand = $"pull {WindowsPath} {PhonePath}";
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
}
