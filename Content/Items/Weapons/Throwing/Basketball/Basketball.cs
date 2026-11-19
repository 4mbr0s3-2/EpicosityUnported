using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class BasketballProjectile : ModProjectile
    {
        public bool hostile = false;
        public int hits = 0;
        public override string Texture => "Epicosity/Items/Weapons/Throwing/Basketball";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Basketball");
        }
        public override void SetDefaults()
        {
            projectile.scale = 1f;
            projectile.thrown = true;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.width = 32;
            projectile.height = 32;
            projectile.frame = Main.rand.Next(52);
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.velocity.Y += 0.3f;
            
            Vector4 slopeCollision = Collision.SlopeCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.position = slopeCollision.XY();
            projectile.velocity = slopeCollision.ZW();

            Vector2 oppositeVector = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Vector2 differenceVector = (oppositeVector - projectile.velocity);
            CheckCollision(differenceVector);
            if (projectile.wet)
            {
                projectile.velocity.Y *= 0.9f;
                projectile.velocity.Y -= 0.5f;
                projectile.velocity.X *= 0.9f;
            }
            if (Main.mouseRight && Main.mouseRightRelease && projectile.getRect().Contains(Main.MouseWorld.ToPoint()) && Main.LocalPlayer.Distance(projectile.Center) <= 100)
			{
                projectile.velocity *= -0.2f;
                projectile.velocity += Main.LocalPlayer.velocity;
            }
        }
        public void CheckCollision(Vector2 differenceVector)
        {
            if (differenceVector != Vector2.Zero) // Prevents NaNs
            {
                differenceVector.Normalize();
            }
            if (differenceVector != Vector2.Zero) // On hit
            {
                hits++;
                if (hits > 3 && hostile)
                {
                    for (int x = 0; x < 10; x++) Dust.NewDust(projectile.position, projectile.width, projectile.height, 1);
                    projectile.Kill();
                }
                if (differenceVector.X != 0) // The X has changed as a result of collision, so inverse it
                {
                    projectile.velocity.X *= -1;
                }
                if (differenceVector.Y != 0) // The Y has changed as a result of collision, so inverse it
                {
                    projectile.velocity.Y *= -1;
                }
                projectile.velocity *= 0.8f;
                if (projectile.velocity.Length() > 2)
                {
                    Terraria.Audio.LegacySoundStyle basketballSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Throwing/BasketballBounce");
                    Main.PlaySound(basketballSound.SoundId, (int)projectile.Center.X, (int)projectile.Center.Y, basketballSound.Style, projectile.velocity.Length() / 10f, 0);
                }
            }
            if (projectile.velocity.Length() <= 0.1f)
            {
                projectile.Kill();
                Item.NewItem(new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), ModContent.ItemType<Basketball>());
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 oppositeVector = Collision.AnyCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Vector2 differenceVector = (oppositeVector - projectile.velocity);
            CheckCollision(differenceVector);
        }
    }
    public class Basketball : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'I'm ballin'!'");
        }
        public override void SetDefaults()
        {
            item.thrown = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 32;
            item.height = 32;
            item.shoot = ModContent.ProjectileType<BasketballProjectile>();
            item.damage = 130;
            item.shootSpeed = 20;
            item.consumable = true;
            item.maxStack = 999;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (item.wet)
            {
                item.velocity.X *= 0.9f;
                item.velocity.Y *= 0.9f;
                item.velocity.Y -= 0.2f;
            }
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speed = Math.Min((player.Center - Main.MouseWorld).Length() / Main.screenHeight * 2, 1);
            speedX *= speed;
            speedY *= speed;
            return true;
        }
    }
}
