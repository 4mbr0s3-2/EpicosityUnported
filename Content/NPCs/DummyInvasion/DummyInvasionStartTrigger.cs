using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs.DummyInvasion
{
    public class DummyInvasionStartTrigger : GlobalNPC
    {
        double TotalDamage = 0;
        public override bool InstancePerEntity => true;
        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.type == NPCID.TargetDummy && TotalDamage < 100000)
            {
                TotalDamage += damage;
                if (TotalDamage >= 100000)    
                {
                    //npc.active = false;
                    TETrainingDummy.Kill((int)npc.ai[0], (int)npc.ai[1]);

                    TileHelper.DestroyFrameImportantTilesWithoutDrops(Main.myPlayer, (int)npc.ai[0], (int)npc.ai[1]);
                    
                    int dummyZeroIndex = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<RogueDummy>());
                    Main.npc[dummyZeroIndex].Bottom = npc.Bottom;

                    if (Main.invasionType != 0 && Main.invasionSize != 0)
                    {
                        return true;
                    }
                    Invasions.DummyInvasion.Start();
                    Invasions.DummyInvasion.DummyInvasionWarning();
                    if (Main.invasionType == -7) // Successfully started
                    {
                        Main.invasionX = Main.spawnTileX; // Immediately start
                    }
                }
            }
            return true;
        }
    }
}
