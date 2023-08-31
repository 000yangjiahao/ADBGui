using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.IO;
using System.Windows;

namespace ADBGui.ViewModels
{
    public class ApkManageViewModel : Conductor<object>
    {
        private string _packageName;
        private string _filePath;
        private string result;
        public static string relativePath = @"..\..\ADB\platform-tools\adb.exe";
        string adbPath = Path.Combine(Environment.CurrentDirectory, relativePath);
        public string PackageName
        {
            get { return _packageName; }
            set { _packageName = value; NotifyOfPropertyChange(nameof(PackageName)); }
        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; NotifyOfPropertyChange(nameof(FilePath)); }
        }
        public async void Install()
        {
            string adbCommand = $"install {FilePath}";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = adbPath,
                Arguments = adbCommand,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            await Task.Run(() =>
            {
                 using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.Start();
                    result= process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    process.Close();

                }
            });
            MessageBox.Show(result);

        }
        public void Uninstall()
        {
            string adbCommand = $"uninstall {PackageName}";
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
