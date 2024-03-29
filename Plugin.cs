﻿using Dalamud.Plugin;
using System;
using System.Linq;
using TitleRoulette.Attributes;

namespace TitleRoulette
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Title Roulette";

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();
            Service.GameFunctions = new GameFunctions();
            Service.Configuration = (Configuration)pluginInterface.GetPluginConfig()
                          ?? pluginInterface.Create<Configuration>();
            InitializeTitles();
            var window = pluginInterface.Create<PluginWindow>();
            if (window is not null)
            {
                Service.WindowSystem.AddWindow(window);
            }

            Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigWindow;
            Service.PluginInterface.UiBuilder.Draw         += Service.WindowSystem.Draw;
            Service.PluginCommandManager                   =  new PluginCommandManager<Plugin>(this, Service.CommandManager);
            Service.ClientState.TerritoryChanged           += _ => RandomTitleEvent();
        }

        private void InitializeTitles()
        {
            foreach (var title in Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Title>())
            {
                if (string.IsNullOrEmpty(title.Masculine) || string.IsNullOrEmpty(title.Feminine))
                    continue;

                Service.Titles.Add(new Title { Id = (ushort)title.RowId, MasculineName = title.Masculine, FeminineName = title.Feminine, IsPrefix = title.IsPrefix });
            }
            Service.MaxTitleId = Service.Titles.Max(x => x.Id);
        }

        private void RandomTitleEvent()
        {
            SetRandomTitleFromGroup(Service.Configuration.randomTitleGroup);
        }

        [Command("/ptitle")]
        [HelpMessage("Picks a random title - optionally specified by a group.")]
        public void PickRandomTitle(string command, string args)
        {
            if (string.IsNullOrEmpty(args))
                args = "default";

            var groups = Service.Configuration.GetCurrentCharacterGroups();
            var group = groups.FirstOrDefault(x => args.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
            if (group == null)
            {
                Service.Chat.PrintError($"[Title Roulette] Group '{args}' does not exist.");
                return;
            }
            SetRandomTitleFromGroup(group);
        }

        public void SetRandomTitleFromGroup(Configuration.TitleGroup group)
        {
            int titleCount = group.Titles.Count;
            if (titleCount == 0)
            {
                Service.Chat.PrintError($"[Title Roulette] Can't pick a random title from group '{group.Name}' as it is empty.");
                return;
            }

            ushort titleId = group.Titles.ToList()[new Random().Next(titleCount)];
            Service.GameFunctions.SetTitle(titleId);
        }

        [Command("/ptitlecfg")]
        [HelpMessage("Opens the configuration window")]
        public void OpenConfigWindow(string command, string args) => OpenConfigWindow();

        private void OpenConfigWindow()
        {
            var window = Service.WindowSystem.Windows.FirstOrDefault(t => t is PluginWindow);
            if (window != null)
                window.IsOpen = !window.IsOpen;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Service.PluginCommandManager.Dispose();
            Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigWindow;
            Service.PluginInterface.UiBuilder.Draw -= Service.WindowSystem.Draw;
            Service.WindowSystem.RemoveAllWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}