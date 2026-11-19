using Epicosity.Buffs;
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
    public class NyanCatMount : ModMountData
    {
		public override void SetMount(Player player, ref bool skipDust)
		{
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Mount/MagicPoptartUse"));
			ModPlayerNyanCatMountData nyanCatMountData = player.GetModPlayer<ModPlayerNyanCatMountData>();
			nyanCatMountData.oldRainbowPosition = new List<Vector2>();
		}
        public override void Dismount(Player player, ref bool skipDust)
        {
		}
        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			ModPlayerNyanCatMountData nyanCatMountData = mountedPlayer.GetModPlayer<ModPlayerNyanCatMountData>();

			if (mountedPlayer.controlJump)
            {
				mountedPlayer.velocity.Y -= 0.5f;
				if (mountedPlayer.velocity.Y < -8)
                {
					mountedPlayer.velocity.Y = -8;

				}
            }
			
			// Animation
			if (velocity.Y == 0)
			{
				if (Math.Abs(velocity.X) > 0.1f)
				{
					nyanCatMountData.catFrame += (int)Math.Abs(mountedPlayer.velocity.X) / 30.0f;

					if (nyanCatMountData.catFrame < 8 || nyanCatMountData.catFrame >= 14)
					{
						nyanCatMountData.catFrame = 8;
					}
				}
				else
				{
					nyanCatMountData.idleCatFrameCounter++;
					if (nyanCatMountData.idleCatFrameCounter > 5)
					{
						nyanCatMountData.idleCatFrameCounter = 0;
						nyanCatMountData.catFrame++;
						if (nyanCatMountData.catFrame < 0 || nyanCatMountData.catFrame >= 8)
						{
							nyanCatMountData.catFrame = 0;
						}
					}
				}
			}
			if (velocity.Y != 0)
			{
				if (velocity.Y < -2)
				{
					nyanCatMountData.catFrame = 13;
				}
				else if (velocity.Y > 2)
				{
					nyanCatMountData.catFrame = 10;
				}
				else
                {
					nyanCatMountData.catFrame = 9;
				}
			}
            SetDefaults();
            mountedPlayer.mount._frame = (int)nyanCatMountData.catFrame;
			mountedPlayer.legFrame.Y = mountedPlayer.legFrame.Height * 6;
			return false;
		}
		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			ModPlayerNyanCatMountData nyanCatMountData = drawPlayer.GetModPlayer<ModPlayerNyanCatMountData>();

			// Draw trail
			
			Main.instance.LoadProjectile(250);
			Texture2D rainbowProjectileTexture = Main.projectileTexture[250];
			Vector2 origin = new Vector2((float)(rainbowProjectileTexture.Width / 2), 0f);
			Vector2 halfSize = new Vector2((float)mountData.textureWidth, (float)mountData.textureHeight) / 2f;
			Color transparentWhite = Color.White;
			transparentWhite.A = 127;
			for (int oldPosIndex = nyanCatMountData.oldRainbowPosition.Count - 1; oldPosIndex > 0; oldPosIndex--)
			{
				Vector2 oldPos = nyanCatMountData.oldRainbowPosition[oldPosIndex] + halfSize;
				if (!(oldPos == halfSize))
				{
					Vector2 vector = nyanCatMountData.oldRainbowPosition[oldPosIndex - 1] + halfSize;
					float rainbowRotation = (vector - oldPos).ToRotation() - 1.57079637f;
					Vector2 scale = new Vector2(1f, Vector2.Distance(oldPos, vector) / (float)rainbowProjectileTexture.Height);
					Color colorAlpha = transparentWhite * ((float)oldPosIndex / (float)nyanCatMountData.oldRainbowPosition.Count);
					Texture2D rainbowTexture = rainbowProjectileTexture;
					Vector2 rainbowDrawPosition = oldPos - Main.screenPosition;
					DrawData rainbowSegment = new DrawData(rainbowTexture, rainbowDrawPosition, new Rectangle(0, 0, rainbowTexture.Width, rainbowTexture.Height), colorAlpha, rainbowRotation, origin, scale, SpriteEffects.None, 0);
					rainbowSegment.shader = Mount.currentShader;
					playerDrawData.Add(rainbowSegment);
					//Main.spriteBatch.End();
					//Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
					//Main.spriteBatch.Draw(rainbowTexture, rainbowDrawPosition, new Rectangle(0, 0, rainbowTexture.Width, rainbowTexture.Height), colorAlpha, rainbowRotation, origin, scale, SpriteEffects.None, 0);
				}
			}

			// Rainbow trail

			if (shadow == 0)
			{
				nyanCatMountData.oldRainbowPosition.Add(drawPlayer.MountedCenter + new Vector2(-35, -264));
				if (nyanCatMountData.oldRainbowPosition.Count > 20)
				{
					nyanCatMountData.oldRainbowPosition.RemoveAt(0);
				}
			}

			return true;
		}
		public override void SetDefaults()
        {
			mountData.buff = BuffType<NyanCatMountBuff>();
			mountData.fallDamage = 0.5f;

			// Default speeds that will be overriden by direct player modification
			mountData.runSpeed = 10f;
			mountData.dashSpeed = 16f;
			mountData.bodyFrame = 4;

			mountData.idleFrameLoop = true;

			mountData.flightTimeMax = 10;
			mountData.fatigueMax = 0;
			
			mountData.jumpHeight = 1;
			mountData.jumpSpeed = 1f;
			
			mountData.acceleration = 0.4f;
			mountData.blockExtraJumps = false;
			mountData.totalFrames = 14;
			mountData.heightBoost = 32;
			
			mountData.xOffset = 0;
			mountData.yOffset = 18;
			
			int[] array = new int[mountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				if (l <= 10)
				{
					array[l] = 26;
				}
				else
				{
					array[l] = 28;
				}
			}

			mountData.playerYOffsets = array;
			mountData.playerHeadOffset = 34;
			
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}
			
			mountData.textureWidth = mountData.backTexture.Width;
			mountData.textureHeight = mountData.backTexture.Height;
		}
	}
}
