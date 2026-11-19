using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Epicosity.Mounts
{
    public class ModPlayerClownBallData : ModPlayer
    {
        public float runSpeed = 0;
        public float dashSpeed = 0;
        public double legFrameCounter;
        public int legFrame = 0;
        public int ballFrame = 0;
        public override void PostUpdateRunSpeeds()
        {
            if (player.mount._type == ModContent.MountType<ClownBallMount>() && player.mount.Active)
            {
                player.maxRunSpeed = runSpeed;
                player.accRunSpeed = dashSpeed;
            }
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.mount._type == ModContent.MountType<ClownBallMount>() && player.mount.Active)
            {
                if (damageSource.SourceOtherIndex == 0)
                {
                    player.velocity.Y = -10;
                    return false;
                }
            }
            return true;
        }
    }
}
