using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Epicosity.NPCs.Bosses
{
    class DespacitoSpiderDownedWorld : ModWorld
    {
        public static bool downedDespacitoSpider = false;
        public override void Initialize()
        {
            downedDespacitoSpider = false;
        }
        public override TagCompound Save()
        {
            return new TagCompound {
                {"downedDespacitoSpider", downedDespacitoSpider}
            };
        }
        public override void Load(TagCompound tag)
        {
            downedDespacitoSpider = tag.GetBool("downedDespacitoSpider");
        }
    }
}
