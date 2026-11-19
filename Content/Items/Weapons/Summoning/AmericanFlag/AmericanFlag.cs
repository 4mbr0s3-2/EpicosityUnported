using Epicosity.Buffs.Summons;
using Epicosity.Items.Tiles.Ores;
using Epicosity.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Summoning
{
	public class AmericanFlag : ModItem
    {
        // TODO: Make a drop / add more reasonable recipe
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Item summonItem = item.Clone();
			summonItem.melee = false;
			summonItem.summon = true;
			tooltips.Find((tooltip1) => tooltip1.Name == "Damage").text += "\n" + Main.LocalPlayer.GetWeaponDamage(summonItem) + " summon damage";
		}
		public override void SetStaticDefaults()
        {
			bool independenceDay = DateTime.Today.Day == 4 && DateTime.Today.Month == 7;
			DisplayName.SetDefault("American Flag");
            Tooltip.SetDefault("'Comin' again to save the motherfrickin' day, yeah!'\nRight click to summon bald eagle" + (independenceDay ? "\n[c/ffff00:Happy Independence Day!]" : ""));
        }
        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 70;
            item.melee = true;
            item.maxStack = 1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.holdStyle = ItemHoldStyleID.HoldingOut;
            item.damage = 76;
            //item.UseSound = SoundID.Item1;
            item.buffType = BuffType<BaldEagleBuff>();
        }
        public override void HoldStyle(Player player)
        {
            Vector2 origin = new Vector2(32, 4);
            player.itemRotation = -0.5f * player.direction;
            player.itemLocation.Y = player.Center.Y + 8 - origin.RotatedBy(player.itemRotation).Y * player.direction;
            player.itemLocation.X = player.Center.X + (8 - origin.RotatedBy(player.itemRotation).X) * player.direction;
        }
        public override void UseStyle(Player player)
        {
            Vector2 origin = new Vector2(32, 4);
            player.itemRotation += 0.5f * player.direction;
            player.itemLocation.Y = player.Center.Y + 8 - origin.RotatedBy(player.itemRotation).Y * player.direction;
            player.itemLocation.X = player.Center.X + (8 - origin.RotatedBy(player.itemRotation).X) * player.direction;
        }
        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.AddBuff(item.buffType, 2);
				Main.PlaySound(SoundID.Item44);
				Projectile.NewProjectile(player.position, Vector2.Zero, ModContent.ProjectileType<BaldEagleMinion>(), 77, 0, player.whoAmI);
            }
			else
			{
				Main.PlaySound(SoundID.Item1);
			}
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
	}
}
