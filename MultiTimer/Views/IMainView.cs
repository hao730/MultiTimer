using System;
using System.Windows.Forms;

namespace MultiTimer.Views;

/// <summary>
/// 主視窗的 View 介面。
/// </summary>
public interface IMainView
{
    bool IsTopMost { get; set; }
    IntPtr ViewHandle { get; }

    event EventHandler AddTimerClicked;
    event EventHandler TopMostChanged;
    event FormClosingEventHandler ViewClosing;

    TimerPanelView AddTimerPanel();
    void RemoveTimerPanel(TimerPanelView panel);
    int TimerCount { get; }
    void AdjustHeight(int count);
}
