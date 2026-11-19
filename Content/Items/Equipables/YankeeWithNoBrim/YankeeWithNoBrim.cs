using Terraria.ModLoader;

namespace Epicosity.Items.Equipables.Vanity
{
	[AutoloadEquip(EquipType.Head)]
	public class YankeeWithNoBrim : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Yankee With No Brim");
			Tooltip.SetDefault("'Let me go around this hat! Woah!'");
		}
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 22;
			item.vanity = true;
		}
	}
}
