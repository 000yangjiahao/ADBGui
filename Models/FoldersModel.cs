using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBGui.Models
{
    public class FoldersModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ObservableCollection<FoldersModel> Subfolders { get; } = new ObservableCollection<FoldersModel>();
    }
}
