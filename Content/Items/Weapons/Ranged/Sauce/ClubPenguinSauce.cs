using Epicosity.Items.Weapons.Melee;
using Epicosity.Network;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Weapons.Ranged
{
	public class ClubPenguinSauce : ModItem
    {
        public float itemRotation = 0;
        public override bool Autoload(ref string name)
        {
            return false;
        }
        public override void SetDefaults()
        {
            item.channel = true;
            item.width = 14;
            item.height = 34;
            item.ranged = true;
            item.useStyle = CustomItemUseStyleID.Custom;
        }
        public override void UseStyle(Player player)
        {
            if (item.owner == Main.myPlayer)
            {
                Microsoft.Xna.Framework.Vector2 direction = Main.MouseWorld - player.Center;
                direction.Normalize();

                itemRotation = direction.ToRotation();
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    Network.ModNetHandler.GetPacketHandler<FlashlightItemRotationPacketHandler>().SendTarget(-1, player.whoAmI, itemRotation, player.direction);
                }
            }
            player.itemRotation = itemRotation + (float)Math.PI / 2f;
            player.bodyFrame.Y = player.bodyFrame.Height * BodyFrameCalculator.GetBodyFrameFromRotation(itemRotation);
            player.itemLocation = player.MountedCenter + new Microsoft.Xna.Framework.Vector2(-8 * player.direction, 8).RotatedBy(player.itemRotation) + new Microsoft.Xna.Framework.Vector2(8 * player.direction, 2);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Microsoft.Xna.Framework.Vector2 direction = new Vector2(speedX, speedY);
            direction.Normalize();
            player.itemRotation = direction.ToRotation() + (float)Math.PI / 2f;
            return true;
        }
        public override void HoldStyle(Player player)
        {
            UseStyle(player);
        }
    }
    public class PizzaSauce : ClubPenguinSauce
    {
        public class PizzaSauceProjectile : ModProjectile
        {
            public override void AutoStaticDefaults()
            {
                Main.projectileTexture[projectile.type] = ModContent.GetTexture("Terraria/Projectile_0");
                Main.projFrames[projectile.type] = 1;
            }
            public override void SetDefaults()
            {
                projectile.aiStyle = 1;
                projectile.width = 15;
                projectile.height = 15;
                projectile.extraUpdates = 3;
                projectile.damage = 1;
                projectile.penetrate = 1;
                projectile.friendly = true;
                projectile.ranged = true;
            }
            public override void AI()
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, 5, 5, 64, 0, 0, 0, new Microsoft.Xna.Framework.Color(255, 128, 0))];
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
        public override bool Autoload(ref string name)
        {
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.shoot = ModContent.ProjectileType<PizzaSauceProjectile>();
            item.shootSpeed = 5;
            item.useTime = 20;
            item.useAnimation = 20;
            item.damage = 10;
            item.autoReuse = true;
        }
    }
    class ChocolateSauce : ClubPenguinSauce
    {
        class ChocolateSauceProjectile : ModProjectile
        {
            public override void AutoStaticDefaults()
            {
                Main.projectileTexture[projectile.type] = ModContent.GetTexture("Terraria/Projectile_0");
                Main.projFrames[projectile.type] = 1;
            }
            public override void SetDefaults()
            {
                projectile.aiStyle = 1;
                projectile.width = 15;
                projectile.height = 15;
                projectile.extraUpdates = 3;
                projectile.damage = 1;
                projectile.penetrate = 1;
                projectile.friendly = true;
                projectile.ranged = true;
            }
            public override void AI()
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, 5, 5, 64, 0, 0, 0, new Microsoft.Xna.Framework.Color(255/5, 128/5, 0))];
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
        public override bool Autoload(ref string name)
        {
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.shoot = ModContent.ProjectileType<ChocolateSauceProjectile>();
            item.shootSpeed = 6;
            item.useTime = 10;
            item.useAnimation = 10;
            item.damage = 20;
            item.autoReuse = true;
        }
    }
    class HotSauce : ClubPenguinSauce
    {
        class HotSauceProjectile : ModProjectile
        {
            public override void AutoStaticDefaults()
            {
                Main.projectileTexture[projectile.type] = ModContent.GetTexture("Terraria/Projectile_0");
                Main.projFrames[projectile.type] = 1;
            }
            public override void SetDefaults()
            {
                projectile.aiStyle = 1;
                projectile.width = 15;
                projectile.height = 15;
                projectile.extraUpdates = 3;
                projectile.damage = 1;
                projectile.penetrate = 1;
                projectile.friendly = true;
                projectile.ranged = true;
            }
            public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
            public override void AI()
            {
                int dustIndex = Dust.NewDust(projectile.position, 5, 5, 64, 0, 0, 0, new Microsoft.Xna.Framework.Color(255, 0, 0));
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
        public override bool Autoload(ref string name)
        {
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.shoot = ModContent.ProjectileType<HotSauceProjectile>();
            item.shootSpeed = 7;
            item.useTime = 10;
            item.useAnimation = 10;
            item.damage = 60;
            item.autoReuse = true;
        }
    }
    internal class ClubPenguinSauceItemRotationPacketHandler : PacketHandler
    {
        public const byte SyncTarget = 1;

        public ClubPenguinSauceItemRotationPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case (SyncTarget):
                    ReceiveTarget(reader, fromWho);
                    break;
            }
        }

        public void SendTarget(int toWho, int fromWho, float itemRotation)
        {
            ModPacket packet = GetPacket(SyncTarget, fromWho);
            packet.Write(itemRotation);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveTarget(BinaryReader reader, int fromWho)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                fromWho = reader.ReadByte(); // fromWho
            }
            float itemRotation = reader.ReadSingle();
            int playerDirection = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server)
            {
                SendTarget(-1, fromWho, itemRotation);
            }
            else
            {
                if (Main.player[fromWho].HeldItem.modItem is ClubPenguinSauce)
                {
                    ((ClubPenguinSauce)Main.player[fromWho].HeldItem.modItem).itemRotation = itemRotation;
                }
            }
        }
    }
}
