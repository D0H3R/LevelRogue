using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Creative;
using LevelRogue.UI;

namespace LevelRogue.Items
{
    public class ResetBook : ModItem
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Книга Забвения");
        public override LocalizedText Tooltip => Language.GetOrRegister("Возвращает уровень и умения вашего персонажа в изначальное состояние.");

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item4;
            Item.rare = ItemRarityID.Red;
        }

        public override bool CanUseItem(Player player)
        {
            LevelPlayer modPlayer = player.GetModPlayer<LevelPlayer>();
            return modPlayer.level > 1 || modPlayer.experience > 0 || modPlayer.statPoints > 0 ||
                   modPlayer.spentMelee + modPlayer.spentRanged + modPlayer.spentMagic +
                   modPlayer.spentSummon + modPlayer.spentRogue + modPlayer.spentPlayer > 0;
        }

        public override bool? UseItem(Player player)
        {
            LevelPlayer modPlayer = player.GetModPlayer<LevelPlayer>();

            modPlayer.level = 1;
            modPlayer.experience = 0;
            modPlayer.statPoints = 0;
            modPlayer.bonusStatPoints = 0;

            // Сбрасываем все бонусы характеристик
            modPlayer.MeleeDamageBonus = 0;
            modPlayer.bonusMeleeCrit = 0;
            modPlayer.bonusMeleeSpeed = 0f;
            modPlayer.bonusEndurance = 0f;

            modPlayer.rangedDamageBonus = 0;
            modPlayer.bonusRangedCrit = 0;

            modPlayer.magicDamageBonus = 0f; // Урон магии
            modPlayer.bonusMagicCrit = 0;
            modPlayer.summonDamageBonus = 0f; // Урон призывателя

            modPlayer.bonusHP = 0;
            modPlayer.regenBonus = 0;
            modPlayer.bonusLuck = 0f;
            modPlayer.bonusAggro = 0;

            // Сбрасываем затраченные очки
            modPlayer.spentMelee = 0;
            modPlayer.spentRanged = 0;
            modPlayer.spentMagic = 0;
            modPlayer.spentSummon = 0;
            modPlayer.spentRogue = 0;
            modPlayer.spentPlayer = 0;

            modPlayer.UpdateWarriorRank();

            Main.NewText("Ваш уровень и характеристики были полностью обнулены!", 255, 60, 60);

            // Обновляем интерфейс
            LevelRogueUI.RefreshWarriorStatDisplay();

            return true;
        }
    }
}
