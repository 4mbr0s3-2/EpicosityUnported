using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Throwing
{
    public class SolitaireDeck : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wes's Deck");
        }
        public override void SetDefaults()
        {
            item.thrown = true;
            item.noMelee = true;
            item.damage = 150;
            item.shoot = mod.ProjectileType("SolitaireCard");
            item.shootSpeed = 10;
            item.UseSound = SoundID.Item1;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.maxStack = 999;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position.Y = player.position.Y - 102;
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
    }
}
