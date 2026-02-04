public enum MacroActionType
{
    KeyDown,
    KeyUp,
    MouseDown,
    MouseUp,
    MouseMove,
    Delay
}

public class MacroAction
{
    public MacroActionType Type;
    public int KeyCode; //keysssbordss
    public int X;
    public int Y;
    public int DelayMs;
}
