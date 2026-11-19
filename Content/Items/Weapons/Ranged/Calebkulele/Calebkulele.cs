using Epicosity.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Epicosity.Items.Weapons.Ranged
{
	public class Calebkulele : ModItem
	{
		int useNumber = 0;
		public override TagCompound Save()
		{
			var tag = new TagCompound {
				{"useNumber" , nameof(useNumber)}
			};
			return tag;
		}
		public override void Load(TagCompound tag)
		{
			useNumber = tag.GetAsInt(nameof(useNumber));
		}
		public override void SetDefaults()
		{
			SetDefaults(item);
		}
		public static void SetDefaults(Item item)
		{
			item.ranged = true;
			item.width = 50;
			item.height = 50;
			item.damage = 40;
			item.shoot = ModContent.ProjectileType<CalebkuleleSmall>();
			item.knockBack = 0.1f;
			item.useTime = 10;
			item.useAnimation = 50;
			item.useStyle = Terraria.ID.ItemUseStyleID.HoldingOut;
			item.shootSpeed = 8;
			item.rare = Items.Rarity.EpicosityItemRarity.WorkInProgress;
		}
		public override Vector2? HoldoutOffset()
		{
			return HoldoutOffset(item);
		}
		public static Vector2? HoldoutOffset(Item item)
		{
			return new Vector2(-6, -6);
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return Shoot(ref useNumber, item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
		public static bool Shoot(ref int useNumber, Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			//Main.NewText(useNumber);
			if (player.itemAnimation == player.itemAnimationMax - 1)
			{
				useNumber++;
				useNumber %= 3;
				Utilities.SoundUtil.PlaySoundOnAllClients(Epicosity.inst.GetLegacySoundSlot(SoundType.Item, $"Sounds/Item/Weapons/Ranged/Calebkulele{(useNumber + 2) % 3}"), (int)player.position.X, (int)player.position.Y);
			}
			//Main.NewText(useNumber);
			if (useNumber == 0)
			{
				type = ModContent.ProjectileType<CalebkuleleBig>();
				damage *= 7;
				speedX *= 0.8f;
				speedY *= 0.8f;
				knockBack *= 2;
				//CombatText.NewText(new Rectangle((int)player.Top.X - 15, (int)player.Top.Y - 15, 15, 15), new Color(232, 71, 46), "Yes", true);
				//return true;
				if (player.itemAnimation != player.itemAnimationMax - 1)
				{
					return false;
				}
			}
			//CombatText.NewText(new Rectangle((int)player.Top.X - 15, (int)player.Top.Y + 15, 15, 15), new Color(255, 111, 79), "Now");
			return true;
		}
	}
}
