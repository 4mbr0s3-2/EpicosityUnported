using Epicosity.Systems;
using Epicosity.Systems.MemoryManagement;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Epicosity.UI.SpellCard
{
	public abstract class SpellCardUIState : UIState
	{
		public static UserInterface SpellCardInterface;
		[LoadManager(LoadManagerAttribute.ManageType.Unload)]
		public static void Unload()
		{
			SpellCardInterface = null;
		}
		SoundEffectInstance sound;
		Coroutine userCoroutine;
		Coroutine nameCoroutine;
		private Coroutine bgCoroutine;
		Texture2D cachedUserTexture;
		Texture2D cachedUserBackground;
		Texture2D nameBackground;
		RenderTarget2D spellNameTex;
		string cachedSpellName;
		public abstract Texture2D GetUserTexture();
		public abstract string GetSpellName();
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		public override void OnActivate()
		{
			nameBackground = ModContent.GetTexture($"{PathConstants.PathUI}SpellCard/SpellCardNameBG");
			sound = ModContent.GetSound($"{nameof(Epicosity)}/Sounds/Item/Weapons/Magic/SpellCard").CreateInstance();
			sound.Volume = Main.soundVolume * 0.5f;
			Main.PlaySoundInstance(sound);
			cachedUserTexture = GetUserTexture();
			cachedUserBackground = GetBackgroundTexture();
			cachedSpellName = GetSpellName();
			userCoroutine = new Coroutine(UserSequence());
			nameCoroutine = new Coroutine(NameSequence());
			bgCoroutine = new Coroutine(BGSequence());
			spellNameTex = RenderTag();
		}

		protected abstract Texture2D GetBackgroundTexture();

		public RenderTarget2D RenderTag()
		{
			float scale = 0.5f;
			int padding = 15;
			Vector2 nameSize = Main.fontDeathText.MeasureString(cachedSpellName);
			RenderTarget2D target = new RenderTarget2D(Main.instance.GraphicsDevice, (int)(nameSize.X * scale + padding * 2), (int)(nameSize.Y * scale), mipMap: false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			Main.graphics.GraphicsDevice.SetRenderTarget(target);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin();
			Main.spriteBatch.Draw(nameBackground,
					new Rectangle(0, 0, (int)(nameSize.X + padding * 2), nameBackground.Height * 2),
					null,
					Color.White * 0.7f,
					0,
					Vector2.Zero,
					SpriteEffects.None,
					0);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, cachedSpellName, new Vector2(padding, 0), Color.White, 0, Vector2.Zero, Vector2.One * scale);
			Main.spriteBatch.End();
			Main.graphics.GraphicsDevice.SetRenderTarget(null);
			return target;
		}

		public IEnumerator NameSequence()
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			float margin = 100;
			Vector2 barOffset = new Vector2(-100, 100);
			// Fade in
			for (float i = 0; i < 0.5f; i += 1 / 60f)
			{
				float progress = Easings.EaseOutQuad(i / 0.5f);
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2,
					Main.screenHeight / 2) + barOffset;
				spriteBatch.Draw(spellNameTex,
					portraitPos,
					null,
					Color.White * progress,
					0,
					new Vector2(spellNameTex.Width / 3, spellNameTex.Height / 2),
					3 - progress * 2,
					SpriteEffects.None,
					0);
				yield return null;
			}
			// Hold
			for (float i = 0; i < 1.1f; i += 1 / 60f)
			{
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2,
					Main.screenHeight / 2) + barOffset;
				spriteBatch.Draw(spellNameTex,
					portraitPos,
					null,
					Color.White,
					0,
					new Vector2(spellNameTex.Width / 3, spellNameTex.Height / 2),
					1,
					SpriteEffects.None,
					0);
				yield return null;
			}
			// Move up out of screen
			for (float i = 0; i < 1f; i += 1 / 60f)
			{
				float progress = Easings.EaseInQuad(i);
				float offScreen = Main.screenHeight * progress;
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2,
					Main.screenHeight / 2 - offScreen) + barOffset;
				spriteBatch.Draw(spellNameTex,
					portraitPos,
					null,
					Color.White * (1 - progress * 2),
					0,
					new Vector2(spellNameTex.Width / 3, spellNameTex.Height / 2),
					1,
					SpriteEffects.None,
					0);
				yield return null;
			}
			SpellCardInterface.SetState(null);
			yield break;
		}
		public IEnumerator BGSequence()
		{
			float upOffset = 0;
			SpriteBatch spriteBatch = Main.spriteBatch;
			float scale = 3;
			for (float i = 0; i < 0.5f; i += 1 / 60f)
			{
				float progress = Easings.EaseOutQuad(i / 0.5f);
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - cachedUserBackground.Width * scale + 20 + 150 * (1 - progress),
					-upOffset);
				for (int j = 0; j < Main.screenHeight / cachedUserBackground.Height / scale + 2; j++)
				{
					spriteBatch.Draw(cachedUserBackground,
						 portraitPos + new Vector2(0, cachedUserBackground.Height * scale) * j,
						 null,
						 Color.White * 0.5f * progress,
						 0,
						 Vector2.Zero,
						 scale,
						 SpriteEffects.None,
						 0);
				}
				upOffset++;
				yield return null;
			}
			for (float i = 0; i < 1.3f; i += 1 / 60f)
			{
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - cachedUserBackground.Width * scale + 20,
					-upOffset);
				for (int j = 0; j < Main.screenHeight / cachedUserBackground.Height / scale + 2; j++)
				{
					spriteBatch.Draw(cachedUserBackground,
						 portraitPos + new Vector2(0, cachedUserBackground.Height * scale) * j,
						 null,
						 Color.White * 0.5f,
						 0,
						 Vector2.Zero,
						 scale,
						 SpriteEffects.None,
						 0);
				}
				upOffset++;
				yield return null;
			}
			for (float i = 0; i < 0.5f; i += 1 / 60f)
			{
				float progress = Easings.EaseInQuad(i / 0.5f);
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - cachedUserBackground.Width * scale + 20,
					-upOffset);
				for (int j = 0; j < Main.screenHeight / cachedUserBackground.Height / scale + 2; j++)
				{
					spriteBatch.Draw(cachedUserBackground,
						 portraitPos + new Vector2(0, cachedUserBackground.Height * scale) * j,
						 null,
						 Color.White * 0.5f * (1 - progress),
						 0,
						 Vector2.Zero,
						 scale,
						 SpriteEffects.None,
						 0);
				}
				upOffset++;
				yield return null;
			}
		}
		public override void OnInitialize()
		{
		}
		public IEnumerator UserSequence()
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			float margin = 100;
			// Transition
			for (float i = 0; i < 0.5f; i += 1 / 60f)
			{
				float progress = Easings.EaseOutQuad(i / 0.5f);
				float offScreen = 700 * (1 - progress);
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2 + offScreen,
					Main.screenHeight / 2);
				spriteBatch.Draw(cachedUserTexture,
					 portraitPos,
					 null,
					 Color.White,
					 0,
					 cachedUserTexture.Size() / 2,
					 1.5f,
					 SpriteEffects.None,
					 0);
				yield return null;
			}
			// Hold
			for (float i = 0; i < 1f; i += 1 / 60f)
			{
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2,
					Main.screenHeight / 2);
				spriteBatch.Draw(cachedUserTexture,
					portraitPos,
					null,
					Color.White,
					0,
					cachedUserTexture.Size() / 2,
					1.5f,
					SpriteEffects.None,
					0);
				yield return null;
			}
			// Fade
			for (float i = 0; i < 0.5f; i += 1 / 60f)
			{
				float progress = i / 0.5f;
				Vector2 portraitPos = new Vector2(
					Main.screenWidth - margin - cachedUserTexture.Width / 2,
					Main.screenHeight / 2);
				spriteBatch.Draw(
					cachedUserTexture,
					portraitPos,
					null,
					Color.White * (1 - progress),
					0,
					cachedUserTexture.Size() / 2,
					1.5f + progress * 2,
					SpriteEffects.None,
					0);
				yield return null;
			}
			yield break;
		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			bgCoroutine.Update();
			userCoroutine.Update();
			nameCoroutine.Update();
		}
	}
}
