using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epicosity.NPCs.Bosses
{
    public class FluteTurnip : SmallTurnip
    {
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
            Main.npcFrameCount[npc.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            npc.width = 40;
            npc.height = 50;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 5)
            {
                npc.frameCounter = 0;
                frame++;
                if (frame >= Main.npcFrameCount[npc.type])
                {
                    frame = 0;
                }
            }
            npc.frame.Y = (int)frame * frameHeight;
        }
    }
}
