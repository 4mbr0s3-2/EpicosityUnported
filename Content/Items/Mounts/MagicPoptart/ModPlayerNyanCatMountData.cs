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
    public class ModPlayerNyanCatMountData : ModPlayer
    {
        public float idleCatFrameCounter = 0;
        public float catFrame = 0;
        public List<Vector2> oldRainbowPosition = new List<Vector2>();
    }
}
