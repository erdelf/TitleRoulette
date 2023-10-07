using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Collections.Generic;

namespace TitleRoulette
{
    using Dalamud.Plugin.Services;

    internal class Service
    {
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; set; } = null!;
        [PluginService] public static IChatGui Chat { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
        [PluginService] public static IDataManager DataManager { get; set; } = null!;

        [PluginService] public static IGameInteropProvider GameInteropProvider { get; set; } = null!;

        public static GameFunctions GameFunctions { get; set; } = null!;
        public static Configuration Configuration { get; set; } = null!;
        public static WindowSystem WindowSystem { get; } = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);
        public static PluginCommandManager<Plugin> PluginCommandManager { get; set; } = null!;
        public static List<Title> Titles { get; set; } = new List<Title>();
        public static ushort MaxTitleId { get; set; } = 676;
    }
}
