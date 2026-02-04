using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class MacroRecorder
{
    public List<MacroAction> Actions = new List<MacroAction>();
    private Stopwatch timer = new Stopwatch();

    private IntPtr keyboardHook;
    private IntPtr mouseHook;

    private const int WH_KEYBOARD_LL = 13;
    private const int WH_MOUSE_LL = 14;

    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_MOUSEMOVE = 0x0200;

    private LowLevelKeyboardProc keyboardProc;
    private LowLevelMouseProc mouseProc;

    public void Start()
    {
        Actions.Clear();
        timer.Restart();

        keyboardProc = KeyboardCallback;
        mouseProc = MouseCallback;

        keyboardHook = SetHook(keyboardProc, WH_KEYBOARD_LL);
        mouseHook = SetHook(mouseProc, WH_MOUSE_LL);
    }

    public void Stop()
    {
        timer.Stop();
        UnhookWindowsHookEx(keyboardHook);
        UnhookWindowsHookEx(mouseHook);
    }

  
    private IntPtr KeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int delay = (int)timer.ElapsedMilliseconds;
            timer.Restart();

            int vkCode = Marshal.ReadInt32(lParam);

            if ((int)wParam == WM_KEYDOWN)
                Actions.Add(new MacroAction { Type = MacroActionType.KeyDown, KeyCode = vkCode, DelayMs = delay });

            if ((int)wParam == WM_KEYUP)
                Actions.Add(new MacroAction { Type = MacroActionType.KeyUp, KeyCode = vkCode, DelayMs = delay });
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    private IntPtr MouseCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int delay = (int)timer.ElapsedMilliseconds;
            timer.Restart();

            MSLLHOOKSTRUCT info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            if ((int)wParam == WM_MOUSEMOVE)
                Actions.Add(new MacroAction { Type = MacroActionType.MouseMove, X = info.pt.x, Y = info.pt.y, DelayMs = delay });

            if ((int)wParam == WM_LBUTTONDOWN)
                Actions.Add(new MacroAction { Type = MacroActionType.MouseDown, DelayMs = delay });

            if ((int)wParam == WM_LBUTTONUP)
                Actions.Add(new MacroAction { Type = MacroActionType.MouseUp, DelayMs = delay });
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr SetHook(Delegate proc, int hookType)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(hookType, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    [DllImport("user32.dll")] static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("kernel32.dll")] static extern IntPtr GetModuleHandle(string lpModuleName);

    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
    }

    private struct POINT
    {
        public int x;
        public int y;
    }
}
