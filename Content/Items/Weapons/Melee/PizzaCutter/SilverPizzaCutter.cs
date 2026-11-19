using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
	public class SilverPizzaCutter : PizzaCutterBase
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.damage = 10;
			item.useAnimation = 7;
			item.useTime = 7;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SilverBar, 6);
			recipe.AddIngredient(ItemID.Wood, 3);
			recipe.AddTile(ItemID.WorkBench);
			recipe.SetResult(item.type);
			recipe.AddRecipe();
		}
	}
}
