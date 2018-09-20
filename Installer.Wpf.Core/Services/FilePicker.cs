﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Installer.UI;
using Microsoft.Win32;

namespace Installer.Wpf.Core.Services
{
    public class FilePicker : IFilePicker
    {
        public string InitialDirectory { get; set; }
        public List<FileTypeFilter> FileTypeFilter { get; } = new List<FileTypeFilter>();
        public string PickFile()
        {
            var dialog = new OpenFileDialog();
            var lines = FileTypeFilter.Select(x =>
            {
                var exts = string.Join(";", x.Extensions);
                return $"{x.Description}|{exts}";
            });

            var filter = string.Join("|", lines);

            dialog.Filter = filter;
            dialog.FileName = "";
            dialog.InitialDirectory = InitialDirectory;

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return dialog.FileName;
            }
            
            return null;
        }
    }
}
