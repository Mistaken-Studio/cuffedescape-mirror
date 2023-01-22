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
    private static void SetupWeapon(ReferenceHub player, Firearm firearm)
    {
        if (!AttachmentsServerHandler.PlayerPreferences.TryGetValue(player, out var value) || !value.TryGetValue(firearm.ItemTypeId, out uint attachments))
            attachments = AttachmentsUtils.GetRandomAttachmentsCode(firearm.ItemTypeId);

        FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
        if (firearm.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
            firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;

        firearm.ApplyAttachmentsCode(attachments, true);
        firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearmStatusFlags, firearm.GetCurrentAttachmentsCode());
    }

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
                    player.inventory.ServerAddItem(ItemType.KeycardChaosInsurgency);
                    SetupWeapon(player, player.inventory.ServerAddItem(ItemType.GunAK) as Firearm);
                    SetupWeapon(player, player.inventory.ServerAddItem(ItemType.GunRevolver) as Firearm);
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
                    player.inventory.ServerAddItem(ItemType.KeycardNTFLieutenant);
                    SetupWeapon(player, player.inventory.ServerAddItem(ItemType.GunE11SR) as Firearm);
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
