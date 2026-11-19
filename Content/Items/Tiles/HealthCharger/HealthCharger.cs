using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Epicosity.Tiles
{
	public class HealthCharger : ModTile
    {
		public override bool HasSmartInteract()
		{
			return true;
		}
		public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            // This hook is needed to sync the placement of the tile entity
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEHealthCharger>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(Terraria.Enums.AnchorType.None, 0, 0);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(201, 47, 45));
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // TODO: Glow mask
            base.PostDraw(i, j, spriteBatch);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
			Point16 point = new Point16(i + 1, j + 2); // Center
            Vector2 worldCoordinates = point.ToWorldCoordinates();
            int index = ModContent.GetInstance<TEHealthCharger>().Find(i, j);
            if (index == -1)
            {
                Main.NewText("Can't find tile entity!");
                return;
            }
            TEHealthCharger tileEntity = TileEntity.ByID[index] as TEHealthCharger;
            int itemIndex = Item.NewItem(new Rectangle((int)worldCoordinates.X, (int)worldCoordinates.Y, 1, 1), ModContent.ItemType<HealthChargerItem>());
            (Main.item[itemIndex].modItem as HealthChargerItem).healthStored = tileEntity.healthStored;
            ModContent.GetInstance<TEHealthCharger>().Kill(i, j);
        }
        public override void MouseOver(int i, int j)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Tile tile = Main.tile[i, j];
                int left = i - tile.frameX / 18;
                int top = j - tile.frameY / 18;

                int index = ModContent.GetInstance<TEHealthCharger>().Find(left, top);
                if (index == -1)
                {
                    Main.NewText("Can't find tile entity!");
                    return;
                }
                TEHealthCharger tileEntity = TileEntity.ByID[index] as TEHealthCharger;
                Main.LocalPlayer.showItemIcon = true;
                Main.LocalPlayer.showItemIcon2 = -1;
                Main.LocalPlayer.showItemIconText = $"[i:58]{tileEntity.healthStored}";
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            // Once placed, change the health stored and sync
            Tile tile = Main.tile[i, j];
            int left = i - tile.frameX / 18;
            int top = j - tile.frameY / 18;
            int index = ModContent.GetInstance<TEHealthCharger>().Find(left, top);
            if (index == -1)
            {
                Main.NewText("Can't find tile entity!");
                return;
            }
            TEHealthCharger tileEntity = ((TEHealthCharger)TileEntity.ByID[index]);
            tileEntity.healthStored = ((HealthChargerItem)item.modItem).healthStored;
            if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, index);
            }
        }
        public override bool NewRightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.frameX / 18;
            int top = j - tile.frameY / 18;

            int index = ModContent.GetInstance<TEHealthCharger>().Find(left, top);
            if (index == -1)
            {
                return false;
            }
            TEHealthCharger tileEntity = ((TEHealthCharger)TileEntity.ByID[index]);
            if (Main.LocalPlayer.HeldItem.healLife > 0)
            {
                if (tileEntity.healthStored < 500)
                {
                    Main.LocalPlayer.HeldItem.stack--;
                }
                tileEntity.healthStored += Main.LocalPlayer.HeldItem.healLife;
                if (tileEntity.healthStored > 500)
                {
                    tileEntity.healthStored = 500;
                }
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, index);
            }
            return true;
        }
    }
    public class HealthChargerItem : ModItem
    {
        public override bool CloneNewInstances => true;
        public int healthStored = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Health Charger");
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "HealthStored", $"Health stored: [i:58] {healthStored}"));
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(healthStored);
        }
        public override void NetRecieve(BinaryReader reader)
        {
            healthStored = reader.ReadInt32();
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.createTile = ModContent.TileType<HealthCharger>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 15;
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
            item.maxStack = 99;
        }
    }
}
