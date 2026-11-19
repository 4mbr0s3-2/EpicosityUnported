using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Epicosity.Systems.MemoryManagement;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria.ID;

namespace Epicosity.Items.Rarity
{
	public partial class EpicosityItemRarity : GlobalItem
	{
		public static RenderTarget2D rarityRenderTarget;
		public class EpicosityRarityItemText
		{
			public int storedRarity = -1;
		}
		public static EpicosityRarityItemText[] itemText;
		public abstract class EpicosityRarity
		{
			/// <summary>
			/// Return false if you want this method to modify the item's tooltips.
			/// </summary>
			/// <param name="item"></param>
			/// <param name="tooltips"></param>
			/// <returns></returns>
			public virtual bool ModifyTooltips(Item item, List<TooltipLine> tooltips) { return false; }
			public virtual Color? GetColor() { return null; }
			public virtual void PreDrawRarityText() { }
			public virtual void PostDrawRarityText() { }
			public virtual bool HideNameTooltip() { return false; }
		}
		public static Dictionary<int, EpicosityRarity> rarityRegistry;
		public static float rarityAnimationTime = 0;
		[Obsolete]
		public const int PulsatingMagenta = 102;
		[Obsolete]
		public const int PulsatingBlackAndWhite = 103;
		
		public const int EasterEggPastelRainbow = 104;
		public const int WorkInProgress = 404;
		public const int PewDiePie = 105;
		[LoadManager(LoadManagerAttribute.ManageType.Load)]
		public static void Load()
		{
			Main.OnPreDraw += Main_OnPreDraw;
			rarityRegistry = new Dictionary<int, EpicosityRarity>();
			itemText = new EpicosityRarityItemText[20];
			for (int itemTextIndex = 0; itemTextIndex < 20; itemTextIndex++)
			{
				itemText[itemTextIndex] = new EpicosityRarityItemText();
			}
			rarityRegistry.Add(PulsatingMagenta, new RarityPulsatingMagenta());
			rarityRegistry.Add(PulsatingBlackAndWhite, new RarityPulsatingBlackAndWhite());
			rarityRegistry.Add(EasterEggPastelRainbow, new RarityEasterEggPastelRainbow());
			rarityRegistry.Add(WorkInProgress, new RarityWorkInProgress());
			rarityRegistry.Add(PewDiePie, new RarityPewDiePie());
		}

		private static void Main_OnPreDraw(GameTime obj)
		{
			DrawItemRarityTooltipToRenderTarget();
		}

		[LoadManager(LoadManagerAttribute.ManageType.Unload)]
		public static void Unload()
		{
			Main.OnPreDraw -= Main_OnPreDraw;
			itemText = null;
			rarityRegistry = null;
			rarityRenderTarget = null;
		}
		// Utilizes EpicosityRarityItemText in order to store rarity values
		public static void ItemText_NewText(On.Terraria.ItemText.orig_NewText orig, Item newItem, int stack, bool noStack, bool longText)
		{
			if (rarityRegistry.ContainsKey(newItem.rare))
			{
				int itemTextIndex = -1;
				for (int j = 0; j < 20; j++)
				{
					if (!Main.itemText[j].active)
					{
						itemTextIndex = j;
						break;
					}
				}
				if (itemTextIndex < 0)
					return;
				itemText[itemTextIndex].storedRarity = newItem.rare;
			}
			orig(newItem, stack, noStack, longText);
		}
		public static void HookItemTextDraw(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			int matches = 0;
			bool success = cursor.TryGotoNext(MoveType.Before, (i) =>
			{
				bool match = i.MatchCall(typeof(DynamicSpriteFontExtensionMethods).GetMethod("DrawString", new Type[] { typeof(SpriteBatch), typeof(DynamicSpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) }));
				if (match)
				{
					matches++;
					if (matches == 3)
					{
						return true;
					}
				}
				return false;
			});
			if (!success)
			{
				return;
			}
			cursor.Emit(OpCodes.Ldloc, 124); // 124 is the index of local variable num64 (item index); you can find this in dnSpy all the way at the top
			cursor.EmitDelegate<Action<int>>( // We're taking in that int32 that we just put in stack
				(itemTextIndex) =>
				{
					if (rarityRegistry.ContainsKey(itemText[itemTextIndex].storedRarity))
					{
						rarityRegistry.TryGetValue(itemText[itemTextIndex].storedRarity, out EpicosityRarity rarity);
						rarity.PreDrawRarityText();
					}
				}
			);
			cursor.Index++; // Skip over the DrawString... now we're emitting after it
			cursor.Emit(OpCodes.Ldloc, 124); // Load that item index back in the stack
			cursor.EmitDelegate<Action<int>>(
				(itemTextIndex) =>
				{
					if (rarityRegistry.ContainsKey(itemText[itemTextIndex].storedRarity))
					{
						rarityRegistry.TryGetValue(itemText[itemTextIndex].storedRarity, out EpicosityRarity rarity);
						rarity.PostDrawRarityText();
					}
				}
			);
		}
		public static void DrawItemRarityTooltipToRenderTarget()
		{
			rarityRenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, 200, 200, mipMap: false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, Main.instance.GraphicsDevice.PresentationParameters.DepthStencilFormat);
			if (Main.HoverItem.type <= ItemID.None)
			{
				return;
			}
			rarityRegistry.TryGetValue(Main.HoverItem.rare, out EpicosityRarity rarity);
			if (rarity == null || rarity?.HideNameTooltip() == false) return;
			Main.instance.GraphicsDevice.SetRenderTarget(rarityRenderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);

			int lineIndex = 0; // First line is always (or should always be) item name
			int numOfTooltipLines = 20 + Main.HoverItem.ToolTip?.Lines ?? 0;
			string[] text = new string[numOfTooltipLines];
			bool[] modifier = new bool[numOfTooltipLines];
			bool[] badModifier = new bool[numOfTooltipLines];
			string[] tooltipNames = new string[numOfTooltipLines];

			text[0] = Main.HoverItem.HoverName;
			tooltipNames[0] = "ItemName";

			List<TooltipLine> lines = new List<TooltipLine>() { new TooltipLine(Epicosity.inst, tooltipNames[0], text[0]) };
			List<DrawableTooltipLine> drawableLines = lines.Select((parent, index) => new DrawableTooltipLine(parent, index, 0, 0, Color.White)).ToList();
			ChatManager.DrawColorCodedStringWithShadow(
				Main.spriteBatch,
				drawableLines[lineIndex].font,
				drawableLines[lineIndex].text,
				new Vector2(16, 16),
				Color.White, // This will be the base color for our custom shaders for the rarities
				drawableLines[lineIndex].rotation,
				drawableLines[lineIndex].origin,
				drawableLines[lineIndex].baseScale,
				drawableLines[lineIndex].maxWidth,
				drawableLines[lineIndex].spread);

			Main.spriteBatch.End();
			Main.instance.GraphicsDevice.SetRenderTarget(null);

		}
		public static void Update()
		{
			rarityAnimationTime += 1f;
			if (rarityAnimationTime >= 360)
			{
				rarityAnimationTime = 0;
			}
		}
		// TODO: Make more dynamic?
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (rarityRegistry.ContainsKey(item.rare))
			{
				EpicosityRarity rarity;
				rarityRegistry.TryGetValue(item.rare, out rarity);
				Color? rarityColor = rarity.GetColor();
				if (rarityColor != null)
				{
					TooltipLine itemName = tooltips.Find((tooltip) => tooltip.Name == "ItemName");
					itemName.overrideColor = rarityColor;
				}
				rarity.ModifyTooltips(item, tooltips);
			}
		}
		public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
		{
			if (line.Name == "ItemName" && rarityRegistry.ContainsKey(item.rare))
			{
				EpicosityRarity rarity;
				rarityRegistry.TryGetValue(item.rare, out rarity);
				rarity.PreDrawRarityText();
				return !rarity.HideNameTooltip();
			}
			return true;
		}
		public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
		{
			if (line.Name == "ItemName" && rarityRegistry.ContainsKey(item.rare))
			{
				EpicosityRarity rarity;
				rarityRegistry.TryGetValue(item.rare, out rarity);
				rarity.PostDrawRarityText();
			}
		}
	}
}
