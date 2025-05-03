using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace LevelRogue
{
    public class LevelNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            // Проверим, что кто-то убил NPC
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                // Ищем игрока, который нанес последний удар (проверка на взаимодействие с NPC)
                if (npc.playerInteraction[i]) 
                {
                    Player player = Main.player[i];
                    LevelPlayer modPlayer = player.GetModPlayer<LevelPlayer>();

                    int baseExp = CalculateExperience(npc);
                    modPlayer.AddExperience(baseExp);

                    // Показываем количество опыта, которое было получено
                    CombatText.NewText(player.getRect(), Color.Green, $"{baseExp} XP", true);

                    TryIncreaseMaxLevel(npc, modPlayer);
                    break; // Если нашли игрока, выходим из цикла
                }
            }
        }

        private int CalculateExperience(NPC npc)
        {
            int hp = npc.lifeMax;
            int damage = npc.damage;
            int defense = npc.defense;

            return (int)(hp * 0.5 + damage * 1.5 + defense * 1.2);
        }

        private void TryIncreaseMaxLevel(NPC npc, LevelPlayer modPlayer)
        {
            if (npc.type == NPCID.EyeofCthulhu && modPlayer.maxLevel < 15)
                modPlayer.maxLevel = 15;
            else if (npc.type == NPCID.EaterofWorldsHead && modPlayer.maxLevel < 20)
                modPlayer.maxLevel = 20;
            else if (npc.type == NPCID.SkeletronHead && modPlayer.maxLevel < 25)
                modPlayer.maxLevel = 25;
            else if (npc.type == NPCID.WallofFlesh && modPlayer.maxLevel < 35)
                modPlayer.maxLevel = 35;
            else if ((npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime) && modPlayer.maxLevel < 45)
                modPlayer.maxLevel = 45;
            else if (npc.type == NPCID.Plantera && modPlayer.maxLevel < 55)
                modPlayer.maxLevel = 55;
            else if (npc.type == NPCID.Golem && modPlayer.maxLevel < 65)
                modPlayer.maxLevel = 65;
            else if ((npc.type == NPCID.LunarTowerSolar || npc.type == NPCID.LunarTowerNebula || 
                    npc.type == NPCID.LunarTowerVortex || npc.type == NPCID.LunarTowerStardust) && modPlayer.maxLevel < 70)
                modPlayer.maxLevel = 70;
            else if (npc.type == NPCID.MoonLordCore && modPlayer.maxLevel < 75)
                modPlayer.maxLevel = 75;
        }

        public override bool InstancePerEntity => true;
    }
}
