using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Epicosity.Items.CraftingMaterials;
using Epicosity.Items.CraftingMaterials.Essences;
using Epicosity.Items.Tiles.Ores;
using Epicosity.Items.Rarity;
using Epicosity.Projectiles.Weapons.Ranged;

namespace Epicosity.Items.Weapons.Ranged
{
	public class RodOfDiscord : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rod of Discord�");
			Tooltip.SetDefault("Sends three notifications across enemies");
		}
		public override void SetDefaults()
		{
			item.damage = 60;
			item.ranged = true;
			item.width = 40;
			item.height = 20;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = 10000;
			item.rare = EpicosityItemRarity.WorkInProgress;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/DiscordMessage");
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<DiscordMessage>();
			item.shootSpeed = 10f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Blurplessence>(), 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}