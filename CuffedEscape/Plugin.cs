using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Mistaken.CuffedEscape;

internal sealed class Plugin
{
    public static Plugin Instance { get; private set; }

    [PluginConfig]
    public Config Config;

    [PluginPriority(LoadPriority.Medium)]
    [PluginEntryPoint("Cuffed Escape", "1.0.0", "Plugin that allows escape of cuffed CI and NTF", "Mistaken Devs")]
    private void Load()
    {
        Instance = this;
        EventManager.RegisterEvents(this);
    }

    [PluginUnload]
    private void Unload()
    {
        EventManager.UnregisterEvents(this);
    }

    [PluginEvent(ServerEventType.WaitingForPlayers)]
    private void OnWaitingForPlayers()
        => Server.Instance.GameObject.AddComponent<CuffedEscapeHandler>();
}

