using Epicosity.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
    public class SawBlade : ChargeUpPizzaCutterBase
    {
        public override string Texture => PathConstants.PlaceholderTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.width = 32;
            item.height = 32;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.damage = 80;
        }
    }
}
