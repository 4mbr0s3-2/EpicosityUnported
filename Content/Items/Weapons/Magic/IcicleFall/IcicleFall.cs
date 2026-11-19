using Epicosity.Items.Rarity;
using Epicosity.Projectiles.Weapons.Magic;
using Epicosity.Systems.Item;
using Epicosity.UI.SpellCard;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Epicosity.Items.Weapons.Magic
{
	public class IcicleFall : SpecialItem
	{
		public override void SetDefaults()
		{
			item.damage = 99;
			item.magic = true;
			item.width = 34;
			item.height = 42;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 10;
			item.useTime = 10;
			item.rare = EpicosityItemRarity.WorkInProgress;
		}
		public override void SetStaticSpecialDefaults()
		{
			DisplayName.SetDefault("Ice Sign \"Icicle Fall\"");
			Tooltip.SetDefault("Spell Card 04");
		}
		public override bool CanUseItem(Player player)
		{
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.type == ModContent.ProjectileType<NineBall>() && proj.owner == player.whoAmI)
				{
					return false;
				}
			}
			return true;
		}
		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SpellCardUIState.SpellCardInterface.SetState(new IceSignIcicleFallUIState());
				Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<NineBall>(), item.damage, 0, player.whoAmI);
			}
			return true;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string damageText = tooltips.Find(tooltip => tooltip.Name == "Damage").text;
			tooltips.Find(tooltip => tooltip.Name == "Damage").text = damageText.Replace("9", "⑨");
		}
	}
}
