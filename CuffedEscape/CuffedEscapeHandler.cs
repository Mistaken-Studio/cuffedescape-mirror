// -----------------------------------------------------------------------
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
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Reason != Exiled.API.Enums.SpawnReason.Escaped)
                return;

            if (ev.Player.HasItem(ItemType.KeycardFacilityManager) || ev.Player.HasItem(ItemType.KeycardContainmentEngineer))
            {
                foreach (var item in ev.Player.Items.Where(i => i.Type == ItemType.KeycardChaosInsurgency || i.Type == ItemType.KeycardNTFLieutenant || i.Type == ItemType.KeycardFacilityManager || i.Type == ItemType.KeycardNTFCommander || i.Type == ItemType.KeycardContainmentEngineer).ToArray())
                {
                    ev.Player.RemoveItem(item);
                    ev.Player.AddItem(ItemType.KeycardO5);
                }
            }
        }

        private void Server_RoundStarted()
        {
            // Escape
            void Handler(Player player)
            {
                if (!player.IsCuffed)
                    return;
                switch (player.Role.Team)
                {
                    case Team.MTF:
                        Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.ChaosInsurgency, 2);
                        player.SetRole(RoleType.ChaosConscript, Exiled.API.Enums.SpawnReason.Escaped, true);
                        player.RemoveHandcuffs();
                        player.AddItem(ItemType.KeycardChaosInsurgency);
                        player.AddItem(ItemType.GunLogicer);
                        player.AddItem(ItemType.GunRevolver);
                        player.AddItem(ItemType.ArmorHeavy);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.Adrenaline);
                        player.AddItem(ItemType.Medkit);
                        player.Ammo[ItemType.Ammo762x39] = 200;
                        player.Ammo[ItemType.Ammo44cal] = 48;
                        break;
                    case Team.CHI:
                        Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox, 2);
                        player.SetRole(RoleType.NtfSpecialist, Exiled.API.Enums.SpawnReason.Escaped, true);
                        player.RemoveHandcuffs();
                        player.AddItem(ItemType.KeycardNTFLieutenant);
                        player.AddItem(ItemType.GunE11SR);
                        player.AddItem(ItemType.GunRevolver);
                        player.AddItem(ItemType.ArmorHeavy);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.Adrenaline);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Radio);
                        player.Ammo[ItemType.Ammo556x45] = 160;
                        player.Ammo[ItemType.Ammo44cal] = 48;
                        player.Ammo[ItemType.Ammo9x19] = 40;
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
