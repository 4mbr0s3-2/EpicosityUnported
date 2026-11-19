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
using Epicosity.Buffs;

namespace Epicosity.NPCs.DummyInvasion
{
    class FrenchDummy : RogueDummy
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("French Dummy");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void NPCLoot()
        {
            base.NPCLoot();
            if (Main.netMode != NetmodeID.MultiplayerClient) Item.NewItem(npc.getRect(), mod.ItemType("Crossiant"), Main.rand.Next(1, 3));
        }
        public override void AI()
        {
            base.AI();
            foreach (Player player in Main.player)
            {
                if (player.difficulty < 1)
                {
                    return;
                }
                int range = 300;
                if ((player.Center - npc.Center).Length() < range)
                {
                    player.AddBuff(ModContent.BuffType<CroissantScare>(), 180);
                }
            }
        }
    }
}
