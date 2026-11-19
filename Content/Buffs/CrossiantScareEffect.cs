using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Epicosity.Buffs
{
	public class CrossiantScareEffect : GlobalItem
	{
		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			try
			{
				if (Main.LocalPlayer.HasBuff(BuffType<CroissantScare>()))
				{
					position.X += Main.rand.Next(-1, 2);
					position.Y += Main.rand.Next(-1, 2);

					Texture2D texture2D3 = Main.itemTexture[item.type];
					spriteBatch.Draw(texture2D3, position, frame, drawColor * 0.5f, 0f, origin, scale, SpriteEffects.None, 0f);
					if (drawColor != Color.Transparent)
					{
						spriteBatch.Draw(texture2D3, position, frame, itemColor * 0.5f, 0f, origin, scale, SpriteEffects.None, 0f);
					}
				}
			}
			catch { }
		}
	}
}
