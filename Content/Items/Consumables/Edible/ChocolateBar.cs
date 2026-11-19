using Epicosity.Items.CraftingMaterials;
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

namespace Epicosity.Items.Consumable.Edible
{
    public class ChocolateBar : ModItem
    {
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("'With or without nuts?'");
		}
		public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.consumable = true;
            item.useTurn = true;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.UseSound = SoundID.Item2;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<ChocolateWaterBucket>(), 1);
            recipe.AddTile(TileID.Hellforge);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
