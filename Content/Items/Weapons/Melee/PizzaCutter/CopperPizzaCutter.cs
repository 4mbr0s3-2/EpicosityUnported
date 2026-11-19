using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
	public class CopperPizzaCutter : PizzaCutterBase
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.damage = 3;
			item.useAnimation = 10;
			item.useTime = 10;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CopperBar, 3);
			recipe.AddIngredient(ItemID.Wood, 3);
			recipe.AddTile(ItemID.WorkBench);
			recipe.SetResult(item.type);
			recipe.AddRecipe();
		}
	}
}
