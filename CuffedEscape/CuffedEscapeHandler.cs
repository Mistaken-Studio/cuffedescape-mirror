using System.Linq;
using InventorySystem.Disarming;
using Mistaken.API.Components;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;
using Utils.Networking;

namespace Mistaken.CuffedEscape;

internal sealed class CuffedEscapeHandler
{
    public CuffedEscapeHandler()
    {
        EventManager.RegisterEvents(this);
    }

    ~CuffedEscapeHandler()
    {
        EventManager.UnregisterEvents(this);
    }

    [PluginEvent(ServerEventType.RoundStart)]
    private void OnRoundStart()
    {
        static void Handler(Player player)
        {
            if (!player.ReferenceHub.inventory.IsDisarmed())
                return;

            switch (player.Team)
            {
                case Team.FoundationForces:
                    {
                        Respawning.RespawnTokensManager.GrantTokens(Respawning.SpawnableTeamType.ChaosInsurgency, 2);
                        player.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Escaped);
                        DisarmedPlayers.Entries.RemoveAt(DisarmedPlayers.Entries.FindIndex(x => x.DisarmedPlayer == player.NetworkId));
                        new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                        player.AddItem(ItemType.KeycardChaosInsurgency);
                        player.AddItem(ItemType.GunAK);
                        player.AddItem(ItemType.GunRevolver);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.Medkit);
                        player.AmmoBag[ItemType.Ammo762x39] = 120;
                        player.AmmoBag[ItemType.Ammo44cal] = 36;
                        player.ReferenceHub.inventory.SendAmmoNextFrame = true;
                        break;
                    }

                case Team.ChaosInsurgency:
                    {
                        Respawning.RespawnTokensManager.GrantTokens(Respawning.SpawnableTeamType.NineTailedFox, 2);
                        player.SetRole(RoleTypeId.NtfSpecialist, RoleChangeReason.Escaped);
                        DisarmedPlayers.Entries.RemoveAt(DisarmedPlayers.Entries.FindIndex(x => x.DisarmedPlayer == player.NetworkId));
                        new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                        player.AddItem(ItemType.KeycardNTFLieutenant);
                        player.AddItem(ItemType.GunE11SR);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.Radio);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.Medkit);
                        player.AmmoBag[ItemType.Ammo556x45] = 120;
                        player.AmmoBag[ItemType.Ammo9x19] = 40;
                        player.ReferenceHub.inventory.SendAmmoNextFrame = true;
                        break;
                    }

                default:
                    return;
            }

            // RLogger.Log("ESCAPE", "ESCAPE", $"{player.PlayerToString()} has escaped");
        }

        InRange.Spawn(new Vector3(179.5f, 990, 32.5f), new Vector3(13, 20, 19), Handler);
        InRange.Spawn(new Vector3(174.5f, 990, 37), new Vector3(21, 20, 10), Handler);
    }

    [PluginEvent(ServerEventType.PlayerChangeRole)]
    private void OnPlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        if (reason != RoleChangeReason.Escaped)
            return;

        if (player.Items.Any(x => x.ItemTypeId == ItemType.KeycardFacilityManager || x.ItemTypeId == ItemType.KeycardContainmentEngineer))
        {
            foreach (var item in player.Items.Where(x =>
            x.ItemTypeId == ItemType.KeycardChaosInsurgency ||
            x.ItemTypeId == ItemType.KeycardNTFLieutenant ||
            x.ItemTypeId == ItemType.KeycardFacilityManager ||
            x.ItemTypeId == ItemType.KeycardNTFCommander ||
            x.ItemTypeId == ItemType.KeycardContainmentEngineer).ToArray())
            {
                player.ReferenceHub.inventory.UserInventory.Items.Remove(item.ItemSerial);
                player.ReferenceHub.inventory.SendItemsNextFrame = true;
                player.AddItem(ItemType.KeycardO5);
            }
        }
    }
}
