using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Epicosity.UI.PizzaCutter.PizzaCutterUI;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Melee.PizzaCutter
{
    public abstract class ChargeUpPizzaCutterBase : ModItem
    {
        public float chargeUpIncrement = 0.1f;
        public float maxRPM = 100;
        public override void SetDefaults()
        {
            item.melee = true;
            item.autoReuse = false;
            item.UseSound = SoundID.Item1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Find((tooltip) => tooltip.Name == "Damage").text = item.damage + " maximum melee damage";
        }
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult = player.GetModPlayer<ModPlayerPizzaCutter>().trueWheelSpeed;
        }
        public void SetChargeUpAmount(float amount)
        {
            chargeUpIncrement = amount;
        }
    }
}
