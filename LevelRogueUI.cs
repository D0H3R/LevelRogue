using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using System;
using LevelRogue;

namespace LevelRogue.UI
{
    public class LevelRogueUI : UIState
    {
		private UIText[] warriorStatTexts = new UIText[3];
		private int[] warriorStatLevels = new int[3];

        private UIPanel mainPanel;
        private UIPanel playerInfoPanel;
        private UIPanel levelInfoPanel;
        private UIElement tabButtonsPanel;
        private UIText hpText, mpText, defenseText, regenText, luckText, aggroText;
        private UIText levelText, expText, skillPointsText;

        private UIPanel[] tabPanels;
		private static UIText warriorDamageText;
		private static UIText warriorCritText;
		private static UIText warriorSpeedText;



        public static bool Visible;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Width.Set(1024, 0f);
            mainPanel.Height.Set(768, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            Append(mainPanel);

            // 🔹 Панель с уровнем, опытом и очками навыков
            levelInfoPanel = new UIPanel();
            levelInfoPanel.Width.Set(200, 0f);
            levelInfoPanel.Height.Set(100, 0f);
            levelInfoPanel.Left.Set(800, 0f);
            levelInfoPanel.Top.Set(20, 0f);
            levelInfoPanel.SetPadding(10);
            mainPanel.Append(levelInfoPanel);

            levelText = CreateInfoText("Уровень: 1", 0);
            expText = CreateInfoText("Опыт: 0 / 100", 30);
            skillPointsText = CreateInfoText("Очки навыков: 0", 60);

            levelInfoPanel.Append(levelText);
            levelInfoPanel.Append(expText);
            levelInfoPanel.Append(skillPointsText);

            // 🔹 Панель с характеристиками игрока (СДВИНУТА НИЖЕ)
            playerInfoPanel = new UIPanel();
            playerInfoPanel.Width.Set(200, 0f);
            playerInfoPanel.Height.Set(300, 0f);
            playerInfoPanel.Top.Set(130, 0f); // ← Сдвинули ниже
            playerInfoPanel.Left.Set(800, 0f);
            mainPanel.Append(playerInfoPanel);

            hpText = CreateInfoText("100 / 100", 0);
            mpText = CreateInfoText("20 / 20", 30);
            defenseText = CreateInfoText("10%", 60);
            regenText = CreateInfoText("0.5/sec", 90);
            luckText = CreateInfoText("0.1", 120);
            aggroText = CreateInfoText("500", 150);

            playerInfoPanel.Append(hpText);
            playerInfoPanel.Append(mpText);
            playerInfoPanel.Append(defenseText);
            playerInfoPanel.Append(regenText);
            playerInfoPanel.Append(luckText);
            playerInfoPanel.Append(aggroText);

            // 🔹 Панель с кнопками вкладок
            tabButtonsPanel = new UIElement();
            tabButtonsPanel.Width.Set(150, 0f);
            tabButtonsPanel.Height.Set(400, 0f);
            tabButtonsPanel.Left.Set(20, 0f);
            tabButtonsPanel.Top.Set(100, 0f);
            mainPanel.Append(tabButtonsPanel);

            string[] tabNames = { "Воин", "Стрелок", "Маг", "Призыватель", "Разбойник", "Игрок" };
            tabPanels = new UIPanel[tabNames.Length];

            for (int i = 0; i < tabNames.Length; i++)
            {
                var buttonPanel = new UIPanel();
                buttonPanel.Width.Set(120, 0f);
                buttonPanel.Height.Set(40, 0f);
                buttonPanel.Top.Set(i * 50, 0f);
                buttonPanel.HAlign = 0.5f;
                buttonPanel.BackgroundColor = new Color(73, 94, 171);

                var buttonText = new UIText(tabNames[i], 0.85f);
                buttonText.HAlign = 0.5f;
                buttonText.VAlign = 0.5f;
                buttonPanel.Append(buttonText);

                int index = i;
                buttonPanel.OnLeftClick += (evt, element) => ShowTab(index);

                tabButtonsPanel.Append(buttonPanel);

                tabPanels[i] = i == 0
                    ? CreateWarriorTab()
                    : CreateTabPanel($"Прокачка для {tabNames[i]}");

                mainPanel.Append(tabPanels[i]);
            }

            ShowTab(0);
        }

        private UIPanel CreateWarriorTab()
		{
			var panel = new UIPanel();
			panel.Width.Set(500, 0f);
			panel.Height.Set(500, 0f);
			panel.Left.Set(180, 0f);
			panel.Top.Set(50, 0f);
			panel.SetPadding(10);

			var title = new UIText("Прокачка для Воина", 0.9f);
			title.Top.Set(10, 0f);
			title.HAlign = 0.5f;
			panel.Append(title);

			

			// Добавление навыков (как у тебя уже реализовано)
			
			Player p = Main.LocalPlayer;
			LevelPlayer mp = p.GetModPlayer<LevelPlayer>();
			
			string[] statNames = { "Урон ближнего боя: ", "Шанс крита: ", "Скорость Атаки: " };

			// Создание текста и кнопок для улучшения характеристик
			for (int i = 0; i < statNames.Length; i++)
			{
				int index = i;

				// Устанавливаем начальное значение на основе текущих характеристик
				string initialValue = index switch
				{
					0 => mp.spentMelee.ToString(),     // Для урона ближнего боя
					1 => mp.spentMeleeCrit.ToString(), // Для шанса крита
					2 => mp.spentMeleeSpeed.ToString(),// Для скорости атаки
					_ => "0" // Если индекс не соответствует ни одному из вышеуказанных, то показываем 0
				};

				var statText = new UIText($"{statNames[i]} {initialValue}", 0.8f); // Используем текущие значения
				statText.Top.Set(350 + index * 40, 0f);
				statText.Left.Set(50, 0f);
				panel.Append(statText);

				// Привязка текстовых объектов к переменным для обновления их позже
				if (index == 0) warriorDamageText = statText;
				else if (index == 1) warriorCritText = statText;
				else if (index == 2) warriorSpeedText = statText;

				var addButton = new UITextPanel<string>("+");
				addButton.Width.Set(40, 0f);
				addButton.Height.Set(30, 0f);
				addButton.Top.Set(345 + index * 40, 0f);
				addButton.Left.Set(300, 0f);
				panel.Append(addButton);

				addButton.OnLeftClick += (evt, el) =>
				{
					Player p = Main.LocalPlayer;
					LevelPlayer mp = p.GetModPlayer<LevelPlayer>();

					if (mp.statPoints > 0)
					{
						switch (index)
						{
							case 0: mp.spentMelee++; break;
							case 1: mp.spentMeleeCrit++; break;
							case 2: mp.spentMeleeSpeed++; break;
						}

						mp.statPoints--;
						skillPointsText.SetText($"Очки навыков: {mp.statPoints}");
						RefreshWarriorStatDisplay(); // Обновить визуально
					}
					else
					{
						Main.NewText("Недостаточно очков навыков!");
					}
				};
			}
			return panel;
		}


        private UIPanel CreateTabPanel(string title)
        {
            var panel = new UIPanel();
            panel.Width.Set(500, 0f);
            panel.Height.Set(500, 0f);
            panel.Left.Set(180, 0f);
            panel.Top.Set(50, 0f);
            panel.SetPadding(10);

            var text = new UIText(title, 0.9f);
            text.Top.Set(10, 0f);
            text.HAlign = 0.5f;
            panel.Append(text);

            var exampleText = new UIText("Здесь будут опции прокачки.", 0.75f);
            exampleText.Top.Set(60, 0f);
            exampleText.HAlign = 0.5f;
            panel.Append(exampleText);

            return panel;
        }

        private void ShowTab(int tabIndex)
        {
            for (int i = 0; i < tabPanels.Length; i++)
            {
                if (i == tabIndex)
                {
                    if (!mainPanel.HasChild(tabPanels[i]))
                        mainPanel.Append(tabPanels[i]);
                }
                else
                {
                    if (mainPanel.HasChild(tabPanels[i]))
                        mainPanel.RemoveChild(tabPanels[i]);
                }
            }
        }

        public static void RefreshWarriorStatDisplay()
	{
	    // Логика обновления интерфейса
	}
	
 	private UIText CreateInfoText(string text, float top)
        {
            var uiText = new UIText(text);
            uiText.Top.Set(top, 0f);
            uiText.HAlign = 0.5f;
            return uiText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (!Visible)
                return;

            Player player = Main.LocalPlayer;

            Vector2 drawPosition = new Vector2(mainPanel.GetDimensions().X + 600, mainPanel.GetDimensions().Y + 100);

            Vector2 oldPos = player.position;
            Vector2 oldScreenPos = Main.screenPosition;
            int oldDirection = player.direction;

            player.direction = 1;
            player.heldProj = -1;
            player.itemAnimation = 0;
            player.itemTime = 0;

            player.position = drawPosition;
            Main.screenPosition = Vector2.Zero;

            Main.PlayerRenderer.DrawPlayer(Main.Camera, player, drawPosition, 0f, Vector2.One, 0);

            player.position = oldPos;
            Main.screenPosition = oldScreenPos;
            player.direction = oldDirection;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Update(gameTime);

            Player player = Main.LocalPlayer;
            LevelPlayer modPlayer = player.GetModPlayer<LevelPlayer>();

            hpText.SetText($"{player.statLife} / {player.statLifeMax2}");
            mpText.SetText($"{player.statMana} / {player.statManaMax2}");
            defenseText.SetText($"{Math.Round(player.endurance * 100, 1)}%");
            regenText.SetText($"{Math.Round(player.lifeRegen / 2f, 1)} / sec");
            luckText.SetText($"{Math.Round(player.luck, 2)}");
            aggroText.SetText($"{player.aggro}");

            levelText.SetText($"Уровень: {modPlayer.level}");
            expText.SetText($"Опыт: {modPlayer.experience} / {modPlayer.ExpToNextLevel}");
            skillPointsText.SetText($"Очки навыков: {modPlayer.statPoints}");
        }
    
		public void RefreshWarriorStatDisplay()
		{
			Player p = Main.LocalPlayer;
			LevelPlayer mp = p.GetModPlayer<LevelPlayer>();

			if (warriorDamageText != null)
				warriorDamageText.SetText($"Урон ближнего боя: {mp.spentMelee}");
			if (warriorCritText != null)
				warriorCritText.SetText($"Шанс крита: {mp.spentMeleeCrit}");
			if (warriorSpeedText != null)
				warriorSpeedText.SetText($"Скорость Атаки: {mp.spentMeleeSpeed}");
		}

	}	
}
