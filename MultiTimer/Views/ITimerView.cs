using System;
using System.Windows.Forms;

namespace MultiTimer.Views;

/// <summary>
/// 單一計時器面板的 View 介面。
/// </summary>
public interface ITimerView
{
    string TimerName { get; set; }
    int ModeIndex { get; set; }
    int Hours { get; set; }
    int Minutes { get; set; }
    int Seconds { get; set; }
    Keys HotkeyValue { get; }
    void SetHotkey(Keys key);
    void UpdateTimeDisplay(string text);
    void SetStartButtonText(string text);
    void SetSettingsEnabled(bool enabled);

    event EventHandler StartClicked;
    event EventHandler ResetClicked;
    event EventHandler RemoveClicked;
    event EventHandler ModeChanged;
    event EventHandler HotkeyChanged;
}
