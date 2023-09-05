using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.ComponentModel;
using ADBGui.Models;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;

namespace ADBGui.ViewModels
{
    public class CatalogViewModel : Conductor<object>
    {
        public ObservableCollection<FoldersModel> RootFolders { get; } = new ObservableCollection<FoldersModel>();
        public CatalogViewModel()
        {
            string sdCardRoot = "/sdcard";
            RootFolders.Add(CreateFolderItem(sdCardRoot));
        }
        private FoldersModel CreateFolderItem(string folderPath)
        {
            FoldersModel item = new FoldersModel();
            item.Name = System.IO.Path.GetFileName(folderPath);
            item.Path = folderPath;
            item.Subfolders.Add(new FoldersModel { Name = "Loading..." });
            return item;
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
            {
                FoldersModel folderItem = item.DataContext as FoldersModel;
                if (folderItem != null && folderItem.Subfolders.Count == 1 && folderItem.Subfolders[0].Name == "Loading...")
                {
                    folderItem.Subfolders.Clear();
                    folderItem.Path = System.IO.Path.Combine(folderItem.Path.Replace('\\', '/'));
                    LoadFolderContents(folderItem);
                }
            }
        }

        private void LoadFolderContents(FoldersModel parentFolder)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\xiaolongxia\Desktop\C# windows开发\ADBGui\ADB\platform-tools\adb.exe";
            process.StartInfo.Arguments = $"shell ls {parentFolder.Path}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] folderNames = output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string folderName in folderNames)
            {
                if (!folderName.StartsWith(".") && !folderName.StartsWith(".."))
                {
                    string fullPath = System.IO.Path.Combine(parentFolder.Path, folderName);
                    FoldersModel folderItem = CreateFolderItem(fullPath);
                    parentFolder.Subfolders.Add(folderItem);
                }
            }
        }
    }
}
