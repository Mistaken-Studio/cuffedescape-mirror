﻿// -----------------------------------------------------------------------
// <copyright file="CuffedEscapeHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Features;
using Mistaken.API.Components;
using Mistaken.API.Diagnostics;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CuffedEscape
{
    internal class CuffedEscapeHandler : Module
    {
        public CuffedEscapeHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => "CuffedEscape";

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.ChangingRole += this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Escaped)
                return;

            if (ev.Player.HasItem(ItemType.KeycardO5))
                ev.Player.RemoveItem(ev.Player.Items.First(i => i.Type == ItemType.KeycardChaosInsurgency || i.Type == ItemType.KeycardNTFLieutenant || i.Type == ItemType.KeycardFacilityManager || i.Type == ItemType.KeycardNTFCommander));

            if (ev.Player.HasItem(ItemType.KeycardChaosInsurgency) || ev.Player.HasItem(ItemType.KeycardNTFLieutenant) || ev.Player.HasItem(ItemType.KeycardFacilityManager) || ev.Player.HasItem(ItemType.KeycardNTFCommander))
            {
                ev.Player.RemoveItem(ev.Player.Items.First(i => i.Type == ItemType.KeycardChaosInsurgency || i.Type == ItemType.KeycardNTFLieutenant || i.Type == ItemType.KeycardFacilityManager || i.Type == ItemType.KeycardNTFCommander));
                ev.Player.AddItem(ItemType.KeycardO5);
            }
        }

        private void Server_RoundStarted()
        {
            // Escape
            void Handler(Player player)
            {
                if (!player.IsCuffed)
                    return;
                switch (player.Team)
                {
                    case Team.MTF:
                        Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.ChaosInsurgency, 2);
                        player.SetRole(RoleType.ChaosConscript, Exiled.API.Enums.SpawnReason.Escaped, true);
                        break;
                    case Team.CHI:
                        Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox, 2);
                        player.SetRole(RoleType.NtfSpecialist, Exiled.API.Enums.SpawnReason.Escaped, true);
                        break;
                    default:
                        return;
                }

                RLogger.Log("ESCAPE", "ESCAPE", $"{player.PlayerToString()} has escaped");
            }

            InRange.Spawn(new Vector3(179.5f, 990, 32.5f), new Vector3(13, 20, 19), Handler);
            InRange.Spawn(new Vector3(174.5f, 990, 37), new Vector3(21, 20, 10), Handler);
        }
    }
}
