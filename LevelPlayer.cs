using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Microsoft.Xna.Framework;
using LevelRogue.UI;
using LevelRogue.Buffs;

namespace LevelRogue
{
    public class LevelPlayer : ModPlayer
	{
		private int savedItemAnimation;
		private int savedItemTime;

        // 🔥 Статистика игрока
        public int level = 1;
        public int experience = 0;
        public int statPoints = 0;
        public int bonusStatPoints = 0; // Дополнительные очки

        // 🔥 Бонусы
        public int MeleeDamageBonus = 0;
		public int bonusMeleeCrit = 0;
		public float bonusMeleeSpeed = 0f;
		public float bonusEndurance = 0f;
		
		// Переменные для Стрелка
		public int spentRangedDamage;
		public int spentRangedCrit;
		public int spentRangedSpeed;

		// Бонусы для Стрелка
		public int rangedDamageBonus = 0; // Урон в процентах
		public int bonusRangedCrit = 0;   // Шанс крита
		public float bonusRangedSpeed = 0f; // Скорость стрельбы
		
        public int magicDamageBonus = 0;
		public int bonusMagicCrit = 0;
		
        public int summonDamageBonus = 0;
		public float bonusSummonKnockback = 0f;
		
        public int bonusHP = 0;
		
        public int regenBonus = 0;
		
        public float blockChanceBonus = 0f; // Шанс блокировки урона
		
		public float bonusLuck = 0f;
		
		public int bonusAggro = 0;

        // 🔥 Потраченные очки на характеристики
		public int spentMelee;
		public int spentMeleeCrit;
		public int spentMeleeSpeed;

		public int spentRanged;
		public int spentMagic;
		public int spentSummon;
		public int spentRogue; // если у тебя есть класс Разбойника
		public int spentPlayer; // для общей прокачки типа HP, Regen, Crit и т.д.
		
		public enum WarriorRank
		{
			None,
			Student,
			Adept,
			Berserker,
			Paladin,
			BloodBerserker,
			HighPaladin,
			LegendaryBerserker,
			ArchangelPaladin
		}
		
		public override void LoadData(TagCompound tag)
		{
			level = tag.GetInt("level");
			experience = tag.GetInt("experience");
			statPoints = tag.GetInt("statPoints");
			bonusStatPoints = tag.GetInt("bonusStatPoints");

			MeleeDamageBonus = tag.GetInt("MeleeDamageBonus");
			bonusMeleeCrit = tag.GetInt("bonusMeleeCrit");
			bonusMeleeSpeed = tag.GetFloat("bonusMeleeSpeed");
			bonusEndurance = tag.GetFloat("bonusEndurance");

			rangedDamageBonus = tag.GetInt("rangedDamageBonus");
			bonusRangedCrit = tag.GetInt("bonusRangedCrit");

			magicDamageBonus = tag.GetInt("magicDamageBonus");
			bonusMagicCrit = tag.GetInt("bonusMagicCrit");

			summonDamageBonus = tag.GetInt("summonDamageBonus");
			bonusSummonKnockback = tag.GetFloat("bonusSummonKnockback");

			bonusHP = tag.GetInt("bonusHP");
			regenBonus = tag.GetInt("regenBonus");

			blockChanceBonus = tag.GetFloat("blockChanceBonus");

			bonusLuck = tag.GetFloat("bonusLuck");
			bonusAggro = tag.GetInt("bonusAggro");

		}
		
		public bool isPaladinPath = false;

		public WarriorRank warriorRank = WarriorRank.None;


		
		

        // 🔥 Максимальный уровень
        public int maxLevel = 5; // Начальный максимальный уровень

        // 🔥 Получение опыта для следующего уровня
        public int ExpToNextLevel => GetExperienceForNextLevel();

        public override void Initialize()
        {
            level = 1;
            experience = 0;
            statPoints = 0;
            bonusStatPoints = 0;
            maxLevel = 5;
        }

        public override void ResetEffects()
		{
			ApplyStatBonuses();
			UpdateWarriorRank();
			ApplyWarriorRankEffects();

			// Временные переменные
			float meleeDamageBonus = 0f;
			int meleeCritBonus = 0;
			float meleeSpeedBonus = 0f;

			// Применяем прокачанные бонусы
			meleeDamageBonus += spentMelee * 0.01f;
			meleeCritBonus += spentMeleeCrit;
			meleeSpeedBonus += spentMeleeSpeed * 0.01f;

			// Применяем к игроку
			Player.GetDamage(DamageClass.Melee) += meleeDamageBonus;
			Player.GetCritChance(DamageClass.Melee) += meleeCritBonus;
			Player.GetAttackSpeed(DamageClass.Melee) += meleeSpeedBonus;
			
			// Применение бонусов для Стрелка
			float rangedDamageBonus = spentRangedDamage * 0.01f;
			int rangedCritBonus = spentRangedCrit;
			float rangedSpeedBonus = spentRangedSpeed * 0.01f;

			Player.GetDamage(DamageClass.Ranged) += rangedDamageBonus;
			Player.GetCritChance(DamageClass.Ranged) += rangedCritBonus;
			Player.GetAttackSpeed(DamageClass.Ranged) += rangedSpeedBonus;
		}


        private void ApplyStatBonuses()
        {
            Player.GetDamage(DamageClass.Melee) += MeleeDamageBonus / 100f;
            Player.GetDamage(DamageClass.Ranged) += rangedDamageBonus / 100f;
            Player.GetDamage(DamageClass.Magic) += magicDamageBonus / 100f;
            Player.GetDamage(DamageClass.Summon) += summonDamageBonus / 100f;

            Player.GetCritChance(DamageClass.Melee) += bonusMeleeCrit;
            Player.GetCritChance(DamageClass.Ranged) += bonusMeleeCrit;
            Player.GetCritChance(DamageClass.Magic) += bonusMeleeCrit;
			
			Player.GetAttackSpeed(DamageClass.Melee) += bonusMeleeSpeed;
        }

        public override void UpdateDead()
        {
            // Потеря опыта при смерти, если нужно
        }

        public override void UpdateLifeRegen()
        {
            Player.lifeRegen += regenBonus;
        }

        public override void PostUpdate()
		{

		}
		
		public void UpdateWarriorRank()
		{
			int totalWarriorPoints = spentMelee; // Пока считаем только очки вложенные в Воина

			if (totalWarriorPoints >= 150)
			{
				warriorRank = isPaladinPath ? WarriorRank.ArchangelPaladin : WarriorRank.LegendaryBerserker;
			}
			else if (totalWarriorPoints >= 100)
			{
				warriorRank = isPaladinPath ? WarriorRank.HighPaladin : WarriorRank.BloodBerserker;
			}
			else if (totalWarriorPoints >= 70)
			{
				warriorRank = isPaladinPath ? WarriorRank.Paladin : WarriorRank.Berserker;
			}
			else if (totalWarriorPoints >= 50)
			{
				warriorRank = WarriorRank.Adept;
			}
			else if (totalWarriorPoints >= 30)
			{
				warriorRank = WarriorRank.Student;
			}
			else
			{
				warriorRank = WarriorRank.None;
			}
		}
		
		public void SpendWarriorSkillPoint()
		{
			if (statPoints > 0)
			{
				statPoints--;

				WarriorRank oldRank = warriorRank; // запоминаем старый ранг
				UpdateWarriorRank(); // обновляем ранг

				if (warriorRank != oldRank) // если ранг изменился
				{
					Main.NewText($"Вы достигли ранга: {warriorRank}!", Color.Gold);
				}
			}
		}


		public void AddExperience(int amount)
		{
			if (level >= maxLevel)
			{
				// Останавливаем опыт при максимальном уровне
				return;
			}
			
			experience += amount;

			while (experience >= ExpToNextLevel && level < maxLevel)
			{
				experience -= ExpToNextLevel;
				level++;
				statPoints += 2;
				CombatText.NewText(Player.getRect(), Color.Green, $"Уровень {level}!");
			}
		}

        private int GetExperienceForNextLevel()
        {
            return 500 + 50 * level * level - 50; // Формула для опыта
        }

        public void ResetStats()
		{
			// Сбрасываем затраченные очки
			spentMelee = 0;
			spentMeleeCrit = 0;
			spentMeleeSpeed = 0;
			spentRanged = 0;
			spentMagic = 0;
			spentSummon = 0;
			spentRogue = 0;
			spentPlayer = 0;

			// Сбрасываем бонусы
			MeleeDamageBonus = 0;
			bonusMeleeCrit = 0;
			bonusMeleeSpeed = 0f;
			bonusEndurance = 0f;
			magicDamageBonus = 0;
			bonusMagicCrit = 0;
			summonDamageBonus = 0;
			bonusSummonKnockback = 0f;
			bonusHP = 0;
			regenBonus = 0;
			bonusLuck = 0f;
			bonusAggro = 0;
			
			// Сбрасываем бонусы для Стрелка
			rangedDamageBonus = 0;
			bonusRangedCrit = 0;
			bonusRangedSpeed = 0f;

			// Сбрасываем затраченные очки
			spentRangedDamage = 0;
			spentRangedCrit = 0;
			spentRangedSpeed = 0;

			// Пересчитываем очки навыков по уровню
			statPoints = level * 2 + bonusStatPoints;
		}


        public void ResetStat(string statName)
        {
            switch (statName)
            {
                case "Melee":
                    MeleeDamageBonus = 0;
                    break;
                case "Ranged":
                    rangedDamageBonus = 0;;
                    break;
                case "Magic":
                    magicDamageBonus = 0;
                    break;
                case "Summon":
                    summonDamageBonus = 0;
                    break;
                case "HP":
                    bonusHP = 0;
                    break;
                case "Regen":
                    regenBonus = 0;
                    break;
                case "Crit":
                    bonusMeleeCrit = 0;
                    break;
            }

            statPoints = level * 2 + bonusStatPoints;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;

            health.Base += bonusHP; // Увеличение максимального здоровья
        }

		public override void SaveData(TagCompound tag)
			{
				tag["level"] = level;
				tag["experience"] = experience;
				tag["statPoints"] = statPoints;
				tag["bonusStatPoints"] = bonusStatPoints;

				tag["MeleeDamageBonus"] = MeleeDamageBonus;
				tag["bonusMeleeCrit"] = bonusMeleeCrit;
				tag["bonusMeleeSpeed"] = bonusMeleeSpeed;
				tag["bonusEndurance"] = bonusEndurance;

				tag["rangedDamageBonus"] = rangedDamageBonus;
				tag["bonusRangedCrit"] = bonusRangedCrit;

				tag["magicDamageBonus"] = magicDamageBonus;
				tag["bonusMagicCrit"] = bonusMagicCrit;

				tag["summonDamageBonus"] = summonDamageBonus;
				tag["bonusSummonKnockback"] = bonusSummonKnockback;

				tag["bonusHP"] = bonusHP;
				tag["regenBonus"] = regenBonus;

				tag["blockChanceBonus"] = blockChanceBonus;

				tag["bonusLuck"] = bonusLuck;
				tag["bonusAggro"] = bonusAggro;

				tag["spentMelee"] = spentMelee;
				tag["spentRanged"] = spentRanged;
				tag["spentMagic"] = spentMagic;
				tag["spentSummon"] = spentSummon;
				tag["spentRogue"] = spentRogue;
				tag["spentPlayer"] = spentPlayer;
			}
		
		private void ApplyWarriorRankEffects()
		{
			switch (warriorRank)
			{
				case WarriorRank.Student:
					Player.endurance += 0.05f;  // +5% сопротивления урону
					Player.lifeRegen += 15;     // +1.5 регенерации
					break;
				case WarriorRank.Adept:
					Player.endurance += 0.10f;  // пример эффекта для Adept
					Player.lifeRegen += 30;
					break;
				// Добавь остальные ранги по аналогии
			}
		}
    }
}
