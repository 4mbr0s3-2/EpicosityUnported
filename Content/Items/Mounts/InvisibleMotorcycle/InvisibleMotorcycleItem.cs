using Terraria.ModLoader;
using Terraria.ID;
using Epicosity.Mounts;

namespace Epicosity.Items.Mounts
{
	public class InvisibleMotorcycle : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Invisible Motorcycle");
			Tooltip.SetDefault("'Ding ding!'\nSummons an invisible motorcycle");
		}

		public override void SetDefaults()
		{
			item.width = 44;
			item.height = 28;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = Terraria.Item.sellPrice(0, 10, 0, 0);
			item.rare = ItemRarityID.LightPurple;
			item.scale = 1;
			//item.UseSound = SoundID.Item79;
			item.noMelee = true;
			item.mountType = ModContent.MountType<InvisibleMotorcycleMount>();
		}
	}
}
