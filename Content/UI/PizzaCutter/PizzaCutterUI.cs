using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using Epicosity.Items.Weapons.Melee.PizzaCutter;
using Epicosity.Systems.MemoryManagement;

namespace Epicosity.UI.PizzaCutter
{
    public static class PizzaCutterUI
    {
        public static UserInterface PizzaCutterInterface;
        public static PizzaCutterUIState CurrentPizzaCutterUIState;
        [LoadManager(LoadManagerAttribute.ManageType.Unload)]
        public static void Unload()
        {
            PizzaCutterInterface = null;
            CurrentPizzaCutterUIState = null;
        }
        public static void ShowPizzaCutterUI()
        {
            PizzaCutterInterface.SetState(CurrentPizzaCutterUIState);
        }
        public static void HidePizzaCutterUI()
        {
            PizzaCutterInterface.SetState(null);
        }
        public static Item GetHeldPizzaCutter()
        {
            Item heldItem = Main.LocalPlayer.HeldItem;
            if (heldItem != null)
            {
                if (heldItem.modItem != null)
                {
                    if (heldItem.modItem as ChargeUpPizzaCutterBase != null)
                    {
                        return heldItem;
                    }
                }
            }
            return null;
        }
        public class ModPlayerPizzaCutter : ModPlayer
        {
            public float trueWheelSpeed = 0;
            public float dynamicWheelSpeed = 0;
            bool lastRightClick = false;
            public override void PreUpdate()
            {
            }
            public override void PostUpdate()
            {
                trueWheelSpeed = dynamicWheelSpeed;
                dynamicWheelSpeed -= 0.004f;
                if (trueWheelSpeed < 0)
                {
                    trueWheelSpeed = 0;
                }
                if (trueWheelSpeed > 1)
                {
                    trueWheelSpeed = 1;
                }
                if (dynamicWheelSpeed < 0)
                {
                    dynamicWheelSpeed = 0;
                }
                if (dynamicWheelSpeed > 1.2f)
                {
                    dynamicWheelSpeed = 1.2f;
                }

                bool hasChargeUpPizzaCutter = false;

                Item heldItem = GetHeldPizzaCutter();
                if (heldItem != null)
                {
                    hasChargeUpPizzaCutter = true;
                }

                if (hasChargeUpPizzaCutter)
                {
                    ShowPizzaCutterUI();
                }
                else
                {
                    HidePizzaCutterUI();
                }

                if (!lastRightClick && player.controlUseTile && hasChargeUpPizzaCutter)
                {
                    float chargeUpAmount = ((ChargeUpPizzaCutterBase)heldItem.modItem).chargeUpIncrement;
                    player.GetModPlayer<ModPlayerPizzaCutter>().dynamicWheelSpeed += chargeUpAmount;
                }
                lastRightClick = player.controlUseTile;
            }
        }
        public class PizzaCutterUIState : UIState
        {
            public static Texture2D handleTexture;
            public static Texture2D wheelTexture;
            public static Vector2 UIPosition = Vector2.Zero;
            float wheelFrame = 0;
            public override void OnInitialize()
            {
                handleTexture = ModContent.GetTexture("Epicosity/UI/PizzaCutter/UIHandle");
                wheelTexture = ModContent.GetTexture("Epicosity/UI/PizzaCutter/UIWheel");
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                PizzaCutterUIState.UIPosition = Main.LocalPlayer.MountedCenter + new Vector2(0, -60) - Main.screenPosition;
                Item heldItem = GetHeldPizzaCutter();
                if (heldItem == null)
                {
                    return;
                }
                int rpm = (int)(heldItem.damage * Main.LocalPlayer.GetModPlayer<ModPlayerPizzaCutter>().trueWheelSpeed);
                float handleFrame = 13 * Main.LocalPlayer.GetModPlayer<ModPlayerPizzaCutter>().trueWheelSpeed;
                if (handleFrame > 12)
                {
                    handleFrame = 12;
                }
                else if (handleFrame < 0)
                {
                    handleFrame = 0;
                }
                wheelFrame += rpm * 0.1f;
                if (wheelFrame > 11)
                {
                    wheelFrame = 0;
                }
                else if (wheelFrame < 0)
                {
                    wheelFrame = 0;
                }

                Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow
                (
                    spriteBatch,
                    Main.fontMouseText,
                    rpm + " RPM",
                    UIPosition + new Vector2(0, -30),
                    Color.White,
                    0,
                    Main.fontMouseText.MeasureString(rpm + "RPM") / 2f,
                    new Vector2(1)
                );
                bool flipped = Main.LocalPlayer.direction == -1;
                spriteBatch.Draw
                (
                    handleTexture,
                    new Rectangle
                    (
                        (int)UIPosition.X,
                        (int)UIPosition.Y,
                        32,
                        38
                    ),
                    new Rectangle
                    (
                        0,
                        (int)handleFrame * 38,
                        32,
                        38
                    ),
                    Color.White,
                    0,
                    new Vector2(16, 19),
                    flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0
                );
                spriteBatch.Draw
                (
                    wheelTexture,
                    new Rectangle
                    (
                        (int)UIPosition.X,
                        (int)UIPosition.Y,
                        32,
                        38
                    ),
                    new Rectangle
                    (
                        0,
                        (int)wheelFrame * 38,
                        32,
                        38
                    ),
                    Color.White,
                    0,
                    new Vector2(16, 19),
                    flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0
                );

            }
            public override void Update(GameTime gameTime)
            {
            }
        }
    }
}
