
using IL.Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Magic
{
    public class SeedaboutStaff : ModItem
    {
        public class SeedaboutStaffSeed : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Seed");
            }
            public override void SetDefaults()
            {
                ProjectileID.Sets.TrailingMode[projectile.type] = 2;
                ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
                projectile.width = 20;
                projectile.height = 12;
                projectile.aiStyle = 0;
                projectile.friendly = true;
                projectile.tileCollide = true;
            }
            public override void AI()
            {
                projectile.rotation -= 0.1f;
                Lighting.AddLight(projectile.Center, new Vector3(0.2f, 0.2f, 0.2f));
            }
            public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                Texture2D texture = ModContent.GetTexture(Texture);
                spriteBatch.Draw
                (
                    texture,
                    new Vector2
                    (
                        projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                        projectile.position.Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 2f
                    ),
                    null,
                    Color.White,
                    projectile.rotation,
                    texture.Size() * 0.5f,
                    projectile.scale,
                    SpriteEffects.None,
                    0f
                );
                for (int oldPosIndex = 0; oldPosIndex < projectile.oldPos.Length; oldPosIndex++)
                {
                    spriteBatch.Draw
                    (
                        texture,
                        new Vector2
                        (
                            projectile.oldPos[oldPosIndex].X - Main.screenPosition.X + projectile.width * 0.5f,
                            projectile.oldPos[oldPosIndex].Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 2f
                        ),
                        null,
                        Color.White * 0.4f * (1.0f - (float)oldPosIndex / (float)projectile.oldPos.Length),
                        projectile.oldRot[oldPosIndex],
                        texture.Size() * 0.5f,
                        projectile.scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
        public class SeedaboutStaffRing : ModProjectile
        {
            int time = 0;
            int useTime = 30; // Defines how long it takes until shoot
            int currentSeed = 0;
            int numOfSeeds = 10;
            List<SeedaboutStaffSeed> seedList = new List<SeedaboutStaffSeed>();
            public void SetUseTime(int useTime)
            {
                this.useTime = useTime;
            }
            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Seedabout Staff Ring");
                Main.projFrames[projectile.type] = 3;
            }
            public override void SendExtraAI(BinaryWriter writer)
            {
                writer.Write(useTime);
            }
            public override void ReceiveExtraAI(BinaryReader reader)
            {
                useTime = reader.ReadInt32();
            }
            public override void SetDefaults()
            {
                projectile.width = 182;
                projectile.height = 182;
                projectile.tileCollide = false;
                projectile.friendly = true;
                projectile.ranged = true;
                projectile.scale = 1f;
            }
            public override bool CanDamage()
            {
                return false;
            }
            public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                Texture2D texture = ModContent.GetTexture(Texture);
                spriteBatch.Draw
                (
                    texture,
                    new Vector2
                    (
                        projectile.position.X - Main.screenPosition.X + texture.Width / 2 * projectile.scale,
                        projectile.position.Y - Main.screenPosition.Y + texture.Height / 6 * projectile.scale
                    ),
                    new Rectangle(0, texture.Height / Main.projFrames[projectile.type] * projectile.frame, texture.Width, texture.Height / Main.projFrames[projectile.type]),
                    Color.White,
                    projectile.rotation,
                    new Vector2(texture.Width, texture.Height / 3) / 2,
                    projectile.scale,
                    SpriteEffects.None,
                    0f
                );
                return true;
            }
            public override void AI()
            {
                time++;
                Lighting.AddLight(projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));
                if (time < useTime)
                {
                    if (time % 2 == 0) projectile.frame++;
                    if (projectile.frame > 1)
                    {
                        projectile.frame = 0;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Magic/SeedaboutStaffRing").WithPitchVariance(0.0001f), projectile.Center);
                    }
                }  
                else
                {
                    projectile.frame = 2;
                    if (time < useTime + 40 || currentSeed < numOfSeeds)
                    {
                        if (time % 2 == 0) // Every other frame
                        {
                            if (currentSeed < numOfSeeds)
                            {
                                currentSeed++;
                                if (projectile.owner == Main.myPlayer)
                                {
                                    int seedIndex = Projectile.NewProjectile(projectile.Center + new Vector2(92, 0).RotatedBy((float)Math.PI * 2 * currentSeed / numOfSeeds), Vector2.Zero, ModContent.ProjectileType<SeedaboutStaffSeed>(), projectile.damage, projectile.knockBack, projectile.owner);
                                    seedList.Add((SeedaboutStaffSeed)Main.projectile[seedIndex].modProjectile);
                                }
                            }
                            else
                            {
                                if (currentSeed == numOfSeeds) // Do once
                                {
                                    currentSeed++;
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Magic/SeedaboutStaffRingTarget"), projectile.Center);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Magic/SeedaboutStaffRingShoot"), projectile.Center);
                        }
                        if (projectile.owner == Main.myPlayer)
                        {
                            foreach (SeedaboutStaffSeed seed in seedList) // Make all seeds go to point
                            {
                                if (seed.projectile.active && seed.projectile.type == ModContent.ProjectileType<SeedaboutStaffSeed>())
                                {
                                    Vector2 targetVelocity = projectile.Center - seed.projectile.Center;
                                    targetVelocity.Normalize();
                                    seed.projectile.velocity = targetVelocity * 20;
                                    seed.projectile.netUpdate = true; // Multiplayer support
                                }
                            }
                        }
                        projectile.Kill();
                    }
                }
            }
        }
        public override void UseStyle(Player player)
        {
            player.itemLocation.X = -player.itemWidth / 2 + player.Center.X + player.direction * -18;
            player.itemLocation.Y -= 6;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seedabout Staff");
        }
        //public override string Texture => "Terraria/Item_1";
        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 54;
            item.damage = 90;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = ModContent.ProjectileType<SeedaboutStaffRing>();
            item.magic = true;
            item.mana = 3;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int projIndex = Projectile.NewProjectile(Main.MouseWorld, new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-1, 1)), type, damage, knockBack, player.whoAmI, 0f, 0f);
            Main.projectile[projIndex].Center = Main.MouseWorld;
            if (Main.projectile[projIndex].modProjectile.GetType() == typeof(SeedaboutStaffRing))
            {
                ((SeedaboutStaffRing)Main.projectile[projIndex].modProjectile).SetUseTime(item.useTime);
                Main.projectile[projIndex].netUpdate2 = true;
            }
            return false;
        }
    }
}
