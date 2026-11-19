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

namespace Epicosity.NPCs.DummyInvasion
{
	public class RogueDummy : ModNPC
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
        protected float maxSpeed = 3.5f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rogue Dummy");
            Main.npcFrameCount[npc.type] = 10;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 400;
            npc.width = 28;
            npc.height = 54;
            npc.aiStyle = 3;
            npc.friendly = false;
            npc.damage = 60;
            npc.defense = 70;
            npc.HitSound = SoundID.NPCHit15;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPCs/DummyPunch");
            npc.knockBackResist = 0.001f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(acceleration);
            writer.Write(maxSpeed);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            acceleration = reader.ReadSingle();
            maxSpeed = reader.ReadSingle();
        }
        public override bool PreAI()
        {
            return true;
        }
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !initialized)
            {
                acceleration = Main.rand.Next(10, 30) / 100f;
                maxSpeed = Main.rand.Next(30, 50) / 10f;
                npc.netUpdate = true;
                initialized = true;
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
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            if (speed < -maxSpeed)
            {
                speed = -maxSpeed;
            }
            npc.velocity.X = speed;
            //npc.stepSpeed = 3;
            //npc.position.X += npc.velocity.X;

        }
        public override void NPCLoot()
        {
            Projectile.NewProjectile(npc.position, new Microsoft.Xna.Framework.Vector2(-npc.velocity.X * 2, 0) + new Microsoft.Xna.Framework.Vector2(0, -Main.rand.Next(7, 12)), ModContent.ProjectileType<Projectiles.Enemy.DeadDummy>(), 0, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            if (Math.Abs(npc.velocity.X) > 0.01f)
            {
                npc.frameCounter++;
                int frameCounterThreshold = Math.Min(5, (int)Math.Abs(npc.velocity.X));
                if (npc.frameCounter >= 6 - frameCounterThreshold)
                {
                    npc.frameCounter = 0;
                    frame += 1;
                    if (frame >= Main.npcFrameCount[npc.type])
                    {
                        frame = 0;
                    }
                }
            }
            else
            {
                frame = 0;
            }
            if (Math.Abs(npc.velocity.Y) > 0.1f)
            {
                frame = 5;
            }
            npc.spriteDirection = (int)(npc.velocity.X / Math.Abs(npc.velocity.X));
            npc.frame.Y = (int)frame * frameHeight;
        }
    }
}
