namespace MySimplenote.Services;

public interface IThemeService
{
    string   CurrentTheme { get; }
    void     SetTheme(string themeName);
    string[] AvailableThemes { get; }
}