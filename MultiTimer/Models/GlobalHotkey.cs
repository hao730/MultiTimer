using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiTimer;

/// <summary>
/// 管理全域熱鍵的註冊與觸發。
/// </summary>
public class GlobalHotkey : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public const int WM_HOTKEY = 0x0312;

    private readonly IntPtr _hWnd;
    private int _nextId = 1;
    private readonly Dictionary<int, Action> _callbacks = new();

    public GlobalHotkey(IntPtr hWnd)
    {
        _hWnd = hWnd;
    }

    /// <summary>
    /// 註冊一個全域熱鍵，回傳 id（用於之後取消）。失敗回傳 -1。
    /// </summary>
    public int Register(Keys key, Action callback)
    {
        var (modifiers, vk) = SplitKey(key);
        int id = _nextId++;
        if (RegisterHotKey(_hWnd, id, modifiers, vk))
        {
            _callbacks[id] = callback;
            return id;
        }
        return -1;
    }

    /// <summary>
    /// 取消註冊指定 id 的熱鍵。
    /// </summary>
    public void Unregister(int id)
    {
        if (id > 0)
        {
            UnregisterHotKey(_hWnd, id);
            _callbacks.Remove(id);
        }
    }

    /// <summary>
    /// 在 Form 的 WndProc 中呼叫，處理 WM_HOTKEY 訊息。
    /// </summary>
    public bool ProcessMessage(Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();
            if (_callbacks.TryGetValue(id, out var cb))
            {
                cb.Invoke();
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        foreach (var id in _callbacks.Keys.ToList())
        {
            UnregisterHotKey(_hWnd, id);
        }
        _callbacks.Clear();
    }

    private static (uint modifiers, uint vk) SplitKey(Keys key)
    {
        uint modifiers = 0;
        if (key.HasFlag(Keys.Control)) modifiers |= 0x0002;
        if (key.HasFlag(Keys.Alt)) modifiers |= 0x0001;
        if (key.HasFlag(Keys.Shift)) modifiers |= 0x0004;

        uint vk = (uint)(key & Keys.KeyCode);
        return (modifiers, vk);
    }
}
