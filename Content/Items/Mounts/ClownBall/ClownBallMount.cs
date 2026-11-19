using Epicosity.Buffs;
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

namespace Epicosity.Mounts
{
    class ClownBallMount : ModMountData
    {
		public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			SetDefaults();
			// Use PlayerData to accommodate for each player since ModMountData is a singleton
			ModPlayerClownBallData clownBallData = mountedPlayer.GetModPlayer<ModPlayerClownBallData>();
			
			// Default speeds
			clownBallData.dashSpeed = 8f;
			clownBallData.runSpeed = 5f;
			if (mountedPlayer.armor[10].type == ItemID.ClownHat && mountedPlayer.armor[11].type == ItemID.ClownShirt && mountedPlayer.armor[12].type == ItemID.ClownPants)
			{
				clownBallData.dashSpeed = 14f;
				clownBallData.runSpeed = 11f;
			}
			//mod.Logger.Info(mountedPlayer.armor[0].type);
			if (mountedPlayer.wet)
			{
				mountedPlayer.velocity.Y -= 0.5f;
				mountedPlayer.velocity.X *= 1.1f;
				if (mountedPlayer.velocity.X > clownBallData.runSpeed * 2)
				{
					mountedPlayer.velocity.X = clownBallData.runSpeed * 2;
				}
				if (mountedPlayer.velocity.X < -clownBallData.runSpeed * 2)
				{
					mountedPlayer.velocity.X = -clownBallData.runSpeed * 2;
				}
			}
			if (mountedPlayer.velocity.X != 0f)
			{
				if ((mountedPlayer.slippy || mountedPlayer.slippy2 || mountedPlayer.windPushed) && !mountedPlayer.controlLeft && !mountedPlayer.controlRight)
				{
					clownBallData.legFrameCounter = 0.0;
					clownBallData.legFrame = 0;
					clownBallData.ballFrame = 0;
				}
				else
				{
					clownBallData.legFrameCounter += (double)Math.Abs(mountedPlayer.velocity.X) * 1.3;
					while (clownBallData.legFrameCounter > 8.0)
					{
						clownBallData.legFrameCounter -= 8.0;
						clownBallData.legFrame--;
						clownBallData.ballFrame--;
					}
					if (clownBallData.ballFrame > 11)
					{
						clownBallData.ballFrame = 0;
					}
					if (clownBallData.ballFrame < 0)
					{
						clownBallData.ballFrame = 11;
					}
					if (clownBallData.legFrame < 7)
					{
						clownBallData.legFrame = 19;
					}
					else if (clownBallData.legFrame > 19)
					{
						clownBallData.legFrame = 7;
					}
				}
			}
			// Animation
			mountedPlayer.mount._frame = clownBallData.ballFrame;

            mountData.bodyFrame = clownBallData.legFrame;
			mountedPlayer.legFrame.Y = mountedPlayer.legFrame.Height * clownBallData.legFrame;
			return false;
		}
		public override void SetDefaults()
        {
			//mountData.spawnDust = DustType<>();
			mountData.buff = BuffType<ClownBallMountBuff>();
			mountData.fallDamage = 0.5f;

			// Default speeds that will be overriden by direct player modification
			mountData.runSpeed = 5f;
			mountData.dashSpeed = 8f;

			mountData.flightTimeMax = 0;
			mountData.fatigueMax = 0;
			mountData.jumpHeight = 10;
			mountData.acceleration = 0.39f;
			mountData.jumpSpeed = 12f;
			mountData.blockExtraJumps = false;
			mountData.totalFrames = 12;
			mountData.xOffset = 0;
			//mountData.bodyFrame = 1;
			mountData.yOffset = 28;
			mountData.idleFrameLoop = true;
			mountData.heightBoost = 56;
			int[] array = new int[mountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 44;
			}
			mountData.playerYOffsets = array;
			//mountData.yOffset = 44;
			mountData.playerHeadOffset = 58;
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}
			mountData.textureWidth = mountData.backTexture.Width;
			mountData.textureHeight = mountData.backTexture.Height;
			//mountData.constantJump = true;
		}
	}
}
