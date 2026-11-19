using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Epicosity.Projectiles.Enemy;

namespace Epicosity.NPCs.Bosses
{
    public class SaxTurnip : SmallTurnip
    {
        public int shootTimer = 0;
        public float frame
        {
            get
            {
                return npc.localAI[0];
            }
            set
            {
                npc.localAI[0] = value;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            npc.width = 36;
            npc.height = 58;
        }
        public override void FindFrame(int frameHeight)
        {
        }
		public override void AI()
		{
			base.AI();
            npc.TargetClosest();
            if (!npc.HasValidTarget) return;
            shootTimer++;
            if (shootTimer > 60)
			{
                shootTimer = 0;
                Vector2 shootVector = Main.player[npc.target].Center - npc.Center;
                shootVector = shootVector.SafeNormalize(Vector2.One) * 20;
                Projectile.NewProjectile(npc.Center, shootVector, ModContent.ProjectileType<TurnipWholeNote>(), 100, 2);
			}
		}
	}
}
