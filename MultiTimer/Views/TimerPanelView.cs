using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiTimer.Views;

/// <summary>
/// 單一計時器面板的 View 實作。只負責 UI 呈現與事件轉發。
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

    public TimerPanelView()
    {
        BorderStyle = BorderStyle.FixedSingle;
        Height = 60;
        Dock = DockStyle.Top;
        Padding = new Padding(4, 4, 4, 4);
        Margin = new Padding(0, 0, 0, 6);

        int y = 18;

        _txtName = new TextBox { Width = 80, Text = "計時器", Location = new Point(6, y) };

        _cboMode = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 58,
            Location = new Point(90, y)
        };
        _cboMode.Items.AddRange(new object[] { "正數", "倒數" });
        _cboMode.SelectedIndex = 0;
        _cboMode.SelectedIndexChanged += (_, _) => ModeChanged?.Invoke(this, EventArgs.Empty);

        int x = 155;
        _nudHours = new NumericUpDown { Width = 50, Minimum = 0, Maximum = 99, Location = new Point(x, y) };
        var lblH = new Label { Text = "時", AutoSize = true, Location = new Point(x + 54, y + 3), BackColor = Color.Transparent };

        x = 236;
        _nudMinutes = new NumericUpDown { Width = 50, Minimum = 0, Maximum = 59, Location = new Point(x, y) };
        var lblM = new Label { Text = "分", AutoSize = true, Location = new Point(x + 54, y + 3), BackColor = Color.Transparent };

        x = 317;
        _nudSeconds = new NumericUpDown { Width = 50, Minimum = 0, Maximum = 59, Location = new Point(x, y) };
        var lblS = new Label { Text = "秒", AutoSize = true, Location = new Point(x + 54, y + 3), BackColor = Color.Transparent };

        var lblHk = new Label { Text = "快捷鍵:", AutoSize = true, Location = new Point(395, y + 3), BackColor = Color.Transparent };
        _txtHotkey = new HotkeyTextBox { Width = 100, Location = new Point(450, y) };
        _txtHotkey.HotkeyChanged += (_, _) => HotkeyChanged?.Invoke(this, EventArgs.Empty);

        _lblTime = new Label
        {
            Text = "00:00:00",
            Font = new Font("Consolas", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(565, y - 3)
        };

        _btnStart = new Button { Text = "開始", Width = 50, Location = new Point(675, y - 2) };
        _btnStart.Click += (_, _) => StartClicked?.Invoke(this, EventArgs.Empty);

        _btnReset = new Button { Text = "重置", Width = 50, Location = new Point(730, y - 2) };
        _btnReset.Click += (_, _) => ResetClicked?.Invoke(this, EventArgs.Empty);

        _btnRemove = new Button { Text = "✕", Width = 28, Location = new Point(785, y - 2), ForeColor = Color.Red };
        _btnRemove.Click += (_, _) => RemoveClicked?.Invoke(this, EventArgs.Empty);

        Controls.AddRange(new Control[]
        {
            _txtName, _cboMode,
            _nudHours, _nudMinutes, _nudSeconds,
            _txtHotkey,
            _lblTime, _btnStart, _btnReset, _btnRemove
        });

        Controls.Add(lblH);
        Controls.Add(lblM);
        Controls.Add(lblS);
        Controls.Add(lblHk);
        lblH.BringToFront();
        lblM.BringToFront();
        lblS.BringToFront();
        lblHk.BringToFront();
    }
}
