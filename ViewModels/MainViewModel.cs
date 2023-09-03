using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ADBGui.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace ADBGui.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IWindowManager _windowManager;
        private DeviceInfoModel _selectedDevice;
        private string _serialNumber;
        private string _deviceIpAddress;
        private string _deviceModel;
        private string result;
        public static string relativePath = @"..\..\ADB\platform-tools\adb.exe";
        string adbPath = Path.Combine(Environment.CurrentDirectory, relativePath);
        private string device;


        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; NotifyOfPropertyChange(nameof(SerialNumber)); }
        }
        public string DeviceIpAddress
        {
            get { return _deviceIpAddress; }
            set { _deviceIpAddress = value; NotifyOfPropertyChange(nameof(DeviceIpAddress)); }
        }
        public string DeviceModel
        {
            get { return _deviceModel; }
            set { _deviceModel = value; NotifyOfPropertyChange(nameof(DeviceModel)); }
        }
        public DeviceInfoModel SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; NotifyOfPropertyChange(nameof(SelectedDevice));}
        }
        public void AddDevice()
        {
            string adbCommand = "devices";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = adbPath,
                Arguments = adbCommand,
                RedirectStandardOutput = true,
                UseShellExecute=false,
            };
            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                device = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            if (device != null)
            {
                SerialNumber = GetSerialNumber();
                DeviceModel = GetDeviceModel();
                DeviceIpAddress = GetIpAddress();
            }
        }
        private string GetSerialNumber()
        {
            string adbCommand = "get-serialno";
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

                SerialNumber = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return SerialNumber;
            }
        }
        private string GetDeviceModel()
        {
            string adbCommand = $"shell getprop ro.product.model";
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

                DeviceModel = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return DeviceModel;
            }
        }
        private string GetIpAddress()
        {
            string adbCommand = "shell ifconfig wlan0 | grep 'inet ' | awk '{print $2}'";
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

                DeviceIpAddress = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return DeviceIpAddress;
            }
        }
        public void RemoveDevice()
        {
            string adbCommand = "kill-server";
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
            SerialNumber = null;
            DeviceModel=null;
            DeviceIpAddress = null;
            
        }

        public Task InstallOrUninstallApk()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<ApkManageViewModel>());
        }
        public Task DownloadOrUploadFile()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<FileManageViewModel>());
        }
        public Task ExportLogs()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<LogViewModel>());
        }
        public Task BrowseContents()
        {
            return _windowManager.ShowDialogAsync(IoC.Get<CatalogViewModel>());
        }
    }
}
