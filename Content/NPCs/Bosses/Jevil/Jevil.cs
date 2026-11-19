using Epicosity.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs.Bosses
{
	public class Jevil : ModNPC
    {
        Vector2 targetPosition = new Vector2(0, 0);
        Vector2 syncedTargetPosition = new Vector2(0, 0);
        Vector2 idleRelativePosition = new Vector2(300, -300);
        float hoverAnimationFrame;
        int frame = 0;
        int frameTimer = 0;
        int stateTimer = 0;
        float waveTimer = 0;
        private int teleportTimer;
        int currentPhase = 1;
        private BossState AIBossState
        {
            get
            {
                return (BossState)npc.ai[2];
            }
            set
            {
                npc.ai[2] = (float)value;
            }
        }

        enum BossState
        {
            Idle,
            Teleport,
            RingAround,
            DiamondRain,
            Devilsknife
        }
        public bool fleeing = false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 10;
            npc.aiStyle = 0;
        }
        public override void SetDefaults()
        {
            AIBossState = BossState.Idle;
            npc.boss = true;
            npc.width = 92;
            npc.height = 96;
            npc.noGravity = true;
            npc.lifeMax = 34000;
            npc.life = npc.lifeMax;
            //music = SoundLoader.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/TheWorldRevolving");
            if (Main.netMode != NetmodeID.Server && !Main.dedServ)
			{
                music = Epicosity.inst.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/TheWorldRevolving").Style;
			}
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)AIBossState);
            writer.WriteVector2(targetPosition);
            writer.Write(stateTimer);
            writer.Write(currentPhase);
            writer.Write(teleportTimer);
            writer.WriteVector2(syncedTargetPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIBossState = (BossState)reader.ReadInt32();
            targetPosition = reader.ReadVector2();
            stateTimer = reader.ReadInt32();
            currentPhase = reader.ReadInt32();
            teleportTimer = reader.ReadInt32();
            syncedTargetPosition = reader.ReadVector2();
        }
        public override void AI()
        {
            stateTimer++;
            if (!fleeing)
            {
                npc.TargetClosest();
                // Apply vertigo
                if (Main.netMode != NetmodeID.Server)
				{
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<Vertigo>(), 2);
				}
                if (AIBossState == BossState.Idle)
                {
                    if ((float)npc.life / (float)npc.lifeMax < 0.5f)
                    {
                        waveTimer += 0.02f;
                        idleRelativePosition = new Vector2((float)(Math.Sin(waveTimer)) * 300, -300);
                        waveTimer %= 6.28f;
                    }
                    targetPosition = Main.player[npc.target].position + idleRelativePosition;
                    npc.position = (npc.position * 10f + targetPosition) / 11f;
                    if (stateTimer >= 220 - 100 * (float)(1 - ((float)npc.life / (float)npc.lifeMax))) // Speeds up as battle continues
                    {
                        int randSound = (int)(Main.rand.NextFloat() * 8);
                        string soundPath = "";
                        switch (randSound)
                        {
                            case 0:
                                soundPath = "Sounds/NPCs/JevilChaos";
                                break;
                            case 1:
                                soundPath = "Sounds/NPCs/JevilAnything";
                                break;
                            case 2:
                                soundPath = "Sounds/NPCs/JevilByeBye";
                                break;
                            default:
                                break;
                        }
                        if (soundPath != "")
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, soundPath).WithVolume(1f), npc.position);
                            }
                        }
                        stateTimer = 0;
                        // Non-randomized for multiplayer support
                        currentPhase++;
                        if (currentPhase > 3)
                        {
                            currentPhase = 1;
                        }
                        AIBossState = (BossState)currentPhase;
                    }
                }
                if (AIBossState == BossState.DiamondRain)
                {
                    targetPosition = Main.player[npc.target].position + idleRelativePosition;
                    npc.position = (npc.position * 10f + targetPosition) / 11f;
                    if (stateTimer % 10 == 0)
                    {
                        Vector2 position = Main.player[npc.target].position + Main.player[npc.target].velocity * 30 * (npc.lifeMax - npc.life) / npc.lifeMax;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 projPos = position + new Vector2(Main.rand.NextFloat() * 500 - 250, 300 - 150 * (npc.lifeMax - npc.life) / npc.lifeMax);
                            int proj = Projectile.NewProjectile(projPos,
                                    new Vector2(0, -2),
                                    ModContent.ProjectileType<Projectiles.Enemy.Diamond>(),
                                    85,
                                    2);
                            ((Projectiles.Enemy.Diamond)(Main.projectile[proj].modProjectile)).speed = 1 + 3 * (npc.lifeMax - npc.life) / npc.lifeMax;
                            Main.projectile[proj].netUpdate = true;
                        }
                    }
                    if (stateTimer > 300)
                    {
                        frame = 0;
                        AIBossState = BossState.Idle;
                        stateTimer = 0;
                    }
                }
                if (AIBossState == BossState.RingAround)
                {
                    targetPosition = Main.player[npc.target].position + idleRelativePosition;
                    npc.position = (npc.position * 10f + targetPosition) / 11f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (stateTimer == 0) // Does at first frame of stage
                        {
                            int distance = 300;
                            int numberOfProjectiles = 10 + (int)(15.0f * (float)(1 - ((float)npc.life / (float)npc.lifeMax)));
                            Vector2 position = Main.player[npc.target].position;
                            for (int x = 0; x <= numberOfProjectiles - 1; x++)
                            {
                                Vector2 projPos = position + new Vector2((float)Math.Cos(6.28318530718f / numberOfProjectiles * x) * distance, (float)Math.Sin(6.28318530718f / numberOfProjectiles * x) * distance);
                                int proj = Projectile.NewProjectile(projPos,
                                    (position - projPos).ToRotation().ToRotationVector2(),
                                    ModContent.ProjectileType<Projectiles.Enemy.Spade>(),
                                    80,
                                    2);
                                ((Projectiles.Enemy.Spade)(Main.projectile[proj].modProjectile)).delay = x * (5 * (npc.life / npc.lifeMax) + 5);
                                ((Projectiles.Enemy.Spade)(Main.projectile[proj].modProjectile)).speed = 1 + 3 * (npc.lifeMax - npc.life) / npc.lifeMax;
                                Main.projectile[proj].netUpdate2 = true;
                            }
                        }
                    }
                    if (stateTimer > 100)
                    {
                        stateTimer = 0;
                        AIBossState = BossState.Idle;
                    }
                }
                if (AIBossState == BossState.Teleport)
                {
                    Vector2 position = Main.player[npc.target].position;
                    teleportTimer++;
                    // On first tick, calculate random position first, then send to client and set it as the actual targetPosition later
                    
                    if (teleportTimer == 50 - 25 * (npc.lifeMax - npc.life) / npc.lifeMax - 5 // Execute this 5 ticks before the actual teleport 
                        && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        syncedTargetPosition = position + new Vector2(Main.rand.NextFloat() * 500 - 500, Main.rand.NextFloat() * 500 - 500);
                        npc.netUpdate = true; // Send to client
                    }

                    if (teleportTimer > 50 - 25 * (npc.lifeMax - npc.life) / npc.lifeMax)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/JevilTeleport").WithVolume(0.5f), npc.position);
                        }
                        frame = 0;
                        teleportTimer = 0;
                        targetPosition = syncedTargetPosition; // Set on both server and client since both would have had the same syncedTargetPosition at this point
                    }
                    
                    npc.position = (npc.position * 10f + targetPosition) / 11f;
                    if (teleportTimer == 25) // Perform shoot
                    {
                        frame = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int x = 0; x <= 4; x++)
                            {
                                Vector2 projPos = npc.Top + new Vector2(0, 50);
                                int proj = Projectile.NewProjectile(projPos,
                                    ((position - projPos).ToRotation() + (x - 3) * 0.2f).ToRotationVector2() * 7,
                                    ModContent.ProjectileType<Projectiles.Enemy.Club>(),
                                    75,
                                    2); // No need for netUpdate since the club is simple
                            }
                        }
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/JevilOh").WithVolume(1f), npc.position);
                        }
                    }
                    if (stateTimer > 300)
                    {
                        if (npc.position.X < position.X)
                        {
                            idleRelativePosition = new Vector2(-300, -300);
                        }
                        else
                        {
                            idleRelativePosition = new Vector2(300, -300);
                        }
                        frame = 0;
                        AIBossState = BossState.Idle;
                        stateTimer = 0;
                        npc.netUpdate = true;
                    }
                }
            }
            #region Check if all players are dead
            int checkAlive = 0;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    checkAlive++;
                }
            }
            //mod.Logger.Info(checkAlive);
            if (checkAlive == 0 && !fleeing)
            {
                fleeing = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/JevilByeBye").WithVolume(1f), npc.position);
                }
            }
            #endregion
            if (fleeing)
            {
                npc.velocity.Y -= 0.1f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            // Change animation half-way
            if ((AIBossState == BossState.Idle || AIBossState == BossState.RingAround || AIBossState == BossState.DiamondRain) && (float)npc.life / (float)npc.lifeMax < 0.5f)
            {
                frameTimer++;
                if (frameTimer > 4)
                {
                    frameTimer = 0;
                    frame++;
                    if (frame < 2)
                    {
                        frame = 2;
                    }
                    if (frame > 9)
                    {
                        frame = 2;
                    }
                }
            }
            // Hover animation
            npc.frame.Y = frameHeight * frame;
            hoverAnimationFrame += 0.05f;
            hoverAnimationFrame %= 6.28318530718f;
            npc.visualOffset = new Vector2(0, (float)Math.Sin(hoverAnimationFrame) * 15f);
        }
    }
}
