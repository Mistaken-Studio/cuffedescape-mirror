// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.Updater.API.Config;

namespace Mistaken.CuffedEscape
{
    internal sealed class PluginHandler : Plugin<Config>, IAutoUpdateablePlugin
    {
        public override string Author => "Mistaken Devs";

        public override string Name => "CuffedEscape";

        public override string Prefix => "MCuffedEscape";

        public override PluginPriority Priority => PluginPriority.Default;

        public override Version RequiredExiledVersion => new(5, 2, 2);

        public AutoUpdateConfig AutoUpdateConfig => new()
        {
            Type = SourceType.GITLAB,
            Url = "https://git.mistaken.pl/api/v4/projects/67",
        };

        public override void OnEnabled()
        {
            Instance = this;

            new CuffedEscapeHandler(this);

            API.Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
