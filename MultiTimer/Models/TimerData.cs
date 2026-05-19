namespace MultiTimer.Models;

/// <summary>
/// 用於序列化/反序列化計時器設定的資料類別。
/// </summary>
public class TimerData
{
    public string Name { get; set; } = "計時器";
    public int ModeIndex { get; set; } = 0; // 0=正數, 1=倒數
    public int Hours { get; set; } = 0;
    public int Minutes { get; set; } = 0;
    public int Seconds { get; set; } = 0;
    public string HotkeyText { get; set; } = "";
}

public class AppSettings
{
    public bool TopMost { get; set; } = false;
    public List<TimerData> Timers { get; set; } = new();
}
