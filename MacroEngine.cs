using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

public class MacroEngine
{
    private bool looping;
    private Thread macroThread;
    private bool running;
    private List<MacroAction> currentActions; // Track the currently running macro

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    const uint KEYEVENTF_KEYUP = 0x0002;
    const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    const uint MOUSEEVENTF_LEFTUP = 0x0004;
    const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    const uint MOUSEEVENTF_MOVE = 0x0001;

    public bool IsRunning => running;

    public void Start(Action macroLogic)
    {
        if (running) return;

        running = true;
        macroThread = new Thread(() =>
        {
            while (running)
            {
                macroLogic();
            }
        });

        macroThread.IsBackground = true;
        macroThread.Start();
    }

    // Play a macro and track the action list
    public void PlayLoop(List<MacroAction> actions)
    {
        currentActions = actions;
        looping = true;

        Start(() =>
        {
            while (looping && running)
            {
                foreach (var act in actions)
                {
                    if (!running || !looping) break;

                    Delay(act.DelayMs);

                    switch (act.Type)
                    {
                        case MacroActionType.KeyDown:
                            keybd_event((byte)act.KeyCode, 0, 0, UIntPtr.Zero);
                            break;

                        case MacroActionType.KeyUp:
                            keybd_event((byte)act.KeyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                            break;

                        case MacroActionType.MouseDown:
                            LeftClick();
                            break;

                        case MacroActionType.MouseMove:
                            MoveMouse(act.X, act.Y);
                            break;
                    }
                }
            }

            running = false;
            currentActions = null;
            looping = false;
        });
    }

    public void StopLoop(List<MacroAction> actions)
    {
        if (currentActions == actions)
        {
            looping = false;
            running = false;
            if (macroThread != null && macroThread.IsAlive)
                macroThread.Join();

            currentActions = null;
        }
    }

    public void Play(List<MacroAction> actions)
    {
        currentActions = actions;

        Start(() =>
        {
            foreach (var act in actions)
            {
                if (!running) break; // Stop immediately if Stop() is called

                Delay(act.DelayMs);

                switch (act.Type)
                {
                    case MacroActionType.KeyDown:
                        keybd_event((byte)act.KeyCode, 0, 0, UIntPtr.Zero);
                        break;

                    case MacroActionType.KeyUp:
                        keybd_event((byte)act.KeyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                        break;

                    case MacroActionType.MouseDown:
                        LeftClick();
                        break;

                    case MacroActionType.MouseMove:
                        MoveMouse(act.X, act.Y);
                        break;
                }
            }

            running = false;
            currentActions = null;
        });
    }

    // Stop a specific macro
    public void Stop(List<MacroAction> actions)
    {
        if (currentActions == actions)
        {
            running = false;
            if (macroThread != null && macroThread.IsAlive)
                macroThread.Join();

            currentActions = null;
        }
    }

    // Optional: global stop
    public void Stop()
    {
        running = false;
        if (macroThread != null && macroThread.IsAlive)
            macroThread.Join();

        currentActions = null;
    }

    // Input helpers
    public void PressKey(byte key, int holdMs = 40)
    {
        keybd_event(key, 0, 0, UIntPtr.Zero);
        Thread.Sleep(holdMs);
        keybd_event(key, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    public void LeftClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(10);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }

    public void RightClick()
    {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(10);
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
    }

    public void MoveMouse(int x, int y)
    {
        mouse_event(MOUSEEVENTF_MOVE, x, y, 0, UIntPtr.Zero);
    }

    public void Delay(int ms)
    {
        Thread.Sleep(ms);
    }
}
