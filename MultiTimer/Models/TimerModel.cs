using System;
using System.Windows.Forms;

namespace MultiTimer.Models;

/// <summary>
/// 計時器核心邏輯（不依賴 UI）。
/// </summary>
public class TimerModel
{
    public string Name { get; set; } = "計時器";
    public TimerMode Mode { get; set; } = TimerMode.CountUp;
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public Keys Hotkey { get; set; } = Keys.None;

    public TimeSpan Elapsed { get; private set; }
    public TimeSpan Target { get; private set; }
    public bool IsRunning { get; private set; }

    public event EventHandler? TimeUpdated;
    public event EventHandler? TimerCompleted;

    /// <summary>開始或繼續計時</summary>
    public bool Start()
    {
        if (IsRunning) return false;

        if (Elapsed == TimeSpan.Zero)
        {
            Target = new TimeSpan(Hours, Minutes, Seconds);

            if (Mode == TimerMode.Countdown)
            {
                if (Target == TimeSpan.Zero) return false;
                Elapsed = Target;
            }
        }

        IsRunning = true;
        return true;
    }

    /// <summary>暫停計時</summary>
    public void Pause()
    {
        IsRunning = false;
    }

    /// <summary>重置計時</summary>
    public void Reset()
    {
        IsRunning = false;
        Elapsed = TimeSpan.Zero;
        Target = TimeSpan.Zero;
        TimeUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>每秒呼叫一次，推進計時邏輯</summary>
    public void Tick()
    {
        if (!IsRunning) return;

        if (Mode == TimerMode.Countdown)
        {
            Elapsed = Elapsed.Subtract(TimeSpan.FromSeconds(1));
            if (Elapsed <= TimeSpan.Zero)
            {
                Elapsed = TimeSpan.Zero;
                IsRunning = false;
                TimeUpdated?.Invoke(this, EventArgs.Empty);
                TimerCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        else
        {
            Elapsed = Elapsed.Add(TimeSpan.FromSeconds(1));
            if (Target != TimeSpan.Zero && Elapsed >= Target)
            {
                IsRunning = false;
                TimeUpdated?.Invoke(this, EventArgs.Empty);
                TimerCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        TimeUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>從資料還原</summary>
    public void LoadFrom(TimerData data)
    {
        Name = data.Name;
        Mode = data.ModeIndex == 1 ? TimerMode.Countdown : TimerMode.CountUp;
        Hours = data.Hours;
        Minutes = data.Minutes;
        Seconds = data.Seconds;

        if (int.TryParse(data.HotkeyText, out int keyVal) && keyVal != 0)
            Hotkey = (Keys)keyVal;
        else
            Hotkey = Keys.None;
    }

    /// <summary>匯出為可序列化資料</summary>
    public TimerData ToData()
    {
        return new TimerData
        {
            Name = Name,
            ModeIndex = Mode == TimerMode.Countdown ? 1 : 0,
            Hours = Hours,
            Minutes = Minutes,
            Seconds = Seconds,
            HotkeyText = ((int)Hotkey).ToString()
        };
    }
}

public enum TimerMode
{
    CountUp = 0,
    Countdown = 1
}
