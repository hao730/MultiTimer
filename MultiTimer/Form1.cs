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
    private readonly Panel _timerContainer;
    private MainPresenter? _presenter;

    private const int ToolbarHeight = 40;
    private const int TimerPanelHeight = 66;
    private const int WindowPadding = 50;
    private const int MaxAutoHeight = 800;

    public event EventHandler? AddTimerClicked;
    public event EventHandler? TopMostChanged;
    public new event FormClosingEventHandler? ViewClosing;

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
        Width = 840;
        Height = 160;
        MinimumSize = new Size(840, 160);
        StartPosition = FormStartPosition.CenterScreen;

        // 工具列
        _toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = ToolbarHeight,
            Padding = new Padding(6, 6, 6, 0),
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

        // 計時器容器
        _timerContainer = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(6)
        };

        Controls.Add(_timerContainer);
        Controls.Add(_toolbar);

        // 建立 Presenter
        _presenter = new MainPresenter(this);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _presenter?.InitializeHotkeys();
    }

    protected override void WndProc(ref Message m)
    {
        // 讓 GlobalHotkey 透過 Presenter 處理（直接用靜態方式不太好，
        // 但 WndProc 必須在 Form 層級處理）
        if (m.Msg == GlobalHotkey.WM_HOTKEY)
        {
            // Presenter 內部的 GlobalHotkey 會處理
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
        int desiredHeight = ToolbarHeight + (count * TimerPanelHeight) + WindowPadding;
        if (desiredHeight > MaxAutoHeight)
            desiredHeight = MaxAutoHeight;
        if (desiredHeight < 160)
            desiredHeight = 160;
        Height = desiredHeight;
    }
}
