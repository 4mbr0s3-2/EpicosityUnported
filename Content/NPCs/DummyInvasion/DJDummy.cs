using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using Epicosity.Invasions;

namespace Epicosity.NPCs.DummyInvasion
{
	class DJDummy : ModNPC
    {
        protected float acceleration = 0;
        protected bool initialized = false;
        
        protected float frame
        {
            get
            {
                return npc.localAI[1];
            }
            set
            {
                npc.localAI[1] = value;
            }
        }
        protected float speed = 0;
        public bool Stopped = false;
        public int DJStage = 0;
        public int DJStageBuildup = 0;
        public int NoteGoreTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DJ Dummy");
            Main.npcFrameCount[npc.type] = 24;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 2500;
            npc.width = 28;
            npc.height = 54;
            npc.aiStyle = 3;
            npc.friendly = false;
            npc.damage = 60;
            npc.defense = 70;
            npc.HitSound = SoundID.NPCHit15;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPCs/DummyPunch");
            npc.knockBackResist = 0f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(acceleration);
            writer.Write(DJStage);
            writer.Write(DJStageBuildup);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            acceleration = reader.ReadSingle();
            DJStage = reader.ReadInt32();
            DJStageBuildup = reader.ReadInt32();
        }
        public override bool PreAI()
        {
            return true;
        }
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                acceleration = Main.rand.Next(10, 30) / 100f;
                npc.netUpdate = true;
            }
            npc.TargetClosest();
            if (!Main.player[npc.target].dead)
            {
                if (npc.position.X > Main.player[npc.target].position.X)
                {
                    speed -= acceleration;
                }
                if (npc.position.X < Main.player[npc.target].position.X)
                {
                    speed += acceleration;
                }
            }
            else
            {
                if (speed < 0)
                {
                    speed -= acceleration;
                }
                else
                {
                    speed += acceleration;
                }
            }
            if (speed > 1.5f)
            {
                speed = 1.5f;
            }
            if (speed < -1.5f)
            {
                speed = -1.5f;
            }
            if (npc.Distance(npc.GetTargetData().Center) < 900)
            {
                DJStage = 1;
                DJStageBuildup++;
                if (DJStageBuildup > 300)
                {
                    DJStage = 2;
                    DJStageBuildup = 300;
                }
            }
            else
            {
                DJStage = 0;
                DJStageBuildup = 0;
            }

            if (DJStage >= 1)
            {
                speed = 0;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && Invasions.DummyInvasion.PreHardmodeInvaders.ContainsKey(npc.type) && npc != this.npc && !npc.friendly && Vector2.Distance(npc.Center, base.npc.Center) < 300f)
                    {
                        if (Epicosity.loadedCompatibilityMods.HasFlag(Epicosity.CompatibilityMods.ThoriumMod))
                        {
                            npc.AddBuff(ModLoader.GetMod("ThoriumMod").BuffType("Enrage"), 20, true);
                        }
                    }
                }
                NoteGoreTimer++;
                if (NoteGoreTimer >= 0)
                {
                    NoteGoreTimer = DJStage == 2 ? -15 : -30;
                    if (Epicosity.loadedCompatibilityMods.HasFlag(Epicosity.CompatibilityMods.ThoriumMod))
                    {
                        Gore.NewGoreDirect(npc.Center, Vector2.Zero, ModLoader.GetMod("ThoriumMod").GetGoreSlot("Gores/NoteEffect"), 1f).velocity = new Vector2((float)Main.rand.Next(-2, 3), -3f);
                    }
                }
            }
            if (DJStage == 2)
            {
            }
            npc.velocity.X = speed;
        }
        public override void NPCLoot()
        {
            Projectile.NewProjectile(npc.position, new Microsoft.Xna.Framework.Vector2(-npc.velocity.X * 2, 0) + new Microsoft.Xna.Framework.Vector2(0, -Main.rand.Next(7, 12)), ModContent.ProjectileType<Projectiles.Enemy.DeadDummy>(), 0, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            if (DJStage == 0 && Math.Abs(npc.velocity.X) > 0.01f)
            {
                npc.frameCounter++;
                int frameCounterThreshold = Math.Min(5, (int)Math.Abs(npc.velocity.X));
                if (npc.frameCounter >= 6 - frameCounterThreshold)
                {
                    npc.frameCounter = 0;
                    frame += 1;
                    if (frame >= 10)
                    {
                        frame = 0;
                    }
                }
            }
            else if (DJStage == 1)
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 5)
                {
                    npc.frameCounter = 0;
                    frame += 1;
                    if (frame >= 17)
                    {
                        frame = 10;
                    }
                    else if (frame < 10)
                    {
                        frame = 10;
                    }
                }
            }
            else if (DJStage == 2)
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 3)
                {
                    npc.frameCounter = 0;
                    frame += 1;
                    if (frame >= 23)
                    {
                        frame = 17;
                    }
                    else if (frame < 17)
                    {
                        frame = 17;
                    }
                }
            }
            // Jump frame
            if (Math.Abs(npc.velocity.Y) > 0.1f)
            {
                frame = 5;
            }
            npc.spriteDirection = (int)(npc.velocity.X / Math.Abs(npc.velocity.X));
            npc.frame.Y = (int)frame * frameHeight;
        }
    }
}