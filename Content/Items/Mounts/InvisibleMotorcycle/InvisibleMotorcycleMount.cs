using Epicosity.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Mounts
{
	public class InvisibleMotorcycleMount : ModMountData
    {
		public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			SetDefaults();
			// Animation
			ModPlayerInvisibleMotorcycleData modPlayerInvisibleMotorcycleData = mountedPlayer.GetModPlayer<ModPlayerInvisibleMotorcycleData>();
			modPlayerInvisibleMotorcycleData.rattlingAnimation += MathHelper.TwoPi / 60f; // One cycle every second
			modPlayerInvisibleMotorcycleData.rattlingAnimation %= MathHelper.TwoPi;
			mountedPlayer.gfxOffY = (float)(Math.Sin(modPlayerInvisibleMotorcycleData.rattlingAnimation) * 5);
			mountedPlayer.fullRotationOrigin = new Vector2(mountedPlayer.width / 2f, mountedPlayer.height * 0.75f);
			mountedPlayer.fullRotation = velocity.X * 0.02f + velocity.Y * mountedPlayer.direction * 0.03f;

			if (Math.Abs(velocity.X) > 8)
			{
				for (int x = 0; x < 2; x++)
				{
					Point16 tilePoint = mountedPlayer.Bottom.ToTileCoordinates16() + new Point16((1 - x * 2) * mountedPlayer.direction, 0);
					Tile tile = Utilities.TileHelper.ActuallyGetTileSafely(tilePoint);
					if (Main.tileSolid[tile.type] && tile.active())
					{
						WorldGen.KillTile_MakeTileDust(tilePoint.X, tilePoint.Y, tile);
					}
				}
			}

			return false;
		}
		public override void Dismount(Player player, ref bool skipDust)
		{
			player.gfxOffY = 0;
		}
		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			return false;
		}
		public override void SetDefaults()
        {
			//mountData.spawnDust = DustType<>();
			mountData.buff = BuffType<InvisibleMotorcycleMountBuff>();
			mountData.fallDamage = 0.5f;

			// Default speeds that will be overriden by direct player modification
			mountData.runSpeed = 5f;
			mountData.dashSpeed = 13f;

			mountData.flightTimeMax = 0;
			mountData.fatigueMax = 0;
			mountData.jumpHeight = 2;
			mountData.acceleration = 0.39f;
			mountData.jumpSpeed = 12f;
			mountData.blockExtraJumps = false;
			mountData.totalFrames = 12;
			mountData.xOffset = 0;
			mountData.yOffset = 28;
			mountData.idleFrameLoop = true;
			mountData.heightBoost = 12;
			mountData.bodyFrame = 3;
			int[] array = new int[mountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 11;
			}
			mountData.playerYOffsets = array;
			//mountData.yOffset = 44;
			mountData.playerHeadOffset = 58;
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}
			//mountData.textureWidth = mountData.backTexture.Width;
			//mountData.textureHeight = mountData.backTexture.Height;
			//mountData.constantJump = true;
		}
	}
}
