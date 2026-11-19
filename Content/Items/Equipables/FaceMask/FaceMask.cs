using Terraria.ModLoader;

namespace Epicosity.Items.Equipables.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class FaceMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Face Mask");
        }
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = true;
        }
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 10;
            item.vanity = true;
        }
    }
}
