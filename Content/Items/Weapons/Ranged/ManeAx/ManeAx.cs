using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Ranged
{
	class ManeAx : ModItem
    {
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Item summonItem = item.Clone();
			summonItem.melee = true;
			summonItem.summon = false;
			tooltips.Find((tooltip1) => tooltip1.Name == "Damage").text = Main.LocalPlayer.GetWeaponDamage(summonItem) + " melee damage";
			summonItem.melee = false;
			summonItem.summon = true;
			tooltips.Find((tooltip1) => tooltip1.Name == "Damage").text += "\n" + Main.LocalPlayer.GetWeaponDamage(summonItem) + " ranged damage";
		}
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Right click (Interact) to send a Rude Buster");
		}
		public override void SetDefaults()
        {
			item.damage = 150;
			item.melee = true;
			item.width = 50;
			item.height = 42;
			item.useTime = 40;
			item.useAnimation = 40;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}
		public override bool OnlyShootOnSwing => true;
		public override bool CanUseItem(Player player)
		{
			item.shootSpeed = 30;
			item.UseSound = null;
			if (player.altFunctionUse == 2) {
				item.useTime = 60;
				item.useAnimation = 60;
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/RudeBuster").WithPitchVariance(0);
				item.ranged = true;
				item.melee = false;
				item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.RudeBuster>();
			}
			else
			{
				item.useTime = 30;
				item.useAnimation = 30;
				item.ranged = false;
				item.melee = true;
				item.shoot = ProjectileID.None;
			}
			return true;
		}
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	}
}
