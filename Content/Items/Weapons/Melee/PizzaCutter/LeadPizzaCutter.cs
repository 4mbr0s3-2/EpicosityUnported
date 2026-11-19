using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
	public class LeadPizzaCutter : PizzaCutterBase
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.damage = 5;
			item.useAnimation = 9;
			item.useTime = 9;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LeadBar, 5);
			recipe.AddIngredient(ItemID.Wood, 3);
			recipe.AddTile(ItemID.WorkBench);
			recipe.SetResult(item.type);
			recipe.AddRecipe();
		}
	}
}
