using Epicosity.Buffs;
using Epicosity.Dusts;
using IL.Terraria.GameContent.Achievements;
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

namespace Epicosity.Mounts
{
    class GDShipMount : ModMountData
    {
        public override void SetMount(Player player, ref bool skipDust)
        {
            player.GetModPlayer<ModPlayerGDShipData>().verticalAcceleration = 0;
        }
        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            ModPlayerGDShipData gdShipData = mountedPlayer.GetModPlayer<ModPlayerGDShipData>();
            SetDefaults();
            mountedPlayer.fullRotationOrigin = new Microsoft.Xna.Framework.Vector2(mountedPlayer.width / 2, mountedPlayer.height);
            mountedPlayer.mount._frame = 0;
            if (mountedPlayer.controlJump)
            {
                gdShipData.verticalAcceleration -= 1f;
                if (gdShipData.verticalAcceleration < -0.7f)
                {
                    gdShipData.verticalAcceleration = -0.7f;
                }
            }
            else
            {
                gdShipData.verticalAcceleration += 0.7f;
                if (gdShipData.verticalAcceleration > 0.7f)
                {
                    gdShipData.verticalAcceleration = 0.7f;
                }
            }
            mountedPlayer.velocity.Y += gdShipData.verticalAcceleration;
            // Cap the velocities
            if (mountedPlayer.velocity.Y > 15)
            {
                mountedPlayer.velocity.Y = 15;
            }
            else if (mountedPlayer.velocity.Y < -15)
            {
                mountedPlayer.velocity.Y = -15;
            }
            #region Spawn dust
            mountedPlayer.fullRotation = (mountedPlayer.velocity.Y * 0.04f * mountedPlayer.direction) * 0.1f + mountedPlayer.fullRotation * 0.9f;
            int numOfDust = Main.rand.Next(1, 3) * (int)(mountedPlayer.velocity.Length() * 0.3f);
            for (int x = 0; x < numOfDust; x++)
            {
                Dust.NewDust(mountedPlayer.MountedCenter + new Vector2(-4, 12) + new Vector2(-28 * mountedPlayer.direction, 0).RotatedBy(mountedPlayer.fullRotation), 1, 1, ModContent.DustType<Square>(), 0f, 0f);
            }
            if (mountedPlayer.controlJump)
            {
                Dust.NewDust(mountedPlayer.MountedCenter + new Vector2(-4, 12) + new Vector2(-28 * mountedPlayer.direction, 0).RotatedBy(mountedPlayer.fullRotation), 1, 1, ModContent.DustType<Square>(), 0f, 0f, 0, default(Color), Main.rand.NextFloat(1.3f, 2f));
            }
            #endregion
            //Main.NewText(mountedPlayer.velocity.Length() * 0.3f);
            return false;
        }
        public override void SetDefaults()
        {
            mountData.buff = BuffType<GDShipMountBuff>();
            mountData.fallDamage = 0.5f;

            // Default speeds that will be overriden by direct player modification
            mountData.runSpeed = 10f;
            mountData.dashSpeed = 16f;
            mountData.bodyFrame = 4;
            mountData.flightTimeMax = 0;

            //mountData.idleFrameLoop = true;
            //mountData.idleFrameStart = 0;
            //mountData.idleFrameCount = 8;
            //mountData.idleFrameDelay = 3;

            //mountData.flightTimeMax = 10;
            mountData.fatigueMax = 0;

            mountData.jumpHeight = 0;
            mountData.jumpSpeed = 0f;

            mountData.acceleration = 0.4f;
            mountData.blockExtraJumps = true;
            mountData.totalFrames = 1;
            mountData.heightBoost = 0;

            mountData.xOffset = 0;
            mountData.yOffset = 12;

            int[] array = new int[1] { 4 };

            mountData.playerYOffsets = array;
            mountData.playerHeadOffset = 34;

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            mountData.textureWidth = mountData.frontTexture.Width;
            mountData.textureHeight = mountData.frontTexture.Height;
        }
    }
}
