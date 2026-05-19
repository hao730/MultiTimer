using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MultiTimer.Views;

/// <summary>
/// 自訂 TextBox，按下按鍵組合時顯示快捷鍵文字（如 Ctrl+F5），而非輸入字元。
/// </summary>
public class HotkeyTextBox : TextBox
{
    public Keys HotkeyValue { get; private set; } = Keys.None;

    public HotkeyTextBox()
    {
        ReadOnly = true;
        BackColor = System.Drawing.SystemColors.Window;
        TextAlign = HorizontalAlignment.Center;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.SuppressKeyPress = true;

        if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
            e.KeyCode == Keys.Menu || e.KeyCode == Keys.None)
        {
            return;
        }

        HotkeyValue = e.KeyData;
        Text = KeyToString(e.KeyData);
        OnHotkeyChanged();
    }

    public event EventHandler? HotkeyChanged;

    private void OnHotkeyChanged()
    {
        HotkeyChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetHotkey(Keys key)
    {
        HotkeyValue = key;
        Text = key == Keys.None ? "" : KeyToString(key);
    }

    private static string KeyToString(Keys key)
    {
        var parts = new List<string>();
        if (key.HasFlag(Keys.Control)) parts.Add("Ctrl");
        if (key.HasFlag(Keys.Alt)) parts.Add("Alt");
        if (key.HasFlag(Keys.Shift)) parts.Add("Shift");

        var keyCode = key & Keys.KeyCode;
        parts.Add(keyCode.ToString());
        return string.Join("+", parts);
    }
}
