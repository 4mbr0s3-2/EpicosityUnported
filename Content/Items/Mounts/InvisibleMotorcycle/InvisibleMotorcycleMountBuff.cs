using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
namespace Epicosity.Buffs
{
	public class InvisibleMotorcycleMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Invisible Motorcycle");
            Description.SetDefault("Ding ding!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<Mounts.InvisibleMotorcycleMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
