using Epicosity.Buffs.Summons;
using Epicosity.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Summoning
{
	public class RoombaRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Roomba Remote");
			Tooltip.SetDefault("Summons a roomba with a knife duct-taped to it");
        }
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 16;
            item.summon = true;
            item.maxStack = 1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 60;
			item.shoot = ModContent.ProjectileType<KilleroombaMinion>();
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			player.AddBuff(ModContent.BuffType<KilleroombaBuff>(), 2);
			return true;
		}
	}
}
