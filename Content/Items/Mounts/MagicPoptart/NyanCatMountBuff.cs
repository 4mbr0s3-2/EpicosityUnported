using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
namespace Epicosity.Buffs
{
	public class NyanCatMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Nyan Cat");
            Description.SetDefault("Nyan nyan nyan nyan nyan nyan nyan nyan nyan nyan\nnyan nyan nyan nyan nyan nyan nyan nyan nyan nyan nyan");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<Mounts.NyanCatMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
