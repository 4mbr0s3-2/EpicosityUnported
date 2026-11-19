using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Epicosity.Backgrounds
{
	public class JevilBackgroundShaderData : MiscShaderData
	{
		public JevilBackgroundShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
		{
		}
		public override void Apply(DrawData? drawData = null)
		{
			Ref<Texture2D> _uImage = new Ref<Texture2D>(ModContent.GetTexture("Epicosity/Backgrounds/JevilDistortionMap"));
			GameShaders.Misc[Utilities.ShaderNames.JevilBackgroundDistortion].Shader.Parameters["mapping"].SetValue(_uImage.Value);
			base.Apply(drawData);
		}
	}
}