using Epicosity.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs.Bosses
{
	public class DancingTurnip : ModNPC
    {
        // TODO: Check for multiplayer compatibility
        public int frame = 0;
        public bool teleporting = false;
        public float smallTurnipsRotation = 0;
        public class DancingTurnipTrail : ModProjectile
        {
            public Rectangle textureRectangle = new Rectangle();
            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Dancing Turnip");
            }
            public override void SetDefaults()
            {
                projectile.tileCollide = false;
                projectile.width = 64;
                projectile.height = 148;
                projectile.scale = 1;
            }
            public override void SendExtraAI(BinaryWriter writer)
            {
                writer.Write(textureRectangle.X);
                writer.Write(textureRectangle.Y);
                writer.Write(textureRectangle.Width);
                writer.Write(textureRectangle.Height);
            }
            public override void ReceiveExtraAI(BinaryReader reader)
            {
                textureRectangle = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            }
            public override void AutoStaticDefaults()
            {
            }
            public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
            {
                Texture2D texture = mod.GetTexture("NPCs/Bosses/DancingTurnip");
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw
                (
                    texture,
                    new Vector2
                    (
                        projectile.position.X - Main.screenPosition.X + 4,
                        projectile.position.Y - Main.screenPosition.Y + 50
                    ),
                    textureRectangle.Width < 0 ? new Rectangle(0, textureRectangle.Y, -textureRectangle.Width, textureRectangle.Height) : textureRectangle,
                    new Color(1, 1, 1, 0.5f - projectile.alpha * 0.5f / 255f),
                    projectile.rotation,
                    projectile.getRect().Size() * 0.5f,
                    projectile.scale,
                    textureRectangle.Width < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0f
                );
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                return false;
            }
            public override void AI()
            {
                projectile.velocity /= 1.05f;
                if (projectile.velocity.Length() <= 1f)
                {
                    projectile.alpha += 20;
                }
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                }
            }
        }
        public enum BossStage
        {
            Dancing,
            KRound,
            Dashing,
            Jumping
        }
        public BossStage bossStage = BossStage.Dancing;
        public BossStage syncBossStage = BossStage.Dancing;
        public int jumpDirection = 0;
        public float speed = 14;
        public int bossStageTimer = 0;
        public float teleportDistance = 3000;
        public Vector2 posBeforeTeleport = new Vector2(0, 0);
        public bool foundTeleportPosition = false;
        public int teleportTimer = 0;
        public Vector2 teleportPosition = new Vector2(0, 0);
        public Vector2 jumpTargetPosition = new Vector2(0, 0);
        public Vector2 posBeforeJump = new Vector2(0, 0);
        public List<int> smallTurnips = new List<int>(); 
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dancing Turnip");
            Main.npcFrameCount[npc.type] = 18;
            NPCID.Sets.TrailingMode[npc.type] = 0;
            NPCID.Sets.TrailCacheLength[npc.type] = 10;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)bossStage);
            writer.Write((int)syncBossStage); // Sent before bossStage is set to this
            writer.Write(bossStageTimer); // Just to be safe
            writer.WriteVector2(teleportPosition);
            writer.WriteVector2(posBeforeTeleport);
            writer.Write(foundTeleportPosition);
            writer.Write(teleporting);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bossStage = (BossStage)reader.ReadInt32();
            syncBossStage = (BossStage)reader.ReadInt32();
            bossStageTimer = reader.ReadInt32();
            teleportPosition = reader.ReadVector2();
            posBeforeTeleport = reader.ReadVector2();
            foundTeleportPosition = reader.ReadBoolean();
            teleporting = reader.ReadBoolean();
        }
        public override void SetDefaults()
        {
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.lifeMax = 40000;
            npc.width = 64;
            npc.height = 148;
            npc.aiStyle = 0;
            npc.boss = true;
            npc.damage = 80;
            npc.knockBackResist = 0f;
            music = SoundLoader.GetSoundSlot(SoundType.Music, "Epicosity/Sounds/Music/5050");
        }
        public override void AI()
        {
            speed = 14;
            npc.TargetClosest();
            //#region Horizontal movement
            if (npc.target < 0)
            {
                return;
            }
            Player targetPlayer = Main.player[npc.target];
            if (!teleporting)
            {
                if (bossStage == BossStage.Dancing)
                {
                    if (bossStageTimer == 2) // Prepare well before the switch
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int rand = Main.rand.Next(0, 2);
                            switch (rand)
                            {
                                case 0:
                                    syncBossStage = BossStage.Dashing;
                                    break;
                                default:
                                    syncBossStage = BossStage.Jumping;
                                    break;
                            }
                            npc.netUpdate = true;
                        }
                    }
                    #region Jump if on ground
                    if (Math.Abs(npc.velocity.Y) <= 0.001f)
                    {
                        npc.velocity.Y = -7;
                        bossStageTimer++;
                        #region Change jump direction on landing
                        if (targetPlayer.position.X < npc.position.X)
                        {
                            jumpDirection = -1;
                        }
                        else
                        {
                            jumpDirection = 1;
                        }
                        #endregion
                    }
                    #endregion
                    #region Switch if has made 8 jumps
                    if (bossStageTimer >= 8)
                    {
                        bossStageTimer = 0;
                        bossStage = syncBossStage; // At this point, syncBossStage should be synced
                    }
                    #endregion
                    npc.velocity.X = jumpDirection * speed * 0.5f;
                }
                if (bossStage == BossStage.Dashing)
                {
                    bossStageTimer++;
                    if (bossStageTimer % 60 == 0)
                    {
                        Vector2 dashVel = -(npc.Center - targetPlayer.Center); // Deterministic, execute on both server and client
                        dashVel.Normalize();
                        npc.velocity = dashVel * speed;
                        if (Main.netMode != NetmodeID.MultiplayerClient) // Projectiles, so execute on server
                        {
                            int projIndex = Projectile.NewProjectile(npc.position, dashVel * speed, ModContent.ProjectileType<DancingTurnipTrail>(), 20, 0);
                            Main.projectile[projIndex].Center = npc.Center;
                            Rectangle currentFrame = npc.frame;
                            currentFrame.Width *= npc.direction;
                            ((DancingTurnipTrail)Main.projectile[projIndex].modProjectile).textureRectangle = currentFrame;
                            Main.projectile[projIndex].netUpdate2 = true;
                            for (int x = 0; x < 5; x++)
                            {
                                Projectile.NewProjectile(npc.Bottom, dashVel.RotatedBy(x * 0.12f - 0.12f * 2.5f) * 15, ModContent.ProjectileType<Turnip>(), 30, 1);
                            }
                        }
                    }
                    if (bossStageTimer > 200)
                    {
                        bossStageTimer = 0;
                        bossStage = BossStage.Dancing;
                    }
                }
                if (bossStage == BossStage.Jumping)
                {
                    if (bossStageTimer == 0)
                    {
                        jumpTargetPosition = new Vector2(targetPlayer.position.X, targetPlayer.position.Y - 300);
                        npc.noTileCollide = true;
                        posBeforeJump = npc.position;
                    }

                    bossStageTimer++;

                    // Go to target position
                    if (bossStageTimer <= 30)
                    {
                        npc.velocity = Vector2.Zero;
                        if (bossStageTimer % 2 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnTrailProjectile();
                            }
                        }
                        npc.position += (jumpTargetPosition - posBeforeJump) / 30;
                    }
                    else if (bossStageTimer <= 45)
                    {
                        npc.position = jumpTargetPosition;
                    }
                    // Go down
                    if (bossStageTimer > 45)
                    {
                        if (bossStageTimer % 2 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnTrailProjectile();
                            }
                        }
                        npc.position.Y += 10;
                        // Check for collision with tile
                        if (Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height) != npc.velocity)
                        {
                            // Take place both on server and on clients so smallTurnips is synced
                            smallTurnips.Add(NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<FluteTurnip>()));
                            smallTurnips.Add(NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<PianoTurnip>()));
                            smallTurnips.Add(NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SaxTurnip>()));

                            npc.position.Y -= 10;
                            // Perform one-shot projectile blast
                            if (Epicosity.loadedCompatibilityMods.HasFlag(Epicosity.CompatibilityMods.TerrariaOverhaul))
                            {
                                OverhaulScreenshake();
                            }
                            npc.noTileCollide = false;
                            bossStage = BossStage.Dancing;
                            bossStageTimer = 0;

                            Vector2 bottom = npc.Bottom;
                            Point16 bottomTile = bottom.ToTileCoordinates16();
                            Tile tile = Main.tile[bottomTile.X, bottomTile.Y];
                            if (tile.active())
                            {
                                for (int x = 0; x < 20; x++) 
                                {
                                    int dustIndex = WorldGen.KillTile_MakeTileDust(bottomTile.X, bottomTile.Y, tile);
                                    Main.dust[dustIndex].velocity *= 3;
                                }
                            }


                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int x = 0; x <= 10; x++)
                                {
                                    Projectile.NewProjectile(npc.Bottom, new Vector2(15, 0).RotatedBy(-Math.PI / 10f * x), ModContent.ProjectileType<Turnip>(), 55, 0.2f);
                                }
                            }
                        }
                    }
                    if (bossStageTimer > 70)
                    {
                        npc.noTileCollide = false;
                        bossStage = BossStage.Dancing;  
                        bossStageTimer = 0;
                    }
                }
            }
            #region Check if player is not in sight, then teleport
            if (!teleporting && bossStage == BossStage.Dancing)
            {
                // Teleporting is calculated on the server, then synced to client
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if ((!Collision.CanHitLine(npc.position, npc.width, npc.height, targetPlayer.position, targetPlayer.width, targetPlayer.height) && (npc.Center - targetPlayer.Center).Length() > 400) || (npc.Center - targetPlayer.Center).Length() > 800)
                    {
                        teleporting = true;
                        npc.netUpdate = true;
                    }
                }
            }
            #endregion
            if (teleporting)
            {
                if (!foundTeleportPosition)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) // Find a place on behalf of the server, client can't really do anything about it until foundTeleportPosition is true
                    {
                        teleportPosition = targetPlayer.position;
                        bool obstructed = false;
                        #region Attempt to find a good position
                        for (int attemptToFindPosition = 0; attemptToFindPosition < 150; attemptToFindPosition++)
                        {
                            //Main.NewText("Attempt " + attemptToFindPosition);
                            teleportPosition = targetPlayer.position;
                            teleportPosition += new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                            for (int x = teleportPosition.ToTileCoordinates().X; x < (teleportPosition + new Vector2(npc.width, 0)).ToTileCoordinates().X; x++)
                            {
                                for (int y = teleportPosition.ToTileCoordinates().Y; y < (teleportPosition + new Vector2(0, -npc.height)).ToTileCoordinates().Y; y++)
                                {
                                    if (Main.tile[x, y].active())
                                    {
                                        obstructed = true;
                                        break;
                                    }
                                }
                                if (obstructed)
                                {
                                    break;
                                }
                            }
                            if (!Collision.CanHitLine(teleportPosition, npc.width, npc.height, targetPlayer.position, targetPlayer.width, targetPlayer.height))
                            {
                                obstructed = true;
                            }
                            float bottomPosY = teleportPosition.Y + npc.height;
                            Point bottomTile = Utils.ToTileCoordinates(new Vector2(teleportPosition.X, bottomPosY));
                            if (!(Main.tile[bottomTile.X, bottomTile.Y + 1].active() && !Main.tile[bottomTile.X, bottomTile.Y].active()))
                            {
                                obstructed = true; // Only use places that have a block at the bottom and air at the top
                            }
                            if (!obstructed) // Not obstructed, use this position and break
                            {
                                Main.NewText("Found good position, teleporting there");
                                foundTeleportPosition = true;
                                posBeforeTeleport = npc.position; // Store current position
                                npc.netUpdate = true; // Sync the position
                                break;
                            }
                        }
                        if (!foundTeleportPosition) // If still hasn't found teleport position, just teleport to player
                        {
                            Main.NewText("Did not find good position, teleporting to player");
                            teleportPosition = targetPlayer.Center + new Vector2(0, -npc.height);
                            foundTeleportPosition = true;
                            posBeforeTeleport = npc.position; // Store current position
                            npc.netUpdate = true; // Sync the position
                        }
                    }
                    #endregion
                } // Found teleport position, now go there on server and client
                else
                {
                    //bossStageTimer++;
                    if (teleportTimer < 30)
                    {
                        npc.noTileCollide = true;
                        teleportTimer++;
                        npc.position += (teleportPosition - posBeforeTeleport) / 30;
                        npc.velocity = Vector2.Zero;

                        if (teleportTimer % 2 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnTrailProjectile();   
                            }
                        }
                    }
                    else
                    {
                        npc.position = teleportPosition;
                        npc.noTileCollide = false;
                        teleportTimer = 0;
                        //bossStageTimer = 0;
                        teleporting = false;
                        foundTeleportPosition = false;
                        teleportPosition = Vector2.Zero;
                        for (int d = 0; d <= 50; d++) Dust.NewDust(npc.position, npc.width, npc.height, 6, 0, 0);
                    }
                    
                }
            }
            #region Small turnips orbiting
            for (int x = 0; x < smallTurnips.Count(); x++)
            {
                int turnipWhoAmI = smallTurnips[x];
                if (!Main.npc[turnipWhoAmI].active || !(Main.npc[turnipWhoAmI].modNPC is SmallTurnip))
                {
                    smallTurnips.Remove(turnipWhoAmI);
                    continue;
                }
                SmallTurnip turnip = (Main.npc[turnipWhoAmI].modNPC as SmallTurnip);
                turnip.relativeRotation = MathHelper.TwoPi / smallTurnips.Count() * x * 0.2f + turnip.relativeRotation * 0.8f;
                Main.npc[turnipWhoAmI].Center = new Vector2(100, 0).RotatedBy(smallTurnipsRotation + turnip.relativeRotation) + npc.Center;
            }
            smallTurnipsRotation += 0.01f;
            #endregion
        }
        public int SpawnTrailProjectile()
        {
            int projIndex = Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<DancingTurnipTrail>(), 20, 0);
            Main.projectile[projIndex].Center = npc.Center;
            Rectangle currentFrame = npc.frame;
            currentFrame.Width *= npc.direction;
            ((DancingTurnipTrail)Main.projectile[projIndex].modProjectile).textureRectangle = currentFrame;
            Main.projectile[projIndex].netUpdate2 = true;
            return projIndex;
        }
        public void OverhaulScreenshake()
        {
            TerrariaOverhaul.Common.Systems.Visuals.ScreenShakes.ScreenShake screenShake = new TerrariaOverhaul.Common.Systems.Visuals.ScreenShakes.ScreenShake(10, 1, npc.Bottom, 1024);
            TerrariaOverhaul.Common.Systems.Visuals.ScreenShakes.ScreenShakeSystem.New(screenShake);
        }
        public override void FindFrame(int frameHeight)
        {
            if (bossStage == BossStage.Dancing)
            {
                frame = bossStageTimer % 6;
            }
            if (bossStage == BossStage.Dashing)
            {
                frame = 6;
                if (bossStageTimer % 60 < 30)
                {
                    frame = 9;
                }
            }
            if (bossStage == BossStage.Jumping)
            {
                if (bossStageTimer >= 30 && bossStageTimer <= 45)
                {
                    frame = 12;
                }
                if (bossStageTimer > 45)
                {
                    frame = 13 + bossStageTimer % 5;
                }
            }
            npc.frame.Y = frameHeight * frame;
        }
    }
}
