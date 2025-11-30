using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel; // Add this
using System.Collections.Generic; // Add this
using System.Linq; // Add this
using System.Threading.Tasks; // Add this

using DotNet4Parse;
using Dotnet4Parse.Parse;

namespace Dotnet4Parse;

public partial class Gui : Window
{
    public ObservableCollection<TreeNode> RootNodes { get; set; } = new ObservableCollection<TreeNode>();

    public Gui()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void BtnStartExtract(object sender, RoutedEventArgs e)
    {
        // Your implementation
    }

    private void openUsmap(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "USMAP Files (*.usmap)|*.usmap",
            Title = "Select a .usmap file"
        };

        if (dialog.ShowDialog() == true)
        {
            usmapFile.Text = dialog.FileName;
        }
    }

    private void openInputFolder(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog()
        {
            Title = "Select GamePak Directory"
        };

        if (dialog.ShowDialog() == true)
        {
            GamePakDir.Text = dialog.FolderName;
        }
    }

    private void openOutputFolder(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog()
        {
            Title = "Select Output Directory"
        };

        if (dialog.ShowDialog() == true)
        {
            OutputDir.Text = dialog.FolderName;
        }
    }

    private async void BtnGetFolder(object sender, RoutedEventArgs e)
    {
        // Fix 1: Handle null/empty values properly
        string? usmapPath = File.Exists(usmapFile.Text) 
            ? usmapFile.Text 
            : null;

        string aesKey = !string.IsNullOrEmpty(AesKey.Text) ? AesKey.Text : string.Empty;

        if (!Directory.Exists(GamePakDir.Text))
        {
            MessageBox.Show("Please select a valid GamePak directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var mount = new Mount();
        var provider = mount.LoadAndMount(usmapPath, aesKey, GamePakDir.Text);
        var files = provider.Files.Keys.ToList();

        RootNodes.Clear();

        var nodes = await Task.Run(() =>
        {
            var topLevel = new Dictionary<string, TreeNode>();

            foreach (var file in files)
            {
                string[] parts = file.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, TreeNode> currentDict = topLevel;
                TreeNode? currentNode = null;

                foreach (var part in parts)
                {
                    if (!currentDict.TryGetValue(part, out currentNode))
                    {
                        currentNode = new TreeNode { Name = part };
                        currentDict[part] = currentNode;
                    }

                    currentDict = currentNode.Children;
                }
            }

            return topLevel.Values.ToList();
        });

        // Update UI on main thread
        foreach (var node in nodes)
        {
            RootNodes.Add(node);
        }
    }

    private void openAbout(object sender, RoutedEventArgs e)
    {
        string aboutText = @"
DotNet4Parse
Parser application for UE4 files

LEGAL DISCLAIMER:
This software is provided 'AS IS' without warranty of any kind, either express or implied, including, but not limited to, the implied warranties of merchantability, fitness for a particular purpose, or non-infringement. The entire risk as to the quality and performance of the program is with you.

LICENSE:
This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License (CC BY-SA 4.0).
To view a copy of this license, visit: https://creativecommons.org/licenses/by-sa/4.0/

THIRD-PARTY COMPONENTS:
• CUE4Parse
GitHub: https://github.com/FabianFG/CUE4Parse

• Newtonsoft.Json (Json.NET)
Website: https://www.newtonsoft.com/json

• Icon
Website: http://www.playthroneandliberty.com

SOURCE CODE:
The source code for this application is available and modifications are encouraged under the terms of the CC BY-SA 4.0 license.

© 2024 MyCompany. All rights reserved.
".Trim();

        MessageBox.Show(aboutText, "About DotNet4Parse", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}