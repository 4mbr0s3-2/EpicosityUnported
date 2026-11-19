using Epicosity.Projectiles.Enemy;
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
using TerrariaOverhaul.Utilities.Extensions;

namespace Epicosity.NPCs.DummyInvasion
{
	public class PilotDummy : ModNPC
    {
        public bool initialized = false;
        public int PropellerFrameCounter = 0;
        public int PropellerFrame = 0;
        public int PlaneFrame = 0;
        public float LeanFrame = 0;
        public float Acceleration = 0;
        public float AccelerationSpeed = 0;
        public int BombDropTimer = 300;
        public float AccelerationCap = 1;
        public float VelocityCap = 10;
        public bool DroppingBomb = false;
        public float VerticalAcceleration = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aviator Dummy");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.lifeMax = 5000;
            npc.defense = 90;
            npc.HitSound = SoundID.NPCHit15;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPCs/DummyPunch");
            npc.knockBackResist = 0f; // No knockback at all
            npc.aiStyle = -1; // We're doing new AI
            npc.width = 168;
            npc.height = 78;
        }
        public override void AI()
        {
            if (!initialized)
			{
                initialized = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    AccelerationSpeed = Main.rand.Next(3, 7) / 300f;
                    npc.netUpdate2 = true; // TODO: Sync with other clients
                }
			}
            #region Target nearest player
            npc.TargetClosest();
            #endregion
            #region If player targeted, change acceleration
            if (npc.HasPlayerTarget)
            {
                NPCAimedTarget target = npc.GetTargetData();
                if (target.Center.X > npc.Center.X)
                {
                    Acceleration += AccelerationSpeed;
                    if (Acceleration > AccelerationCap)
                    {
                        Acceleration = AccelerationCap;
                    }
                }
                else if (target.Center.X < npc.Center.X)
                {
                    Acceleration -= AccelerationSpeed;
                    if (Acceleration < -AccelerationCap)
                    {
                        Acceleration = -AccelerationCap;
                    }
                }
            }
            #endregion
            #region Apply acceleration
            npc.velocity.X += Acceleration;
            if (npc.velocity.X > VelocityCap)
            {
                npc.velocity.X = VelocityCap;
            }
            else if (npc.velocity.X < -VelocityCap)
            {
                npc.velocity.X = -VelocityCap;
            }
            #endregion
            #region Bomb dropping
            BombDropTimer--;
            if (BombDropTimer <= 0 && !DroppingBomb)
            {
                BombDropTimer = 200;
                DroppingBomb = true;
            }
            if (DroppingBomb && BombDropTimer > 0)
            {
                if (BombDropTimer % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<WoodenBomb>(), 80, 0);
                }
            }
            else if (BombDropTimer <= 0)
            {
                BombDropTimer = 500;
                DroppingBomb = false;
            }
            #endregion
            #region Move if too close to ground
            bool moveUp = false;
            for (int tileYCheck = npc.position.ToTileCoordinates16().Y; tileYCheck < npc.position.ToTileCoordinates16().Y + 32; tileYCheck++)
            {
                if (tileYCheck >= 0 && tileYCheck < Main.maxTilesY && npc.position.ToTileCoordinates16().X >= 0 && npc.position.ToTileCoordinates16().X < Main.maxTilesX) 
                {
                    Tile checkTile = Main.tile[npc.position.ToTileCoordinates16().X, tileYCheck];
                    if (checkTile.active() || checkTile.liquid != 0)
                    {
                        moveUp = true;
                        break;
                    }
                }
            }
            if (moveUp)
            {
                VerticalAcceleration = -0.2f;
            }
            else
            {
                VerticalAcceleration = 0.1f;
            }
            npc.velocity.Y += VerticalAcceleration;
            if (npc.velocity.Y > VelocityCap)
            {
                npc.velocity.Y = VelocityCap;
            }
            else if (npc.velocity.Y < -VelocityCap)
            {
                npc.velocity.Y = -VelocityCap;
            }
            //Main.NewText(npc.velocity.Y);
            #endregion
        }
        public override void FindFrame(int frameHeight)
        {
            PlaneFrame = 1;
            if (Math.Abs(npc.velocity.X) < 7)
            {
                PlaneFrame = 0;
            }
            if (DroppingBomb)
            {
                PlaneFrame = 2;
            }
            npc.frame.Y = PlaneFrame * frameHeight;

            PropellerFrameCounter++;
            if (PropellerFrameCounter > 5)
            {
                PropellerFrameCounter = 0;
                PropellerFrame++;
                if (PropellerFrame > 2)
                {
                    PropellerFrame = 0;
                }
            }
            npc.spriteDirection = npc.velocity.X < 0 ? -1 : 1;
        }
        // The propeller is independent of the base frame, so we need to draw it separately
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D propellerTexture = ModContent.GetTexture("Epicosity/NPCs/DummyInvasion/PilotDummy_Propeller");
            spriteBatch.Draw
            (
                propellerTexture,
                new Rectangle((int)(npc.position.X - Main.screenPosition.X), (int)(npc.position.Y - Main.screenPosition.Y) + 5, npc.width, npc.height),
                new Rectangle(0, PropellerFrame * 78, 168, 78),
                drawColor,
                0,
                Vector2.Zero,
                npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );
        }
    }
}
