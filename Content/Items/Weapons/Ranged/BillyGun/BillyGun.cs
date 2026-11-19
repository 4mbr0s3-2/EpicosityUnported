using Epicosity.Invasions;
using Epicosity.Projectiles.Enemy;
using Epicosity.Projectiles.Weapons.Ranged;
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
	public class BillyGunEffect : GlobalNPC
    {
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.type == ModContent.ProjectileType<BulletBill>() && npc.life <= 0 && !DummyInvasion.PreHardmodeInvaders.ContainsKey(npc.type))
            {
                // TODO: Skip spawning gore
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Color npcColor = npc.GetColor(Color.White);
                    // TODO: Sync with multiplayer or just copy type and source rect in projectile itself
                    int projIndex = Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.Next(-3, 3) * 1.5f, -7), ModContent.ProjectileType<FakeNPCDummy>(), 0, 0);
                    FakeNPCDummy dummy = (FakeNPCDummy)Main.projectile[projIndex].modProjectile;
                    dummy.dummyAnimation = FakeNPCDummy.DummyAnimation.Knockout;
                    dummy.npcRect = npc.getRect();
                    dummy.spriteDirection = -npc.spriteDirection;
                    dummy.copyNPCType = npc.type;
                    dummy.drawSourceRect = npc.frame;
                    //dummy.npcColor = npcColor != default(Color) ? npcColor : Color.White;
                    Main.projectile[projIndex].scale = npc.scale;
                    Main.projectile[projIndex].netUpdate2 = true;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPCs/BillyGunKill"), npc.Center);
                }
            }
        }
    }
    public class BillyGunHoldProjectile : ModProjectile
    {
        public int ShootTimer = 0;
        float rotation = 0;
        public override string Texture => "Epicosity/Items/Weapons/Ranged/BillyGun";
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 3;
        }
        public override void AI()
        {
            if (projectile.owner >= 0 && Main.player[projectile.owner].HeldItem.type == ModContent.ItemType<BillyGun>() && Main.player[projectile.owner].channel)
            {
                Player player = Main.player[projectile.owner];
                projectile.timeLeft = 3;
                projectile.Center = player.MountedCenter;
                if (Main.myPlayer == projectile.owner && player.whoAmI == projectile.owner)
                {
					Vector2 rotation = Main.MouseWorld - player.MountedCenter;
                    this.rotation = rotation.ToRotation();
                    player.ChangeDir(Main.MouseWorld.X > player.MountedCenter.X ? 1 : -1);
                    // TODO: Sync
                }
                projectile.position += rotation.ToRotationVector2() * 20;
                projectile.rotation = player.direction == -1 ? rotation + (float)Math.PI : rotation;
                int bodyFrame = Utilities.BodyFrameCalculator.GetBodyFrameFromRotation(rotation);
                player.heldProj = projectile.whoAmI;
                player.bodyFrame.Y = bodyFrame * player.bodyFrame.Height;

                #region Shoot
                ShootTimer++;
                if (ShootTimer >= 20)
                {
                    ShootTimer = 0;
                    if (Main.myPlayer == projectile.owner && player.whoAmI == projectile.owner)
                    {
                        Projectile.NewProjectile(projectile.Center, rotation.ToRotationVector2() * 10, ModContent.ProjectileType<BulletBill>(), player.GetWeaponDamage(player.HeldItem), 1, projectile.owner);
                        Projectile.NewProjectile(projectile.Center + rotation.ToRotationVector2() * 16, Vector2.Zero, ModContent.ProjectileType<BillyGunShootCloud>(), 0, 1, projectile.owner);
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BillyGunShoot"), projectile.Center);
                    }
                }
                #endregion
                #region Animate
                projectile.frameCounter++;
                if (projectile.frameCounter > 5)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 4)
                    {
                        projectile.frame = 0;
                    }
                }
                #endregion
            }
            else
            {
                projectile.Kill();
            }
        }
    }
    public class BillyGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(item.type, new Terraria.DataStructures.DrawAnimationVertical(5, 5));
        }
        public override void SetDefaults()
        {
            item.damage = 40;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<BillyGunHoldProjectile>();
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 10;
            item.useTime = 10;
            item.autoReuse = false;
            item.noUseGraphic = true; // Animated items don't work on the hand
        }
        public override bool UseItemFrame(Player player)     //this defines what frame the player use when this weapon is used
        {
            return true;
        }
    }
}
