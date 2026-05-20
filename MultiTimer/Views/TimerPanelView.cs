using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiTimer.Views;

/// <summary>
/// 單一計時器面板的 View 實作，以單行表格列呈現。
/// </summary>
public class TimerPanelView : Panel, ITimerView
{
    private readonly TextBox _txtName;
    private readonly ComboBox _cboMode;
    private readonly NumericUpDown _nudHours;
    private readonly NumericUpDown _nudMinutes;
    private readonly NumericUpDown _nudSeconds;
    private readonly HotkeyTextBox _txtHotkey;
    private readonly Label _lblTime;
    private readonly Button _btnStart;
    private readonly Button _btnReset;
    private readonly Button _btnRemove;
    private readonly CheckBox _chkAutoKey;

    public event EventHandler? StartClicked;
    public event EventHandler? ResetClicked;
    public event EventHandler? RemoveClicked;
    public event EventHandler? ModeChanged;
    public event EventHandler? HotkeyChanged;

    public string TimerName
    {
        get => _txtName.Text;
        set => _txtName.Text = value;
    }

    public int ModeIndex
    {
        get => _cboMode.SelectedIndex;
        set => _cboMode.SelectedIndex = value;
    }

    public int Hours
    {
        get => (int)_nudHours.Value;
        set => _nudHours.Value = value;
    }

    public int Minutes
    {
        get => (int)_nudMinutes.Value;
        set => _nudMinutes.Value = value;
    }

    public int Seconds
    {
        get => (int)_nudSeconds.Value;
        set => _nudSeconds.Value = value;
    }

    public Keys HotkeyValue => _txtHotkey.HotkeyValue;

    public void SetHotkey(Keys key) => _txtHotkey.SetHotkey(key);

    public void UpdateTimeDisplay(string text) => _lblTime.Text = text;

    public void SetStartButtonText(string text) => _btnStart.Text = text;

    public void SetSettingsEnabled(bool enabled)
    {
        _cboMode.Enabled = enabled;
        _nudHours.Enabled = enabled;
        _nudMinutes.Enabled = enabled;
        _nudSeconds.Enabled = enabled;
    }

    public bool AutoKeyEnabled
    {
        get => _chkAutoKey.Checked;
        set => _chkAutoKey.Checked = value;
    }

    public TimerPanelView()
    {
        Height = 34;
        Dock = DockStyle.Top;
        Margin = new Padding(0, 0, 0, 1);
        BackColor = SystemColors.Window;

        int y = 5;
        int x = 4;

        // 名稱
        _txtName = new TextBox { Width = 90, Text = "計時器", Location = new Point(x, y), BorderStyle = BorderStyle.FixedSingle };
        x += 96;

        // 模式
        _cboMode = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 62,
            Location = new Point(x, y)
        };
        _cboMode.Items.AddRange(new object[] { "正數", "倒數" });
        _cboMode.SelectedIndex = 0;
        _cboMode.SelectedIndexChanged += (_, _) => ModeChanged?.Invoke(this, EventArgs.Empty);
        x += 68;

        // 時
        _nudHours = new NumericUpDown { Width = 52, Minimum = 0, Maximum = 99, Location = new Point(x, y) };
        x += 58;

        // 分
        _nudMinutes = new NumericUpDown { Width = 52, Minimum = 0, Maximum = 59, Location = new Point(x, y) };
        x += 58;

        // 秒
        _nudSeconds = new NumericUpDown { Width = 52, Minimum = 0, Maximum = 59, Location = new Point(x, y) };
        x += 58;

        // 快捷鍵
        _txtHotkey = new HotkeyTextBox { Width = 100, Location = new Point(x, y) };
        _txtHotkey.HotkeyChanged += (_, _) => HotkeyChanged?.Invoke(this, EventArgs.Empty);
        x += 106;

        // 自動按鍵
        _chkAutoKey = new CheckBox { AutoSize = false, Width = 20, Height = 20, Location = new Point(x + 14, y + 2) };
        x += 48;

        // 經過時間
        _lblTime = new Label
        {
            Text = "00:00:00",
            Font = new Font("Consolas", 12, FontStyle.Bold),
            AutoSize = false,
            Width = 100,
            Height = 24,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(x, y - 1)
        };
        x += 106;

        // 開始
        _btnStart = new Button { Text = "開始", Width = 52, Height = 26, Location = new Point(x, y - 1), TabStop = false };
        _btnStart.Click += (_, _) => StartClicked?.Invoke(this, EventArgs.Empty);
        _btnStart.GotFocus += (_, _) => Parent?.Focus();
        x += 56;

        // 重置
        _btnReset = new Button { Text = "重置", Width = 52, Height = 26, Location = new Point(x, y - 1), TabStop = false };
        _btnReset.Click += (_, _) => ResetClicked?.Invoke(this, EventArgs.Empty);
        _btnReset.GotFocus += (_, _) => Parent?.Focus();
        x += 56;

        // 刪除
        _btnRemove = new Button { Text = "✕", Width = 30, Height = 26, Location = new Point(x, y - 1), ForeColor = Color.Red, TabStop = false };
        _btnRemove.Click += (_, _) => RemoveClicked?.Invoke(this, EventArgs.Empty);
        _btnRemove.GotFocus += (_, _) => Parent?.Focus();

        Controls.AddRange(new Control[]
        {
            _txtName, _cboMode,
            _nudHours, _nudMinutes, _nudSeconds,
            _txtHotkey, _chkAutoKey,
            _lblTime, _btnStart, _btnReset, _btnRemove
        });
    }
}
