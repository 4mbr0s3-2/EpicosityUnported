using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Epicosity.Items.Equipables.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AwesomeFaceMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awesome Face Mask");
            Tooltip.SetDefault("'Balla da li di la!'");
        }
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.vanity = true;
        }
    }
}
