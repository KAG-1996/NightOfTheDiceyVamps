using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary> Customized Debug Tool. </summary>
/// <remarks> Use this to differentiate from other debug logs. </remarks>
public static class ZDebug
{
    public static bool _isDebug => Debug.isDebugBuild;
    const string _Color = HUE.WHITE;

#if ZDEBUG
    const DebugType _type = DebugType.ERROR;
#else
    const DebugType _type = DebugType.LOG;
#endif

    public static void Log(object msg, DebugType type = _type)
    {
        if (!_isDebug) return;
        switch (type)
        {
            case DebugType.LOG:     Debug.Log(DevDebug(msg, _Color)); break;
            case DebugType.WARNING: Debug.LogWarning(DevDebug(msg, _Color)); break;
            case DebugType.ERROR:   Debug.LogError(DevDebug(msg, _Color)); break;
        }
    }
    public static void Log(object msg, string color, DebugType type = _type)
    {
        if (!_isDebug) return;
        switch (type)
        {
            case DebugType.LOG:     Debug.Log(DevDebug(msg, color)); break;
            case DebugType.WARNING: Debug.LogWarning(DevDebug(msg, color)); break;
            case DebugType.ERROR:   Debug.LogError(DevDebug(msg, color)); break;
        }
    }
    public static void Log(object msg, string color, UnityEngine.Object obj, DebugType type = _type)
    {
        if (!_isDebug) return;
        switch (type)
        {
            case DebugType.LOG:     Debug.Log(DevDebug(msg + obj.name, color)); break;
            case DebugType.WARNING: Debug.LogWarning(DevDebug(msg + obj.name, color)); break;
            case DebugType.ERROR:   Debug.LogError(DevDebug(msg + obj.name, color)); break;
        }
    }
    public static void Log(object msgA, string colorA, object msgB, string colorB, DebugType type = _type)
    {
        if (!_isDebug) return;
        switch (type)
        {
            case DebugType.LOG:     Debug.Log($"{DevDebug(msgA, colorA)} / {DevDebug(msgB, colorB)}"); break;
            case DebugType.WARNING: Debug.LogWarning($"{DevDebug(msgA, colorA)} / {DevDebug(msgB, colorB)}"); break;
            case DebugType.ERROR:   Debug.LogError($"{DevDebug(msgA, colorA)} / {DevDebug(msgB, colorB)}"); break;
        }
    }
    private static string DevDebug(object msg, string color = _Color) => $"<color={color}> - {msg} - </color>";
}
public enum DebugType { LOG, WARNING, ERROR }
public class HUE
{
    public const string
        BLACK       = "#000000ff",
        BLUE        = "#0000ffff",
        BROWN       = "#a52a2aff",
        CYAN        = "#00ffffff",
        DARKBLUE    = "#0000a0ff",
        GREEN       = "#008000ff",
        GREY        = "#808080ff",
        LIGHTBLUE   = "#add8e6ff",
        LIME        = "#00ff00ff",
        MAGENTA     = "#ff00ffff",
        MAROON      = "#800000ff",
        NAVY        = "#000080ff",
        OLIVE       = "#808000ff",
        ORANGE      = "#ffa500ff",
        PURPLE      = "#800080ff",
        RED         = "#ff0000ff",
        SILVER      = "#c0c0c0ff",
        TEAL        = "#008080ff",
        WHITE       = "#ffffffff",
        YELLOW      = "#ffff00ff";
}