using System;
using System.Drawing;
using System.Windows.Forms;
using MultiTimer.Presenters;
using MultiTimer.Views;

namespace MultiTimer;

public partial class Form1 : Form, IMainView
{
    private readonly FlowLayoutPanel _toolbar;
    private readonly Button _btnAdd;
    private readonly CheckBox _chkTopMost;
    private readonly Panel _headerPanel;
    private readonly Panel _timerContainer;
    private MainPresenter? _presenter;

    private const int ToolbarHeight = 36;
    private const int HeaderHeight = 24;
    private const int TimerRowHeight = 36;
    private const int WindowPadding = 50;
    private const int MaxAutoHeight = 800;

    public event EventHandler? AddTimerClicked;
    public event EventHandler? TopMostChanged;
    public event FormClosingEventHandler? ViewClosing;

    public bool IsTopMost
    {
        get => _chkTopMost.Checked;
        set
        {
            _chkTopMost.Checked = value;
            TopMost = value;
        }
    }

    public IntPtr ViewHandle => Handle;
    public int TimerCount => _timerContainer.Controls.Count;

    public Form1()
    {
        InitializeComponent();

        Text = "多功能計時器";
        Width = 770;
        Height = 160;
        MinimumSize = new Size(770, 160);
        StartPosition = FormStartPosition.CenterScreen;

        // 工具列
        _toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = ToolbarHeight,
            Padding = new Padding(4, 4, 4, 0),
            AutoSize = false
        };

        _btnAdd = new Button { Text = "＋ 新增計時器", AutoSize = true };
        _btnAdd.Click += (_, _) => AddTimerClicked?.Invoke(this, EventArgs.Empty);

        _chkTopMost = new CheckBox { Text = "視窗置頂", AutoSize = true, Padding = new Padding(12, 4, 0, 0) };
        _chkTopMost.CheckedChanged += (_, _) =>
        {
            TopMost = _chkTopMost.Checked;
            TopMostChanged?.Invoke(this, EventArgs.Empty);
        };

        _toolbar.Controls.Add(_btnAdd);
        _toolbar.Controls.Add(_chkTopMost);

        // 表頭
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = HeaderHeight,
            BackColor = SystemColors.ControlLight,
            Padding = new Padding(0)
        };
        CreateHeaderLabels();

        // 計時器容器
        _timerContainer = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 2, 0, 2)
        };

        Controls.Add(_timerContainer);
        Controls.Add(_headerPanel);
        Controls.Add(_toolbar);

        // 建立 Presenter
        _presenter = new MainPresenter(this);
    }

    private void CreateHeaderLabels()
    {
        var font = new Font(Font.FontFamily, 8.5f, FontStyle.Bold);
        int y = 4;
        int x = 4;

        AddHeaderLabel("名稱", x, y, 90, font); x += 96;
        AddHeaderLabel("模式", x, y, 62, font); x += 68;
        AddHeaderLabel("時", x, y, 52, font); x += 58;
        AddHeaderLabel("分", x, y, 52, font); x += 58;
        AddHeaderLabel("秒", x, y, 52, font); x += 58;
        AddHeaderLabel("快捷鍵", x, y, 100, font); x += 106;
        AddHeaderLabel("自動", x, y, 48, font); x += 48;
        AddHeaderLabel("計時", x, y, 100, font); x += 106;
        AddHeaderLabel("操作", x, y, 138, font);
    }

    private void AddHeaderLabel(string text, int x, int y, int width, Font font)
    {
        var lbl = new Label
        {
            Text = text,
            Font = font,
            Location = new Point(x, y),
            AutoSize = false,
            Width = width,
            Height = 16,
            TextAlign = ContentAlignment.MiddleCenter
        };
        _headerPanel.Controls.Add(lbl);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _presenter?.InitializeHotkeys();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == GlobalHotkey.WM_HOTKEY)
        {
            if (_presenter?.ProcessHotkeyMessage(m) == true)
                return;
        }
        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        ViewClosing?.Invoke(this, e);
        _presenter?.Dispose();
        base.OnFormClosing(e);
    }

    public TimerPanelView AddTimerPanel()
    {
        var panel = new TimerPanelView();
        _timerContainer.Controls.Add(panel);
        _timerContainer.Controls.SetChildIndex(panel, _timerContainer.Controls.Count - 1);
        return panel;
    }

    public void RemoveTimerPanel(TimerPanelView panel)
    {
        _timerContainer.Controls.Remove(panel);
        panel.Dispose();
    }

    public void AdjustHeight(int count)
    {
        int desiredHeight = ToolbarHeight + HeaderHeight + (count * TimerRowHeight) + WindowPadding;
        if (desiredHeight > MaxAutoHeight)
            desiredHeight = MaxAutoHeight;
        if (desiredHeight < 160)
            desiredHeight = 160;
        Height = desiredHeight;
    }
}
