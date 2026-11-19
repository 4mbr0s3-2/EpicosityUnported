using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Epicosity.Tiles
{
	public class TEHealthCharger : ModTileEntity
    {
        public SoundEffectInstance chargerSound = null;
        public int healthStored = 0;
        public int chargeSoundTimer = 0;
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            // Get top-left-most tile
            return tile.active() && tile.type == ModContent.TileType<HealthCharger>() && tile.frameX == 0 && tile.frameY == 0;
        }
        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag.Add("healthStored", healthStored);
            return tag;
        }
        public override void Update()
        {
            bool hover = Framing.GetTileSafely(Main.MouseWorld).type == ModContent.TileType<HealthCharger>();
            if (!hover)
            {
                return;
            }
            Tile tile = Framing.GetTileSafely(Main.MouseWorld);
            int left = Main.MouseWorld.ToTileCoordinates().X - tile.frameX / 18;
            int top = Main.MouseWorld.ToTileCoordinates().Y - tile.frameY / 18;
            int index = ModContent.GetInstance<TEHealthCharger>().Find(left, top);
            if (index == -1)
            {
                Main.NewText("Can't find tile entity!");
                return;
            }
            TEHealthCharger tileEntity = (ByID[index] as TEHealthCharger);
            if (Main.LocalPlayer.HeldItem.healLife > 0)
            {
                return;
            }
            if (Main.LocalPlayer.controlUseTile && Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2)
            {
                tileEntity.healthStored--;
                if (tileEntity.healthStored <= 0)
                {
                    tileEntity.healthStored = 0;
                    return;
                }
                Main.LocalPlayer.statLife++;
                tileEntity.chargeSoundTimer++;
                if (chargeSoundTimer > 10)
                {
                    chargeSoundTimer = 10;
                }
                #region Charger sound
                if (tileEntity.chargerSound == null || tileEntity.chargerSound?.State == SoundState.Stopped)
                {
                    if (tileEntity.chargeSoundTimer <= 5)
                    {
                        LegacySoundStyle playChargerSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Tile/HealthChargerInitiate");
                        tileEntity.chargerSound = Main.PlaySound(playChargerSound.SoundId, (int)tileEntity.Position.ToWorldCoordinates().X, (int)tileEntity.Position.ToWorldCoordinates().Y, playChargerSound.Style, 1, 0);
                    }
                    else
                    {
                        LegacySoundStyle playChargerSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Tile/HealthChargerPump");
                        tileEntity.chargerSound = Main.PlaySound(playChargerSound.SoundId, (int)tileEntity.Position.ToWorldCoordinates().X, (int)tileEntity.Position.ToWorldCoordinates().Y, playChargerSound.Style, 1, 0);
                    }
                }
                #endregion
                // Important for multiplayer support
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, index);
            }
            else
            {
                if (chargeSoundTimer > 0)
                {
                    chargeSoundTimer--;
                }
            }
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //{
            //             ModNetHandler.GetPacketHandler<TileHelper.TileEntityPlacementSyncHandler>().SendTarget(-1, Main.myPlayer, i, j, Type);
            //}
            Point topLeft = TileHelper.GetTopLeftOfFrameImportant(i, j);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, topLeft.X, topLeft.Y, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, topLeft.X, topLeft.Y, Type, 0f, 0, 0, 0);
            }
            return Place(topLeft.X, topLeft.Y);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            healthStored = reader.ReadInt32();
        }
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(healthStored);
        }
        public override void Load(TagCompound tag)
        {
			try
			{
                healthStored = tag.GetAsInt("healthStored");
            }
            catch (Exception) { }
        }
    }
}
