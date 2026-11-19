using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
namespace Epicosity.Buffs
{
	public class GDShipMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Geometrical Ship");
            Description.SetDefault("How straight is your straight-fly?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<Mounts.GDShipMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
