using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ADBGui
{
    public class TreeViewNode
    {
        public string Text { get; set; } // 节点的文本（节点名称）
        public string FullPath { get; set; }// 添加 FullPath 属性
        public ObservableCollection<TreeViewNode> Nodes { get; set; } // 子节点的集合
        public TreeViewItem TreeViewItem { get; set; } // 用于关联TreeViewItem

        public TreeViewNode()
        {
            Nodes = new ObservableCollection<TreeViewNode>();
        }
    }
}
