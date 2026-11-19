using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epicosity.Items.CraftingMaterials;
using Epicosity.Items.Tiles.Ores;
using Epicosity.Items.CraftingMaterials.Essences;

namespace Epicosity.Items.Weapons.Ranged
{
	public class BruhBlaster : ModItem
	{
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips[1].text = Regex.Replace(tooltips[1].text, "ranged", "decreasing ranged");
		}
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Sends a penetrating soundwave");
		}

		public override void SetDefaults()
		{
			item.damage = 20;
			item.ranged = true;
			item.width = 40;
			item.height = 20;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BruhSFX"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 10f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BruhSFX").WithVolume(0.33f));
			for (int x = 0; x < 10; x++)
			{
				Dust.NewDust(player.Center, 0, 0, 15, speedX, speedY, 150, new Color(54, 110, 255), 1f);
			}
			return true;
		}
		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 1);
			recipe.AddIngredient(ItemID.IronBar, 3);
			recipe.AddIngredient(ModContent.ItemType<Bruhssence>(), 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 1);
			recipe.AddIngredient(ItemID.LeadBar, 3);
			recipe.AddIngredient(ModContent.ItemType<Bruhssence>(), 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}