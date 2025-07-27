using System.Windows;
using MySimplenote.Services;

namespace MySimplenote;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var themeService = new ThemeService();
        
        base.OnStartup(e);
    }
}