using Epicosity.Projectiles.Fun;
using Epicosity.Systems.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Epicosity.Network.CommonPlayerControlsSync;

namespace Epicosity.Items.Fun
{
	public class ModPlayerPrimazela : ModPlayer
	{
		public float fade = 0;
		public SoundEffectInstance primaSound;
	}
	public class Primazela : SpecialItem
	{	
		public override void SetDefaults()
		{
			item.width = 76;
			item.height = 36;
			item.rare = ItemRarityID.Lime;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTime = 5;
			item.useAnimation = 1;
			item.autoReuse = true;
			item.shootSpeed = 17;
			item.shoot = ModContent.ProjectileType<PrimazelaLight>();
		}
		public override void PostUpdatePlayer(Player player)
		{
			ModPlayerSyncedMouseControls controls = player.GetModPlayer<ModPlayerSyncedMouseControls>();
			if (controls.leftMouse && player.itemAnimation > 0)
			{
				Vector2 playerToCursor = (controls.mouseWorld - player.Center);
				playerToCursor.Normalize();
				// Change direction only locally
				if (player.whoAmI == Main.myPlayer)
				{
					if (controls.mouseWorld.X >= player.Center.X)
					{
						player.ChangeDir(1);
					}
					else
					{
						player.ChangeDir(-1);
					}
				}
				player.itemRotation = playerToCursor.ToRotation() + (player.direction == -1 ? MathHelper.Pi : 0);
				player.bodyFrame.Y = player.bodyFrame.Height * Utilities.BodyFrameCalculator.GetBodyFrameFromRotation(playerToCursor.ToRotation());
			}
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			ModPlayerSyncedMouseControls controls = player.GetModPlayer<ModPlayerSyncedMouseControls>();
			float pitch = (controls.mouseWorld - player.MountedCenter).Length() / controls.screenSize.Y;
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, pitch * 255);
			return false;
		}
		public override void PreUpdatePlayer(Player player)
		{
			ModPlayerPrimazela primazela = player.GetModPlayer<ModPlayerPrimazela>();
			ModPlayerSyncedMouseControls controls = player.GetModPlayer<ModPlayerSyncedMouseControls>();
			if (controls.leftMouse && player.itemAnimation > 0 && Main.hasFocus)
			{
				// Instantiate sound
				controls.localShouldSyncMousePos = true;
				if (primazela.primaSound == null)
				{
					primazela.primaSound = ModContent.GetSound($"{typeof(Epicosity).Name}/Sounds/Item/Fun/Primazela").CreateInstance();
					primazela.primaSound.IsLooped = true;
					Main.PlaySoundInstance(primazela.primaSound);
				}
				// If starting to fade, fade back
				if (primazela.fade > 0)
				{
					primazela.fade -= 0.1f;
				}
				UpdatePrimaSoundVolume(player);
			}
			else
			{
				if (primazela.primaSound != null)
				{
					StopPrimaSound(player);
				}
			}
		}
		void UpdatePrimaSoundVolume(Player player)
		{
			ModPlayerSyncedMouseControls controls = player.GetModPlayer<ModPlayerSyncedMouseControls>();
			ModPlayerPrimazela primazela = player.GetModPlayer<ModPlayerPrimazela>();
			Utilities.SoundUtil.SetSoundInstancePanAndVolume(primazela.primaSound, (int)player.MountedCenter.X, (int)player.MountedCenter.Y);
			// Pitch modulation
			float pitch = (controls.mouseWorld - player.MountedCenter).Length() / controls.screenSize.Y;
			if (pitch >= 1)
			{
				pitch = 1;
			}
			primazela.primaSound.Pitch = pitch * 2 - 1;
			float volume = 1 - primazela.fade;
			if (volume < 0)
			{
				volume = 0;
			}
			else if (volume > 1)
			{
				volume = 1;
			}
			primazela.primaSound.Volume *= volume;
		}
		void StopPrimaSound(Player player)
		{
			ModPlayerPrimazela primazela = player.GetModPlayer<ModPlayerPrimazela>();
			primazela.fade += 0.1f;
			UpdatePrimaSoundVolume(player);
			if (primazela.fade >= 1)
			{
				primazela.fade = 0;
				primazela.primaSound.Stop();
				primazela.primaSound = null; // Dispose
			}
		}
		public override void UnequipItem(Player player)
		{
			ModPlayerPrimazela primazela = player.GetModPlayer<ModPlayerPrimazela>();
			if (primazela.primaSound != null)
			{
				primazela.fade = 0;
				primazela.primaSound.Stop();
				primazela.primaSound = null; // Dispose
			}
		}
	}
}
