using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;

namespace LevelRogue.UI
{
    public class LevelUISystem : ModSystem
    {
        internal static UserInterface levelInterface;
        internal static LevelRogueUI levelRogueUI;

        private bool uiInitialized = false;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                levelRogueUI = new LevelRogueUI();
                levelInterface = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
		{
			if (!Main.dedServ && LevelRogueUI.Visible)
			{
				if (!uiInitialized && Main.LocalPlayer != null && Main.LocalPlayer.active)
				{
					levelRogueUI.Activate();
					levelInterface.SetState(levelRogueUI);
					uiInitialized = true;
				}

				levelInterface?.Update(gameTime);
			}
		}

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1 && LevelRogueUI.Visible)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LevelRogue: Level UI",
                    () =>
                    {
                        levelInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
