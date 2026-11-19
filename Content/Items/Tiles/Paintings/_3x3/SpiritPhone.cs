using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Epicosity.Tiles.Paintings._3x3
{
    public class SpiritPhone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Microsoft.Xna.Framework.Color(121, 34, 21));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Terraria.DataStructures.Point16 point = new Terraria.DataStructures.Point16(i + 1, j + 1); // Center
            Vector2 worldCoordinates = point.ToWorldCoordinates();
            Item.NewItem(new Microsoft.Xna.Framework.Rectangle((int)worldCoordinates.X, (int)worldCoordinates.Y, 1, 1), ModContent.ItemType<SpiritPhoneItem>());
        }
    }
    public class SpiritPhoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Phone");
            Tooltip.SetDefault("'N. Cicierega'");
        }
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.createTile = ModContent.TileType<SpiritPhone>();
            item.useStyle = Terraria.ID.ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 15;
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
            item.maxStack = 99;
        }
    }
}
