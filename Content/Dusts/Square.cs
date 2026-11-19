using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Epicosity.Dusts
{
    public class Square : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            if (dust.scale <= 1.3f)
            {
                dust.scale = Main.rand.Next(10) * 0.1f;
            }
            dust.alpha = 255;
            //dust.color = new Color(230, 100, 0);
        }
        public override bool Update(Dust dust)
        {
            if (dust.alpha > 0)
            {
                dust.alpha -= 55;
            }
            if (dust.scale <= 1.3f)
            {
                dust.position += dust.velocity * 0.3f;
            }
            else
            {
                dust.position += dust.velocity;
            }
            dust.scale -= 0.06f;
            if (dust.scale <= 0)
            {
                dust.active = false;
            }
            float strength = dust.scale / 2f;
            Lighting.AddLight(dust.position, 0.5f * strength, 100f / 255f * 0.5f * strength, 50f / 255f * 0.5f * strength);
            return false;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(255, 100, 50, 0) *  (1 - (dust.alpha / 255f));
        }
    }
}
