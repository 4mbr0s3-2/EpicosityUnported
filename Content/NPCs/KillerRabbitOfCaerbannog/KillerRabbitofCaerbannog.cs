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
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs
{
    public class KillerRabbitofCaerbannog : ModNPC
    {
		public int attackTimer = 60;
		public int attackDurationTimer = 60;
		public bool attacking = false;
		public override void SetStaticDefaults()
        {
			NPCID.Sets.TrailingMode[npc.type] = 1;
			NPCID.Sets.TrailCacheLength[npc.type] = 5;
			Main.npcFrameCount[npc.type] = 7;
            DisplayName.SetDefault("Killer Rabbit of Caerbannog");
        }
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.ZoneOverworldHeight && spawnInfo.spawnTileType == TileID.Stone && Main.hardMode)
			{
				return Main.dayTime ? 0.0001f : 0.0007f;
			}
			return 0;
		}
		public override void SetDefaults()
		{
			npc.width = 18;
			npc.height = 20;
			npc.aiStyle = 3;
			npc.defense = 100;
			npc.lifeMax = 200;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.damage = 30;
		}
        public override void FindFrame(int frameHeight)
        {
			npc.spriteDirection = npc.direction;
			if (npc.velocity.Y == 0f)
			{
				if (npc.velocity.X == 0f)
				{
					npc.frame.Y = 0;
					npc.frameCounter = 0.0;
					return;
				}
				npc.frameCounter += (double)(Math.Abs(npc.velocity.X) * 1f);
				npc.frameCounter += 1.0;
				if (npc.frameCounter > 6.0)
				{
					npc.frame.Y += frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type])
				{
					npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.velocity.Y < 0f)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = frameHeight * 4;
				}
				if (npc.velocity.Y > 0f)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = frameHeight * 6;
				}
			}
			if (attacking)
			{
				npc.frame.Y = frameHeight * 6;
			}
        }
        public override bool PreAI()
        {
			npc.TargetClosest();
			// Has registered a player target
			Player target = Main.player[npc.target];
			float npcRange = 600;
				
			attacking = false;
			if (npc.Distance(target.Center) < npcRange)
			{
				attackTimer--;
				if (attackTimer < 0) // Attack phase
				{
					attackTimer = 70;
					attackDurationTimer = 10;
				}
				if (attackDurationTimer > 0)
				{
					attackDurationTimer--;
					if (Main.netMode != NetmodeID.Server && attackTimer == -1) // First frame of attack, make sound
					{
						Main.PlaySound(Epicosity.inst.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/Enemy/KillerRabbitSqueak"), npc.Center);
					}
					#region Attack behavior
					float attackSpeed = 10;
					attacking = true;
					if (Collision.CheckAABBvAABBCollision(npc.position, npc.Size, target.position, target.Size))
					{
						// Already touched player, cancel timer
						attackDurationTimer = 0;
						return false;
					}
					Vector2 velocity = target.Center - npc.Center;
					velocity = velocity.SafeNormalize(Vector2.Zero);
					npc.velocity = velocity * attackSpeed;
					#endregion
					return false; // Return false to prevent vanilla aiStyle from running
				}
			}
			return true;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (!attacking)
			{
				return true;
			}
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = texture.Frame(1, Main.npcFrameCount[npc.type], 0, 6);
			for (int oldPosIndex = 0; oldPosIndex < NPCID.Sets.TrailCacheLength[npc.type]; oldPosIndex++)
			{
				spriteBatch.Draw
				(
					texture,
					new Rectangle((int)npc.oldPos[oldPosIndex].X - (int)Main.screenPosition.X + npc.width / 2, (int)npc.oldPos[oldPosIndex].Y - (int)Main.screenPosition.Y + 4 + npc.height, frame.Width, frame.Height),
					frame,
					drawColor * ((float)(NPCID.Sets.TrailCacheLength[npc.type] - oldPosIndex) / NPCID.Sets.TrailCacheLength[npc.type]),
					0,
					frame.Size() * new Vector2(0.5f, 1),
					npc.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					0
				);
			}
			return true;
		}
	}
}
