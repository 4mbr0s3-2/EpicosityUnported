using Epicosity.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Utilities
{
	public class TileHelper : GlobalTile
	{
		public static bool dropTile = true;
		public override bool Drop(int i, int j, int type)
		{
			if (dropTile)
			{
				return true;
			}
			return false;
		}
		public static void DestroyFrameImportantTilesWithoutDrops(int owner, int i, int j)
		{
			Tile tile = Main.tile[i, j];
			tile.active(false);
			tile.halfBrick(false);
			tile.frameX = -1;
			tile.frameY = -1;
			tile.color(0);
			tile.frameNumber(0);

			dropTile = false;
			// Skips kill tile, which drops the table item
			// This updates the rest of the tiles and the tiles adjacent, removing all FrameImportant tiles without drops
			WorldGen.SquareTileFrame(i, j, true);
			dropTile = true;
		}
		public static Tile ActuallyGetTileSafely(Vector2 position)
		{
			position /= 16f;
			return ActuallyGetTileSafely((int)position.X, (int)position.Y);
		}

		public static Tile ActuallyGetTileSafely(Point pt) => ActuallyGetTileSafely(pt.X, pt.Y);
		public static Tile ActuallyGetTileSafely(Point16 pt) => ActuallyGetTileSafely(pt.X, pt.Y);

		public static Tile ActuallyGetTileSafely(int i, int j)
		{
			if (i < 0 || i > Main.maxTilesX - 1 || j < 0 || j > Main.maxTilesY - 1)
			{
				Tile outOfIndexTile = new Tile();
				return outOfIndexTile;
			}

			return Framing.GetTileSafely(i, j);
		}
		public static Point GetTopLeftOfFrameImportant(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.frameX / 18;
			int top = j - tile.frameY / 18;
			return new Point(left, top);
		}
		public static void RenderSmoothLightingTile(SpriteBatch spriteBatch, Texture2D texture, int i, int j)
		{
			// TODO: Reverse engineer smooth lighting
		}
    }
}
