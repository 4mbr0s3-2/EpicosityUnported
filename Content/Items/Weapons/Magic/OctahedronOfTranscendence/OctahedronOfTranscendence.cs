using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epicosity.Items.Rarity;
using Epicosity.Projectiles.Weapons.Ranged;

namespace Epicosity.Items.Weapons.Magic
{
	public class OctahedronOfTranscendence : ModItem
	{
		//public string obfuscation = "a̸b̵d̷e̸f̵g̸h̷i̵j̶k̶l̴m̶n̴o̵p̸q̵r̵s̵t̷u̵v̴w̷x̴y̷z̵";
		public int rotationTime = 0;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'The almighty shanpe from the ΠБe145.21th dimension'\nSummons a powerful octahedron shield");
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			//if (Regex.Match(tooltips[1].text, "", RegexOptions.IgnoreCase).Success)
			//{
			//	return;
			//}
			//tooltips[1].text = Regex.Replace(tooltips[1].text, "magic", "transcendental magic");
			tooltips[1].text = "100 magic damage";
		}
		public override void SetDefaults()
		{
			item.damage = 100;
			//item.ranged = true;
			item.width = 20;
			item.height = 25;
			item.useTime = 10;
			item.useAnimation = 10;
			item.magic = true;
			item.mana = 10;
			item.rare = EpicosityItemRarity.PulsatingBlackAndWhite;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = Item.sellPrice(3, 0, 0, 0);
			item.UseSound = SoundID.Item24;
			item.autoReuse = false;
			item.channel = true;
			item.shoot = ModContent.ProjectileType<TranscendentLight>();
		}
		//public override bool UseItem(Player player)
		//{
		//	mod.Logger.Info("AAAAAAA");
		//	return base.UseItem(player);
		//}
	}
}