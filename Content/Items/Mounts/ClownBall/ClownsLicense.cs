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
    class ClownsLicense : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clown's License");
			Tooltip.SetDefault("'You dropped this'\nSummons a rideable clown ball for your insufferable jests\nFaster with Clown set");
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
			item.mountType = ModContent.MountType<ClownBallMount>();
		}
	}
}
