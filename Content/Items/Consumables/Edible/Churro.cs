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
    public class Churro : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Sweet, crispy, delicious...'");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.maxStack = 30;
            item.consumable = true;
            item.width = 10;
            item.height = 10;
            item.buffType = BuffID.WellFed;
            item.buffTime = 72000;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 5, 0);
        }
        public override void AddRecipes()
        {
            //ModRecipe recipe = new ModRecipe(mod);
            //recipe.AddIngredient(ModContent.ItemType<ChocolateWaterBucket>(), 1);
            //recipe.AddTile(TileID.Hellforge);
            //recipe.SetResult(this, 1);
            //recipe.AddRecipe();
        }
    }
}
