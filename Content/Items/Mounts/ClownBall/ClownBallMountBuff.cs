using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
namespace Epicosity.Buffs
{
    public class ClownBallMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Clown Ball");
            Description.SetDefault("Bonus points if you play \"Entry of the Gladiators\" by Julius Fucik!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<Mounts.ClownBallMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
