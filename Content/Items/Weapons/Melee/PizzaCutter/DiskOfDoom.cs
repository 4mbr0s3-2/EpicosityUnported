using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
	public class DiskOfDoom : PizzaCutterBase
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.damage = 15;
			item.useAnimation = 6;
			item.useTime = 6;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DemoniteBar, 7);
			recipe.AddIngredient(ItemID.Wood, 3);
			recipe.AddTile(ItemID.WorkBench);
			recipe.SetResult(item.type);
			recipe.AddRecipe();
		}
	}
}
