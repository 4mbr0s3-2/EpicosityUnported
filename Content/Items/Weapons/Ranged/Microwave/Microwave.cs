using Epicosity.Items.Tiles.Ores;
using Epicosity.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Ranged
{
    public class Microwave : ModItem
    {
        SoundEffectInstance microwaveHum = null;
        bool playingSound = false;
        bool prevHeld = false;
        public class MicrowaveItemProjectile : ModProjectile
        {
            public bool open = false;
            public override string Texture => "Epicosity/Items/Weapons/Ranged/MicrowaveHeld";
            public override void SetStaticDefaults()
            {
                Main.projFrames[projectile.type] = 6;
            }
            public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                return false;
            }
            public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                SpriteEffects flip = SpriteEffects.None;
                if (Main.player[projectile.owner].direction == -1)
                {
                    flip = SpriteEffects.FlipHorizontally;
                }
                spriteBatch.Draw
                (
                    ModContent.GetTexture("Epicosity/Items/Weapons/Ranged/MicrowaveHeld"),
                    new Rectangle((int)projectile.position.X - (int)Main.screenPosition.X, (int)projectile.position.Y - (int)Main.screenPosition.Y, 40, 24),
                    new Rectangle(0, 24 * projectile.frame, 40, 24),
                    lightColor,
                    projectile.rotation,
                    new Vector2(0, 0),
                    flip,
                    0
                );
                // TODO: Glow mask
                spriteBatch.Draw
                (
                    ModContent.GetTexture("Epicosity/Items/Weapons/Ranged/MicrowaveHeld"),
                    new Rectangle((int)projectile.position.X - (int)Main.screenPosition.X, (int)projectile.position.Y - (int)Main.screenPosition.Y, 40, 24),
                    new Rectangle(0, 24 * projectile.frame, 40, 24),
                    Color.White,
                    projectile.rotation,
                    new Vector2(0, 0),
                    flip,
                    0
                );
            }
            public override void SetDefaults()
            {
                projectile.tileCollide = false;
                projectile.width = 40;
                projectile.height = 24;
                projectile.penetrate = -1;
                projectile.friendly = true;
                projectile.ownerHitCheck = true;
            }
            public override void AI()
            {
                if (projectile.owner < 0)
                {
                    return;
                }
                Player player = Main.player[projectile.owner];
                player.heldProj = projectile.whoAmI;
                projectile.Center = player.MountedCenter + new Vector2(10 * player.direction, 0);
                open = player.controlUseItem;
                projectile.timeLeft = 5;

                if (player.HeldItem.type != ModContent.ItemType<Microwave>())
                {
                    projectile.Kill();
                    return;
                }

                projectile.frameCounter++;
                if (projectile.frameCounter > 5)
                {
                    projectile.frameCounter = 0;
                    
                    if (open)
                    {
                        projectile.frame++;
                    }
                    else
                    {
                        projectile.frame--;
                        if (projectile.frame < 0)
                        {
                            projectile.frame = 0;
                        }
                    }
                    if (projectile.frame > 2)
                    {
                        projectile.frame = 2;
                    }
                }
            }
        }
        public class MicrowaveArrow : ModProjectile
        {
            int time = 0;
            bool dead = false;
            public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                if (projectile.oldPos.Length > 5 && projectile.oldPos[4] != Vector2.Zero)
                {
                    Color color = new Color(255, 142, 85) * ((255 - projectile.alpha) / 255f);
                    if (projectile.frame == 1)
                    {
                        color = new Color(255, 73, 56) * ((255 - projectile.alpha) / 255f);
                    }
                    Utils.DrawLine(spriteBatch, projectile.oldPos[4], projectile.position, color, color, 2);
                }
                spriteBatch.Draw
                (
                    ModContent.GetTexture("Epicosity/Items/Weapons/Ranged/MicrowaveArrowHead"), 
                    new Rectangle((int)projectile.position.X - (int)Main.screenPosition.X, (int)projectile.position.Y - (int)Main.screenPosition.Y, 10, 6), 
                    new Rectangle(0, 6 * projectile.frame, 10, 6), 
                    Color.White * ((255 - projectile.alpha) / 255f), 
                    projectile.rotation, 
                    new Vector2(5, 0), 
                    SpriteEffects.None, 
                    0
                );
            }
            public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                return false;
            }
            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Microwave");
            }
            public override void SetDefaults()
            {
                projectile.aiStyle = 0;
                projectile.width = 10;
                projectile.height = 6;
                projectile.friendly = true;
                projectile.tileCollide = false;
                projectile.penetrate = -1;
                ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
                ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            }
            public override void AI()
            {
                time++;
                projectile.rotation = projectile.velocity.ToRotation() + Microsoft.Xna.Framework.MathHelper.PiOver2;
                Vector2 collisionVector = Collision.TileCollision(projectile.position, projectile.velocity, 5, 5) - projectile.velocity;
                if (time > 20)
                {
                    dead = true;
                }
                if (collisionVector != Vector2.Zero)
                {
                    dead = true;
                }
                if (dead)
                {
                    projectile.velocity *= 0.8f;
                    projectile.alpha += 10;
                    if (projectile.alpha >= 255)
                    {
                        projectile.Kill();
                    }
                }
                projectile.frameCounter++;
                if (projectile.frameCounter > 5)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 1)
                    {
                        projectile.frame = 0;
                    }
                }
            }
        }
        public override void SetDefaults()
        {
            item.useStyle = Utilities.CustomItemUseStyleID.Custom;
            item.noUseGraphic = true;
            item.useTime = 10;
            item.useAnimation = 10;
            item.width = 30;
            item.height = 16;
            item.ranged = true;
            item.shoot = ModContent.ProjectileType<MicrowaveArrow>();
            item.shootSpeed = 10;
            item.autoReuse = true;
            item.damage = 20;
            item.channel = true;
        }
        public override bool HoldItemFrame(Player player)
        {
            player.bodyFrame.Y = player.bodyFrame.Height * 3;
            return true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speedVector = new Vector2(speedX, speedY);
            float speed = speedVector.Length();
            speedVector.Normalize();
            speedVector = speedVector.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
            speedVector *= speed;
            speedX = speedVector.X;
            speedY = speedVector.Y;
            return true;
        }
        public override void UpdateInventory(Player player)
        {
            // All clients will hear this
            if (player.channel && player.HeldItem == item)
            {
                if (microwaveHum == null || microwaveHum.State == SoundState.Stopped)
                {
                    microwaveHum = Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Ranged/MicrowaveHum"), player.position);
                    playingSound = true;
                }
            }
            else
            {
                if (playingSound)
                {
                    playingSound = false;
                    if (microwaveHum != null) microwaveHum.Stop();
                }
            }
            if (player.HeldItem == item)
            {
                if (prevHeld != player.channel)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Ranged/MicrowaveOpen"), player.position);
                }
                prevHeld = player.channel;

                // Light
                if (player.channel)
                {
                    Lighting.AddLight(player.Center, new Vector3(120, 44, 0) / 255f);
                }
            }
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 5);
            recipe.AddIngredient(ItemID.IronBar, 10);
        }
    }
    public class ModPlayerMicrowave : ModPlayer
    {
        public override bool CloneNewInstances => true;
        public int microwaveProj = -1;
        public override void PostUpdate()
        {
            if (player.HeldItem.type == ModContent.ItemType<Microwave>())
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    bool microwaveInWorld = false;
                    foreach (Projectile proj in Main.projectile)
                    {
                        // Local player only
                        if (proj.type == ModContent.ProjectileType<Microwave.MicrowaveItemProjectile>() && proj.owner == Main.myPlayer && proj.active)
                        {
                            microwaveInWorld = true;
                        }
                    }
                    if (!microwaveInWorld)
                    {
                        microwaveProj = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Microwave.MicrowaveItemProjectile>(), 0, 0, player.whoAmI);
                    }
                }
            }
        }
    }
}
