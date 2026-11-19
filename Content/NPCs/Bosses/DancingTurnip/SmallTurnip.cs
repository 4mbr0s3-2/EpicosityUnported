using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Epicosity.NPCs.Bosses
{
    public abstract class SmallTurnip : ModNPC
    {
        public float relativeRotation = 0;
        public float hoverAnimationFrame = 0;
        public override void SetDefaults()
        {
            npc.aiStyle = 0;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.lifeMax = 1000;
        }
        public override void AI()
        {
            #region Hover animation
            npc.gfxOffY = (float)Math.Sin(hoverAnimationFrame) * 10;
            hoverAnimationFrame += 0.1f;
            hoverAnimationFrame %= MathHelper.TwoPi;
            #endregion
        }
    }
}