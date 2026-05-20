using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MultiTimer.Models;
using MultiTimer.Views;

namespace MultiTimer.Presenters;

/// <summary>
/// 單一計時器的 Presenter，協調 TimerModel 與 ITimerView。
/// </summary>
public class TimerPresenter : IDisposable
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private readonly TimerModel _model;
    private readonly ITimerView _view;
    private readonly System.Windows.Forms.Timer _ticker;

    public TimerModel Model => _model;
    public ITimerView View => _view;

    /// <summary>要求移除此計時器</summary>
    public event EventHandler? RemoveRequested;

    /// <summary>快捷鍵變更</summary>
    public event EventHandler? HotkeyChanged;

    public int HotkeyId { get; set; } = -1;

    public TimerPresenter(ITimerView view, TimerData? data = null)
    {
        _model = new TimerModel();
        _view = view;

        if (data != null)
        {
            _model.LoadFrom(data);
            _view.TimerName = _model.Name;
            _view.ModeIndex = data.ModeIndex;
            _view.Hours = _model.Hours;
            _view.Minutes = _model.Minutes;
            _view.Seconds = _model.Seconds;
            if (_model.Hotkey != Keys.None)
                _view.SetHotkey(_model.Hotkey);
            _view.AutoKeyEnabled = _model.AutoKeyEnabled;
        }

        _ticker = new System.Windows.Forms.Timer { Interval = 1000 };
        _ticker.Tick += (_, _) => OnTick();

        _model.TimeUpdated += (_, _) => _view.UpdateTimeDisplay(_model.Elapsed.ToString(@"hh\:mm\:ss"));
        _model.TimerCompleted += (_, _) => OnCompleted();

        _view.StartClicked += (_, _) => OnStartClicked();
        _view.ResetClicked += (_, _) => OnResetClicked();
        _view.RemoveClicked += (_, _) => RemoveRequested?.Invoke(this, EventArgs.Empty);
        _view.ModeChanged += (_, _) => OnModeChanged();
        _view.HotkeyChanged += (_, _) =>
        {
            _model.Hotkey = _view.HotkeyValue;
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    /// <summary>取得目前資料用於儲存</summary>
    public TimerData GetData()
    {
        _model.Name = _view.TimerName;
        _model.Mode = _view.ModeIndex == 1 ? TimerMode.Countdown : TimerMode.CountUp;
        _model.Hours = _view.Hours;
        _model.Minutes = _view.Minutes;
        _model.Seconds = _view.Seconds;
        _model.Hotkey = _view.HotkeyValue;
        _model.AutoKeyEnabled = _view.AutoKeyEnabled;
        return _model.ToData();
    }

    private void OnStartClicked()
    {
        if (_model.IsRunning)
        {
            _model.Pause();
            _ticker.Stop();
            _view.SetStartButtonText("繼續");
            _view.SetSettingsEnabled(false);
        }
        else
        {
            SyncViewToModel();
            if (_model.Start())
            {
                _ticker.Start();
                _view.SetStartButtonText("暫停");
                _view.SetSettingsEnabled(false);
                _view.UpdateTimeDisplay(_model.Elapsed.ToString(@"hh\:mm\:ss"));
            }
            else if (_model.Mode == TimerMode.Countdown)
            {
                MessageBox.Show("倒數模式請設定目標時間。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void OnResetClicked()
    {
        _ticker.Stop();
        _model.Reset();
        _view.SetStartButtonText("開始");
        _view.SetSettingsEnabled(true);
        _view.UpdateTimeDisplay("00:00:00");
    }

    private void OnModeChanged()
    {
        _model.Mode = _view.ModeIndex == 1 ? TimerMode.Countdown : TimerMode.CountUp;
        OnResetClicked();
    }

    private void OnTick()
    {
        _model.Tick();
    }

    private void OnCompleted()
    {
        _ticker.Stop();
        _view.SetStartButtonText("開始");
        _view.SetSettingsEnabled(true);
        SoundService.PlayAlarm();

        // 自動按下快捷鍵
        if (_view.AutoKeyEnabled && _view.HotkeyValue != Keys.None)
        {
            SimulateKeyPress(_view.HotkeyValue);
        }
    }

    private void SimulateKeyPress(Keys key)
    {
        // 拆分修飾鍵與主鍵
        bool ctrl = key.HasFlag(Keys.Control);
        bool alt = key.HasFlag(Keys.Alt);
        bool shift = key.HasFlag(Keys.Shift);
        byte vk = (byte)(key & Keys.KeyCode);

        if (ctrl) keybd_event(0x11, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        if (alt) keybd_event(0x12, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        if (shift) keybd_event(0x10, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);

        keybd_event(vk, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        keybd_event(vk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        if (shift) keybd_event(0x10, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        if (alt) keybd_event(0x12, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        if (ctrl) keybd_event(0x11, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private void SyncViewToModel()
    {
        _model.Name = _view.TimerName;
        _model.Hours = _view.Hours;
        _model.Minutes = _view.Minutes;
        _model.Seconds = _view.Seconds;
    }

    public void Dispose()
    {
        _ticker.Stop();
        _ticker.Dispose();
    }
}
