using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Creative;
using LevelRogue.UI;

namespace LevelRogue.Items
{
    public class ResetSkillBook : ModItem
    {
        public override LocalizedText DisplayName => Language.GetOrRegister("Том Перерождения");
        public override LocalizedText Tooltip => Language.GetOrRegister("Позволяет сбросить очки характеристик и перераспределить их.");

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
            return modPlayer.spentMelee + modPlayer.spentRanged + modPlayer.spentMagic + 
                   modPlayer.spentSummon + modPlayer.spentRogue + modPlayer.spentPlayer > 0;
        }

        public override bool? UseItem(Player player)
        {
            LevelPlayer modPlayer = player.GetModPlayer<LevelPlayer>();

            Main.NewText("Очки характеристик сброшены! Можно перераспределить их заново.", 255, 240, 20);

            modPlayer.ResetStats();

            // Обновление интерфейса
            LevelRogueUI.RefreshWarriorStatDisplay();

            return true;
        }
    }
}
