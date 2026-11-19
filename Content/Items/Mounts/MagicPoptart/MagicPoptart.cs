using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Epicosity.Mounts;

namespace Epicosity.Items.Mounts
{
    class MagicPoptart : ModItem
    {
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'The sleeve is dated 4/5/2011...'\nSummons a rideable Nyan Cat");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 30;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 30000;
			item.rare = ItemRarityID.LightPurple;
			item.scale = 1;
			//item.UseSound = SoundID.Item79;
			item.noMelee = true;
			item.mountType = ModContent.MountType<NyanCatMount>();
		}
	}
}
