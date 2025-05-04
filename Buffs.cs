using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using LevelRogue.UI;
using Microsoft.Xna.Framework.Graphics;

namespace LevelRogue.Buffs
{
    // Бафф "Стойкость Воина" (Воин-Ученик)
    public class WarriorStudentBuff : ModBuff
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Стойкость Воина");
        public override LocalizedText Description => Language.GetOrRegister("+5% сопротивления урону и +1.5 к регенерации здоровья.");

        public static Texture2D BuffIcon;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;

            // Загрузка иконки для баффа
            BuffIcon = ModContent.Request<Texture2D>("LevelRogue/Buffs/WarriorStudentBuff").Value; // Путь к иконке
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.05f; // +5% к сопротивлению урону
            player.lifeRegen += 15;    // +1.5 к регенерации здоровья
        }
    }

    // Бафф "Аура Стойкости" (Воин-Адепт)
    public class WarriorAdeptBuff : ModBuff
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Аура Стойкости");
        public override LocalizedText Description => Language.GetOrRegister("+5% сопротивления урону союзникам рядом, +7.5% себе и +2.25 к регенерации.");

        public static Texture2D BuffIcon;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;

            // Загрузка иконки для баффа
            BuffIcon = ModContent.Request<Texture2D>("LevelRogue/Buffs/WarriorAdeptBuff").Value; // Путь к иконке
        }

        public override void Update(Player player, ref int buffIndex)
		{
			// Эффекты для себя
			player.endurance += 0.075f; // Себе 7.5% сопротивления
			player.lifeRegen += 22;     // Себе +2.25 регенерации

			// Эффекты для союзников
			foreach (Player otherPlayer in Main.player)
			{
				if (otherPlayer.active && !otherPlayer.dead && otherPlayer.whoAmI != player.whoAmI)
				{
					// Проверяем расстояние до другого игрока (100 тайлов = 1600 пикселей)
					float distance = Vector2.Distance(player.Center, otherPlayer.Center);
					if (distance <= 1600f)
					{
						otherPlayer.endurance += 0.05f; // +5% сопротивления урону
						otherPlayer.lifeRegen += 15;   // +1.5 к регенерации здоровья
					}
				}
			}
		}
    }

    // Здесь потом допишем остальные баффы: Берсерка, Паладина и т.д.
}
