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

using DotNet4Parse;
using Dotnet4Parse.Parse;

namespace Dotnet4Parse;

public partial class Gui : Window
{
    public Gui()
    {
        InitializeComponent();
    }
    private void ButtonAddName_Click(object sender, RoutedEventArgs e)
    {

    }
    private async void BtnStartExtract(object sender, RoutedEventArgs e)
    {

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
    public List<TreeNode> RootNodes { get; set; } = new List<TreeNode>();

    private async void BtnGetFolder(object sender, RoutedEventArgs e)
    {
        var mount = new Mount();
        var provider = mount.LoadAndMount(usmapFile.Text, AesKey.Text, GamePakDir.Text);
        var files = provider.Files.Keys.ToList();

        FolderTree.ItemsSource = null; // Clear previous items

        // Build tree in background
        var root = await Task.Run(() =>
        {
            var rootNode = new TreeNode { Name = "Root" };

            foreach (var file in files)
            {
                string[] parts = file.Split(new[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
                var current = rootNode;

                foreach (var part in parts)
                {
                    if (!current.Children.TryGetValue(part, out var child))
                    {
                        child = new TreeNode { Name = part };
                        current.Children[part] = child;
                    }
                    current = child;
                }
            }

            return rootNode;
        });

        // Bind root node(s) to TreeView
        RootNodes.Clear();
        RootNodes.Add(root);

        FolderTree.ItemsSource = provider.Files;
    }

}