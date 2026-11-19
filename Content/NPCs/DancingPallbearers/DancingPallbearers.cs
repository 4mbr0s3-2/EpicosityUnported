using Epicosity.Items.Equipables.Accessories;
using System;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs
{
	// TODO: Rework everything here, it's pretty old code
	public class ModPlayerDancingPallbearersSpawn : ModPlayer
    {
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            bool spawn = false;
            for (int x = 0; x < 10; x++)
            {
                Item item = player.armor[x];
                if (item.type == ModContent.ItemType<PallbearerHandkerchief>() && item.stack > 0)
                {
                    item.stack -= 1;
                    spawn = true;
                    break;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && spawn) // Spawns on the server
            {
                int npcIndex = NPC.NewNPC((int)player.position.X, (int)player.position.Y, ModContent.NPCType<DancingPallbearers>());
                ((DancingPallbearers)Main.npc[npcIndex].modNPC).owner = Main.myPlayer;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EasterEggs/Astronomia"));
            }
        }
    }
    public class DancingPallbearers : ModNPC
    {
        // TODO: Fix bug where they don't spawn
        int frame = 0;
        public int owner = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dancing Pallbearers");
            Main.npcFrameCount[npc.type] = 21;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 3;
            npc.dontTakeDamage = true;
            npc.lifeMax = 10000;
            npc.friendly = true;
            npc.width = 104;
            npc.height = 62;
            npc.knockBackResist = 0;
        }
        public override void AI()
        {
            if (owner <= 0)
            {
                return;
            }
            if (Main.player[owner].respawnTimer == 1) // Run on all other players and server
            {
                Main.player[owner].ChangeSpawn((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
            }
            if (Main.player[owner].respawnTimer <= 0)
            {
                Main.player[owner].RemoveSpawn();
                npc.active = false;
                for (int i = 0; i < 50; i++)
                {
                    int dustIndex = Dust.NewDust(new Microsoft.Xna.Framework.Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 16, 0f, 0f, 100, Colors.RarityOrange, 2f);
                    Main.dust[dustIndex].velocity *= 1.4f;
                }
            }
            //npc.velocity.X += npc.direction;
            //npc.TargetClosest();
            //npc.direction *= -1;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += Math.Abs(npc.velocity.X);
            if (npc.frameCounter >= 5)
            {
                npc.spriteDirection = npc.direction;
                npc.frameCounter = 0;
                if (++frame >= Main.npcFrameCount[npc.type])
                {
                    frame = 0;
                }
            }
            if (Math.Abs(npc.velocity.X) <= 0.001f)
            {
                frame = 0;
            }
            npc.frame.Y = frame * frameHeight;
        }
    }
}
