using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epicosity.Items.Weapons.Ranged
{
	public class BruhGatlingGun : ModItem
	{
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips[1].text = Regex.Replace(tooltips[1].text, "ranged", "decreasing ranged");
		}
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Rapidly sends penetrating, high-speed soundwaves");
		}

		public override void SetDefaults()
		{
			item.damage = 80;
			item.ranged = true;
			item.width = 40;
			item.height = 20;
			item.useTime = 7;
			item.useAnimation = 7;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = 10000;
			item.rare = ItemRarityID.LightRed;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BruhFastSFX"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 20f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BruhSFX").WithVolume(0.33f));
			for (int x = 0; x < 10; x++)
			{
				Dust.NewDust(player.Center, 0, 0, 15, speedX / 2, speedY / 2, 150, Color.DarkRed, 1f);
			}
			return true;
		}
		public override void AddRecipes() 
		{
			//ModRecipe recipe = new ModRecipe(mod);
			//recipe.AddIngredient(ItemID.DirtBlock, 10);
			//recipe.AddTile(TileID.WorkBenches);
			//recipe.SetResult(this);
			//recipe.AddRecipe();
		}
	}
}