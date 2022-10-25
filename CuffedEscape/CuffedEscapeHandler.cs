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
    internal sealed class CuffedEscapeHandler : Module
    {
        public CuffedEscapeHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(CuffedEscapeHandler);

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
                foreach (var item in ev.Player.Items.Where(x =>
                x.Type == ItemType.KeycardChaosInsurgency ||
                x.Type == ItemType.KeycardNTFLieutenant ||
                x.Type == ItemType.KeycardFacilityManager ||
                x.Type == ItemType.KeycardNTFCommander ||
                x.Type == ItemType.KeycardContainmentEngineer).ToArray())
                {
                    ev.Player.RemoveItem(item);
                    ev.Player.AddItem(ItemType.KeycardO5);
                }
            }
        }

        private void Server_RoundStarted()
        {
            // Escape
            static void Handler(Player player)
            {
                if (!player.IsCuffed)
                    return;

                switch (player.Role.Team)
                {
                    case Team.MTF:
                        {
                            Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.ChaosInsurgency, 2);
                            player.SetRole(RoleType.ChaosConscript, Exiled.API.Enums.SpawnReason.Escaped, true);
                            player.RemoveHandcuffs();
                            player.AddItem(ItemType.KeycardChaosInsurgency);
                            player.AddItem(ItemType.GunAK);
                            player.AddItem(ItemType.GunRevolver);
                            player.AddItem(ItemType.ArmorCombat);
                            player.AddItem(ItemType.GrenadeHE);
                            player.AddItem(ItemType.Medkit);
                            player.Ammo[ItemType.Ammo762x39] = 120;
                            player.Ammo[ItemType.Ammo44cal] = 36;
                            break;
                        }

                    case Team.CHI:
                        {
                            Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox, 2);
                            player.SetRole(RoleType.NtfSpecialist, Exiled.API.Enums.SpawnReason.Escaped, true);
                            player.RemoveHandcuffs();
                            player.AddItem(ItemType.KeycardNTFLieutenant);
                            player.AddItem(ItemType.GunE11SR);
                            player.AddItem(ItemType.ArmorCombat);
                            player.AddItem(ItemType.Radio);
                            player.AddItem(ItemType.GrenadeHE);
                            player.AddItem(ItemType.Medkit);
                            player.Ammo[ItemType.Ammo556x45] = 120;
                            player.Ammo[ItemType.Ammo9x19] = 40;
                            break;
                        }

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
