using System;
using System.IO;
using System.Text.Json;

namespace MultiTimer.Models;

/// <summary>
/// 負責設定的儲存與載入。
/// </summary>
public static class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public static AppSettings Load()
    {
        if (!File.Exists(SettingsPath))
            return new AppSettings();

        try
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public static void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }
}
