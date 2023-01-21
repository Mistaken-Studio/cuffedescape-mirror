using InventorySystem;
using InventorySystem.Disarming;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using UnityEngine;

namespace Mistaken.CuffedEscape;

internal sealed class CuffedEscapeHandler : MonoBehaviour
{
    private void Update()
    {
        foreach (var player in ReferenceHub.AllHubs)
        {
            if (player.roleManager.CurrentRole is not HumanRole role)
                continue;

            if ((role.FpcModule.Position - Escape.WorldPos).sqrMagnitude > 156.5f)
                continue;

            if (role.ActiveTime < 10f)
                continue;

            if (!player.inventory.IsDisarmed())
                continue;

            try
            {
                OnEscape(player, role);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }

    private void OnEscape(ReferenceHub player, HumanRole role)
    {
        switch (role.Team)
        {
            case Team.FoundationForces:
                {
                    Respawning.RespawnTokensManager.GrantTokens(Respawning.SpawnableTeamType.ChaosInsurgency, 2);
                    player.roleManager.ServerSetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Escaped, RoleSpawnFlags.None);
                    // new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                    player.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
                    var firearm = player.inventory.ServerAddItem(ItemType.GunAK) as Firearm;
                    firearm.Status = new FirearmStatus(
                        0,
                        FirearmStatusFlags.MagazineInserted,
                        AttachmentsServerHandler.PlayerPreferences[player][ItemType.GunAK]);
                    firearm._refillAmmo = true;
                    firearm.ServerConfirmAcqusition();
                    firearm = player.inventory.ServerAddItem(ItemType.GunRevolver) as Firearm;
                    firearm.Status = new FirearmStatus(
                        firearm.AmmoManagerModule.MaxAmmo,
                        FirearmStatusFlags.MagazineInserted,
                        AttachmentsServerHandler.PlayerPreferences[player][ItemType.GunRevolver]);
                    firearm._refillAmmo = true;
                    firearm.ServerConfirmAcqusition();
                    player.inventory.ServerAddItem(ItemType.ArmorCombat);
                    player.inventory.ServerAddItem(ItemType.GrenadeHE);
                    player.inventory.ServerAddItem(ItemType.Medkit);
                    player.inventory.ServerAddAmmo(ItemType.Ammo762x39, 120);
                    player.inventory.ServerAddAmmo(ItemType.Ammo44cal, 36);
                    EventManager.ExecuteEvent(ServerEventType.PlayerEscape, player, role.RoleTypeId);
                    break;
                }

            case Team.ChaosInsurgency:
                {
                    Respawning.RespawnTokensManager.GrantTokens(Respawning.SpawnableTeamType.NineTailedFox, 2);
                    player.roleManager.ServerSetRole(RoleTypeId.NtfSpecialist, RoleChangeReason.Escaped, RoleSpawnFlags.None);
                    // new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                    player.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
                    var firearm = player.inventory.ServerAddItem(ItemType.GunE11SR) as Firearm;
                    firearm.Status = new FirearmStatus(
                        0,
                        FirearmStatusFlags.MagazineInserted,
                        AttachmentsServerHandler.PlayerPreferences[player][ItemType.GunE11SR]);
                    firearm._refillAmmo = true;
                    firearm.ServerConfirmAcqusition();
                    player.inventory.ServerAddItem(ItemType.ArmorCombat);
                    player.inventory.ServerAddItem(ItemType.Radio);
                    player.inventory.ServerAddItem(ItemType.GrenadeHE);
                    player.inventory.ServerAddItem(ItemType.Medkit);
                    player.inventory.ServerAddAmmo(ItemType.Ammo556x45, 120);
                    player.inventory.ServerAddAmmo(ItemType.Ammo9x19, 40);
                    EventManager.ExecuteEvent(ServerEventType.PlayerEscape, player, role.RoleTypeId);
                    break;
                }

            default:
                return;
        }
    }
}
