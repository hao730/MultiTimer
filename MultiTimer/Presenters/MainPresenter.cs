using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MultiTimer.Models;
using MultiTimer.Views;

namespace MultiTimer.Presenters;

/// <summary>
/// 主視窗的 Presenter，管理所有計時器 Presenter、全域熱鍵、設定存取。
/// </summary>
public class MainPresenter : IDisposable
{
    private readonly IMainView _view;
    private readonly List<TimerPresenter> _timerPresenters = new();
    private GlobalHotkey? _globalHotkey;

    public MainPresenter(IMainView view)
    {
        _view = view;

        _view.AddTimerClicked += (_, _) => AddTimer();
        _view.TopMostChanged += (_, _) => { /* View 自行處理 TopMost */ };
        _view.ViewClosing += (_, _) => SaveAndCleanup();

        LoadSettings();
    }

    public void InitializeHotkeys()
    {
        _globalHotkey = new GlobalHotkey(_view.ViewHandle);

        foreach (var tp in _timerPresenters)
        {
            RegisterHotkey(tp);
        }
    }

    private void AddTimer(TimerData? data = null)
    {
        var panelView = _view.AddTimerPanel();
        var presenter = new TimerPresenter(panelView, data);

        presenter.RemoveRequested += (_, _) =>
        {
            UnregisterHotkey(presenter);
            _timerPresenters.Remove(presenter);
            _view.RemoveTimerPanel(panelView);
            presenter.Dispose();
            _view.AdjustHeight(_view.TimerCount);
        };

        presenter.HotkeyChanged += (_, _) =>
        {
            UnregisterHotkey(presenter);
            RegisterHotkey(presenter);
        };

        _timerPresenters.Add(presenter);
        _view.AdjustHeight(_view.TimerCount);
    }

    private void RegisterHotkey(TimerPresenter tp)
    {
        if (_globalHotkey == null) return;
        var key = tp.Model.Hotkey;
        if (key == Keys.None) return;

        int id = _globalHotkey.Register(key, () => tp.RestartFromHotkey());
        tp.HotkeyId = id;
    }

    private void UnregisterHotkey(TimerPresenter tp)
    {
        if (_globalHotkey == null || tp.HotkeyId <= 0) return;
        _globalHotkey.Unregister(tp.HotkeyId);
        tp.HotkeyId = -1;
    }

    private void LoadSettings()
    {
        var settings = SettingsService.Load();
        _view.IsTopMost = settings.TopMost;

        if (settings.Timers.Count == 0)
        {
            AddTimer();
        }
        else
        {
            foreach (var data in settings.Timers)
            {
                AddTimer(data);
            }
        }
    }

    private void SaveAndCleanup()
    {
        var settings = new AppSettings
        {
            TopMost = _view.IsTopMost,
            Timers = new()
        };

        foreach (var tp in _timerPresenters)
        {
            settings.Timers.Add(tp.GetData());
        }

        SettingsService.Save(settings);
    }

    public void Dispose()
    {
        foreach (var tp in _timerPresenters)
            tp.Dispose();

        _globalHotkey?.Dispose();
    }
}
