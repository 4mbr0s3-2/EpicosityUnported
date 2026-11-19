using Epicosity.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Epicosity.UI.SpellCard
{
	public class IceSignIcicleFallUIState : SpellCardUIState
	{
		public override string GetSpellName()
		{
			return "Ice Sign \"Icicle Fall\"";
		}

		public override Texture2D GetUserTexture()
		{
			return ModContent.GetTexture($"{PathConstants.PathUI}SpellCard/Cirno");
		}

		protected override Texture2D GetBackgroundTexture()
		{
			return ModContent.GetTexture($"{PathConstants.PathUI}SpellCard/CirnoBG");
		}
	}
}
