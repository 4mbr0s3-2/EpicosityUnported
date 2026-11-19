using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs.DummyInvasion
{
    class PyroDummy : RogueDummy
    {
        float shootCounter = 0;
        int shootTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pyro Dummy");
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            npc.lifeMax = 1000;
        }
        public override void NPCLoot()
        {
            base.NPCLoot();
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.Next(0, 15) == 0) Item.NewItem(npc.getRect(), ItemID.Flamethrower);
        }
        public override void AI()
        {
            base.AI();
            shootTimer++;
            if (shootTimer > 60)
            {
                shootCounter++;
                if (shootCounter > 5 && npc.HasValidTarget)
                {
                    shootCounter = 0;
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && (Main.player[npc.target].Center - npc.Center).Length() <= 500)
                    {
                        Microsoft.Xna.Framework.Vector2 direction = Main.player[npc.target].Center - npc.Center;
                        direction.Normalize();
                        Main.PlaySound(SoundID.Item34, npc.position);
                        int projIndex = Projectile.NewProjectile(npc.Center, direction * 7, ProjectileID.Flames, 60, 4);
                        Main.projectile[projIndex].hostile = true;
                        Main.projectile[projIndex].friendly = false;
                    }
                }
            }
            else if (shootTimer > 90)
            {
                shootTimer = 0;
            }
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
            npc.spriteDirection = (int)(npc.velocity.X / Math.Abs(npc.velocity.X));
            npc.frame.Y = (int)frame * frameHeight;
        }
    }
}
