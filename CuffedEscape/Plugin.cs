using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

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
        new CuffedEscapeHandler();
    }

    [PluginUnload]
    private void Unload()
    {
    }
}

