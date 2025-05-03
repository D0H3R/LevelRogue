using Terraria.ModLoader;
using Terraria;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using LevelRogue.UI; // ✅ добавляем это

namespace LevelRogue
{
    public class Keybinds : ModSystem
    {
        public static ModKeybind OpenLevelMenuKeybind;

        public override void Load()
        {
            OpenLevelMenuKeybind = KeybindLoader.RegisterKeybind(Mod, "Открыть окно уровней", "L");
        }

        public override void Unload()
        {
            OpenLevelMenuKeybind = null;
        }

        public override void PostUpdateInput()
        {
            if (OpenLevelMenuKeybind.JustPressed)
            {
                // Заменили LevelStatsUI на LevelRogueUI
                LevelRogueUI.Visible = !LevelRogueUI.Visible;  // Теперь управляем видимостью через LevelRogueUI
            }
        }
    }
}
