using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Epicosity.Projectiles.Weapons.Ranged;
using Epicosity.Items.Rarity;

namespace Epicosity.Items.Weapons.Ranged
{
	public class RodOfDiscordNitro : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rod of Discord Nitro");
			Tooltip.SetDefault("Sends three notifications across enemies rapidly");
		}
		public override void SetDefaults()
		{
			item.damage = 100;
			item.ranged = true;
			item.width = 40;
			item.height = 20;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = 10000;
			item.rare = EpicosityItemRarity.WorkInProgress;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/DiscordMessage");
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<DiscordMessage>();
			item.shootSpeed = 15f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			for (int x = 0; x < 10; x++)
			{
				Dust.NewDust(item.position, 10, 10, DustID.Electric, item.velocity.X * 0.25f, item.velocity.Y * 0.25f, 150, Microsoft.Xna.Framework.Color.Aqua, 0.7f);
			}
			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
		public override void AddRecipes() 
		{
			//ModRecipe recipe = new ModRecipe(mod);
			//recipe.AddIngredient(ItemID.DirtBlock, 10);
			//recipe.AddTile(TileID.WorkBenches);
			//recipe.SetResult(this);
			//recipe.AddRecipe();
		}
	}
}