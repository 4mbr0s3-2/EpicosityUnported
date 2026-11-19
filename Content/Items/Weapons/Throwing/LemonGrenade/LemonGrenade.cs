using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Throwing
{
	public class LemonGrenade : ModItem
    {
		public override void SetStaticDefaults()
		{
			string tooltip = "'Make life take the lemons back!'\nA quick, small explosion that will destroy most tiles";
			Tooltip.SetDefault(tooltip);
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Epicosity.loadedCompatibilityMods.HasFlag(Epicosity.CompatibilityMods.TerrariaOverhaul))
			{
                TooltipLine tooltipLine = new TooltipLine(mod, "Overhaul Bonus", "Overhaul Bonus: Explosion is incendiary")
                {
                    overrideColor = Color.OrangeRed
                };
                tooltips.Add(tooltipLine);
			}
		}
		public override void SetDefaults()
        {
			item.useAnimation = 30;
			item.useTime = 30;
			item.knockBack = 8;
			item.damage = 60;
			item.consumable = true;
			item.width = 24;
			item.height = 32;
			item.shootSpeed = 5;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 10000;
			item.UseSound = SoundID.Item1;
			item.shoot = ModContent.ProjectileType<Projectiles.Throwables.LemonGrenade>();
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<CraftingMaterials.Lemon>(), 1);
			recipe.AddIngredient(ItemID.Bomb, 1);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
