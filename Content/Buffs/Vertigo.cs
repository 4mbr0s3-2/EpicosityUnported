using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Buffs
{
	public class Vertigo : ModBuff
    {
        /// <summary>
        /// Local variable used in order to check if the vertigo effect passes.
        /// </summary>
        public static bool vertigoLastFrame = false;
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Vertigo");
            Description.SetDefault("The world is spinning, spinning...");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
