using Microsoft.Xna.Framework;
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

namespace Epicosity.NPCs.Bosses
{
    [AutoloadBossHead]
    class DespacitoSpider : ModNPC
    {
        float acceleration = 0;
        int jumpDirection = 0;
        int stateDuration = 0;
        private bool skipSetDuration;
        int stateTimer = 0;
        int frame = 0;
        int jumpTimer = 0;
        double frameCounter = 0;
        public BossState aiState = BossState.InitialLanding;
        public enum BossState
        {
            InitialLanding = 0,
            Crawling = 1,
            Jumping = 2
        }
        bool inAir = false;
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                DespacitoSpiderDownedWorld.downedDespacitoSpider = true;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathpacito Spider");
        }
        public override void SetDefaults()
        {
            npc.damage = Main.hardMode ? 33 : 15;
            npc.knockBackResist = 0.2f;
            Main.npcFrameCount[npc.type] = 14;
            //npc.boss = true;
            npc.width = 171;
            npc.height = 70;
            npc.aiStyle = 0;
            npc.lifeMax = Main.hardMode ? 900 : 600;
            npc.Hitbox = new Rectangle(60, 5, 60, 116);
            npc.visualOffset = new Vector2(0, 70);
            npc.HitSound = new LegacySoundStyle(0, -1);
            npc.life = npc.lifeMax;
            npc.DeathSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/SpiderKill");
            musicPriority = MusicPriority.BossHigh;
            //music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Deathpacito");
            //npc.value = Item.buyPrice(0, 55, 0, 0);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = npc.Bottom;
            return true;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.velocity = (target.Center - npc.Center) / 5;
        }
        public override int SpawnNPC(int tileX, int tileY)
        {
            int baseIndex = NPC.NewNPC(tileX * 16 + 8, tileY * 16, npc.type);
            return baseIndex;
        }
        //void Initialize()
        //{
        //    initialized = true;
        //    int headIndex = NPC.NewNPC((int)npc.position.X + 28, (int)npc.position.Y + 43, ModContent.NPCType<DespacitoSpiderHead>());
        //    //Main.npc[headIndex].position = npc.position;
        //    this.headIndex = headIndex;
        //    ((DespacitoSpiderHead)(Main.npc[headIndex].modNPC)).baseNPCIndex = npc.whoAmI;
        //}
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                writer.Write(stateDuration);
                writer.Write(skipSetDuration);
                writer.Write(stateTimer);
                writer.Write(jumpTimer);
                writer.Write(jumpDirection);
                writer.Write((int)aiState);
                writer.Write(acceleration);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                stateDuration = reader.ReadInt32();
                skipSetDuration = reader.ReadBoolean();
                stateTimer = reader.ReadInt32();
                jumpTimer = reader.ReadInt32();
                jumpDirection = reader.ReadInt32();
                aiState = (BossState)reader.ReadInt32();
                acceleration = reader.Read();
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
            if (aiState == BossState.Crawling)
            {
                frameCounter += (double)Math.Abs(npc.velocity.X) * 1.3;
                if (frameCounter > 15d)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame > 11)
                    {
                        frame = 0;
                    }
                }
            }
            if (aiState == BossState.Jumping)
            {
                if (jumpTimer < 20)
                {
                    frame = 12;
                }
                else
                {
                    frame = 13;
                }
            }
            npc.frame.Y = frame * frameHeight;
        }
        public override void AI()
        {
            npc.target = npc.FindClosestPlayer();
            npc.gfxOffY = 35;
            stateTimer++;
            //mod.Logger.Info((Main.player[npc.target].position - npc.position).Length());
            if (aiState == BossState.InitialLanding && npc.velocity.Y == 0)
            {
                acceleration = 0;
                stateTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    stateDuration = Main.rand.Next(600) + 10;
                    npc.netUpdate = true;
                }
                aiState = BossState.Crawling;
            }
            if (aiState == BossState.Crawling)
            {
                if (npc.velocity.X == 0)
                {
                    npc.velocity.Y += -1f;
                    if (npc.velocity.Y < -3)
                    {
                        npc.velocity.Y = -3;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Main.rand.Next(5) == 0)
                        {
                            aiState = BossState.Jumping;
                            skipSetDuration = true;
                            npc.netUpdate = true;
                        }
                    }
                }
                acceleration += npc.direction / 20.0f;
                if (acceleration > 1)
                {
                    acceleration = 1;
                }
                if (acceleration < -1)
                {
                    acceleration = -1;
                }
                npc.velocity.X = acceleration * (3 + 2 * (1 - (npc.life / npc.lifeMax)));
                if (stateTimer > stateDuration)
                {
                    aiState = BossState.Jumping;
                    stateTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        stateDuration = Main.rand.Next(300) + 3;
                        npc.netUpdate = true;
                    }
                }
            }
            if (aiState == BossState.Jumping)
            {
                jumpTimer++;
                if (jumpTimer >= 20 && !inAir)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/NPCs/SpiderJump").WithVolume(0.5f), npc.position);
                    npc.velocity = new Vector2(0, -7);
                    inAir = true;
                    jumpDirection = npc.direction;
                    npc.netUpdate = true;
                }
                if (inAir)
                {
                    npc.velocity.X = jumpDirection * (4 + 2 * (1 - (npc.life / npc.lifeMax)));
                }
                if (inAir && npc.velocity.Y == 0)
                {
                    inAir = false;
                    jumpTimer = 0;
                }
                if ((stateTimer > stateDuration || skipSetDuration) && !inAir)
                {
                    acceleration = 0;
                    aiState = BossState.Crawling;
                    if (!skipSetDuration)
                    {
                        stateTimer = 0; 
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            stateDuration = Main.rand.Next(600) + 10;
                            npc.netUpdate = true;
                        }
                    }
                    skipSetDuration = false;
                }
            }
        }
    }
}
