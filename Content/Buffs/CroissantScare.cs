using Terraria.ModLoader;

namespace Epicosity.Buffs
{
	public class CroissantScare : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Croissant Scare");
            Description.SetDefault("Items randomly fall out of inventory when hit\n'Stop! I could've dropped my croissant!'");
        }
    }
}
