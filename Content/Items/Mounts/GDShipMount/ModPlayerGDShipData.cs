using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Epicosity.Mounts
{
    public class ModPlayerGDShipData : ModPlayer
    {
        public float verticalAcceleration = 0;
        public override void PreUpdate()
        {
            if (player.mount._type == ModContent.MountType<GDShipMount>())
            {
                player.maxFallSpeed = 999;
                player.gravity = 0;
                player.fallStart = Terraria.Utils.ToTileCoordinates(player.position).Y;
            }
        }
        public override void PreUpdateMovement()
        {
        }
    }
}
