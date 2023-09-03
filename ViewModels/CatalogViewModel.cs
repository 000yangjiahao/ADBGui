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

namespace ADBGui.ViewModels
{
    public class CatalogViewModel : Conductor<object>
    {
        private string _fileName;
        private string _folderName;
        private string result;
        private ObservableCollection<FilesModel> _files = new ObservableCollection<FilesModel>();
        private ObservableCollection<FoldersModel> _folders = new ObservableCollection<FoldersModel>();
        public static string relativePath = @"..\..\ADB\platform-tools\adb.exe";
        string adbPath = Path.Combine(Environment.CurrentDirectory, relativePath);
        private TreeViewNode _selectedNode;
        private ObservableCollection<TreeViewNode> _treeViewNodes = new ObservableCollection<TreeViewNode>();
        private TreeViewNode _rootNode = new TreeViewNode { Text = "sdcard", FullPath = "/sdcard/" };

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; NotifyOfPropertyChange(nameof(FileName)); }
        }
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; NotifyOfPropertyChange(nameof(FolderName)); }
        }
        public ObservableCollection<FoldersModel> Folders
        {
            get { return _folders; }
            set { _folders = value; NotifyOfPropertyChange(nameof(Folders)); }
        }
        public ObservableCollection<FilesModel> Files
        {
            get { return _files; }
            set { _files = value; NotifyOfPropertyChange(nameof(Files)); }
        }
        public TreeViewNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                NotifyOfPropertyChange(() => SelectedNode);
                if (_selectedNode != null)
                {
                    UpdateFilesList(_selectedNode.FullPath);
                }
            }
        }
        public TreeViewNode RootNode
        {
            get { return _rootNode; }
            set { _rootNode = value; NotifyOfPropertyChange(nameof(RootNode)); }
        }
        public ObservableCollection<TreeViewNode> TreeViewNodes
        {
            get { return _treeViewNodes; }
            set { _treeViewNodes = value; NotifyOfPropertyChange(() => TreeViewNodes); }
        }

        public CatalogViewModel()
        {
            InitializeFolders();
        }

        public void TreeNodeSelected(object selectedNode)
        {
            // 处理节点选择的逻辑
            TreeViewNode selectedTreeNode = selectedNode as TreeViewNode;
            if (selectedTreeNode != null)
            {
                // 在这里处理选择节点后的操作，例如更新文件列表
                string selectedFolderPath = selectedTreeNode.FullPath;
                UpdateFilesList(selectedFolderPath);
            }
        }
        private void InitializeFolders()
        {
            ExecuteADBCommand("shell ls -d /sdcard/*/");

            RootNode.Text = "sdcard"; // 设置根节点的名称

            foreach (var folder in Folders)
            {
                TreeViewNode subNode = new TreeViewNode();
                subNode.Text = folder.FolderName;
                RootNode.Nodes.Add(subNode);
            }

            // 将根节点添加到 TreeViewNodes 集合中
            TreeViewNodes.Add(RootNode);

            // 默认选中根节点
            SelectedNode = RootNode;
        }

        private void UpdateFilesList(string folderPath)
        {
            ExecuteADBCommand($"shell ls -l {folderPath}");
        }

        private void ExecuteADBCommand(string adbCommand)
        {
            try
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
                ParseADBResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ParseADBResult(string result)
        {
            var lines = result.Split('\n').Select(line => line.Trim()).ToList();

            Folders.Clear();
            Files.Clear();

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    bool isDirectory = line.EndsWith("/");
                    bool isFile = line.Contains(".");
                    if (isDirectory)
                    {
                        int lastSlashIndex = line.LastIndexOf('/');
                        if (lastSlashIndex >= 0)
                        {
                            int secondLastSlashIndex = line.LastIndexOf('/', lastSlashIndex - 1);
                            if (secondLastSlashIndex >= 0)
                            {
                                string folderName = line.Substring(secondLastSlashIndex + 1, lastSlashIndex - secondLastSlashIndex - 1);
                                Folders.Add(new FoldersModel() { FolderName=folderName });
                            }
                        }
                    }
                    else if(isFile)
                    {
                        int lastSlashIndex = line.LastIndexOf('/');
                        string fileName = line.Substring(lastSlashIndex + 1, line.Length);
                        Files.Add(new FilesModel() { FileName=fileName });
                    }
                }
            }
        }
    }
}
