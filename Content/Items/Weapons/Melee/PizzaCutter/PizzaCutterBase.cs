
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
	public abstract class PizzaCutterBase : ModItem
	{
		public static void LoadPreHardmodePizzaCutters(Mod epicosity)
		{
		}
		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 32;
			item.value = Item.sellPrice(0, 0, 1, 0);
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.melee = true;
			item.autoReuse = false;
			item.UseSound = SoundID.Item1;
		}
	}
}
