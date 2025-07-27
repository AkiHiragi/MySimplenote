using System;
using System.IO;
using System.Windows;

namespace MySimplenote.Services;

public class ThemeService : IThemeService
{
    private const string SettingsFile = "theme.txt";
        
    public string CurrentTheme { get; private set; } = "Light";
        
    public string[] AvailableThemes => new[] { "Light", "Dark" };

    public ThemeService()
    {
        LoadSavedTheme();
        ApplyTheme(CurrentTheme);
    }

    public void SetTheme(string themeName)
    {
        if (Array.IndexOf(AvailableThemes, themeName) == -1)
            return;

        CurrentTheme = themeName;
        ApplyTheme(themeName);
        SaveTheme(themeName);
    }

    private void ApplyTheme(string themeName)
    {
        var app = Application.Current;
    
        // Очищаем все ресурсы
        app.Resources.MergedDictionaries.Clear();
    
        // Загружаем новую тему
        app.Resources.MergedDictionaries.Add(new ResourceDictionary 
        { 
            Source = new Uri($"Themes/{themeName}/Colors.xaml", UriKind.Relative) 
        });
        app.Resources.MergedDictionaries.Add(new ResourceDictionary 
        { 
            Source = new Uri($"Themes/{themeName}/Brushes.xaml", UriKind.Relative) 
        });
        app.Resources.MergedDictionaries.Add(new ResourceDictionary 
        { 
            Source = new Uri("Styles/AppStyles.xaml", UriKind.Relative) 
        });
    }

    private void LoadSavedTheme()
    {
        try
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MySimplenote");
            var settingsPath = Path.Combine(appDataPath, SettingsFile);
                
            if (File.Exists(settingsPath))
            {
                CurrentTheme = File.ReadAllText(settingsPath).Trim();
            }
        }
        catch { /* Игнорируем ошибки */ }
    }

    private void SaveTheme(string themeName)
    {
        try
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MySimplenote");
            Directory.CreateDirectory(appDataPath);
            var settingsPath = Path.Combine(appDataPath, SettingsFile);
                
            File.WriteAllText(settingsPath, themeName);
        }
        catch { /* Игнорируем ошибки */ }
    }
}