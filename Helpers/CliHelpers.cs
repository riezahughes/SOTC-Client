using System.Collections.ObjectModel;
using Archipelago.Core;
using Archipelago.MultiClient.Net.Models;
using Helpers;
using SotcArchipelago.Options;

namespace SotcArchipelago.Helpers
{
    public class CliHelpers
    {
        public static void RunOptions(ArchipelagoClient client)
        {
            Console.WriteLine($"--- RUN OPTIONS GO HERE ---");

            Console.WriteLine($"------------------");
        }



        public static void RunStatus(ArchipelagoClient client)
        {
            List<int> colossiInRun = LocationHelpers.GetCurrentColossiInRun(client);

            ReadOnlyCollection<ItemInfo> items = client.CurrentSession.Items.AllItemsReceived;

            int index = App.ProcessedItemIndex - 1;

            PlayerVictoryConditions currentGoal = PlayerStateHelpers.GetPlayerOption<PlayerVictoryConditions>(client.Options, "goal");
            int colossiQuantityOption = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "colossi_quantity");
            ColossiSpawnOptions colossiSpawnOptions = PlayerStateHelpers.GetPlayerOption<ColossiSpawnOptions>(client.Options, "colossi_spawn_choice");
            ColossiCheckOptions colossiCheckOptions = PlayerStateHelpers.GetPlayerOption<ColossiCheckOptions>(client.Options, "colossi_check_choice");
            int colossiMultiQuantity = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "colossi_check_multi_quanitity");
            int soulShardQuantity = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "soul_shard_quantity");
            int lizardQuantity = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "lizard_quantity");
            ToggleOptions fruitSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "fruitsanity");
            ToggleOptions lizardSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "lizardsanity");
            ToggleOptions shrineSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "shrinesanity");
            ToggleOptions gridSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "gridsanity");
            ToggleOptions climbSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "climbsanity");
            int climbSanityRange = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "climbsanity_range");
            int climbSanityBreakPoint = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "climbsanity_break_points");
            ToggleOptions agroSanity = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "agrosanity");
            int agroSanityRange = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "agrosanity_range");
            int agroSanityBreakPoint = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "agrosanity_break_points");

            Console.WriteLine($"--- RUN OPTIONS ---");
            Console.WriteLine($"Goal: {currentGoal}");
            Console.WriteLine($"Colossi Quantity: {colossiQuantityOption}");
            Console.WriteLine($"Colossi Spawn Choice: {colossiSpawnOptions}");
            Console.WriteLine($"Colossi Check Choice: {colossiCheckOptions}");
            Console.WriteLine($"Colossi Multi Quantity: {colossiMultiQuantity}");
            Console.WriteLine($"Soul Shard Quantity: {soulShardQuantity}");
            Console.WriteLine($"Lizard Quantity: {lizardQuantity}");
            Console.WriteLine($"Fruit Sanity: {fruitSanity}");
            Console.WriteLine($"Lizard Sanity: {lizardSanity}");
            Console.WriteLine($"Shrine Sanity: {shrineSanity}");
            Console.WriteLine($"Grid Sanity: {gridSanity}");
            Console.WriteLine($"Climb Sanity: {climbSanity}");
            Console.WriteLine($"Climb Sanity Range: {climbSanityRange}");
            Console.WriteLine($"Climb Sanity Breakpoint: {climbSanityBreakPoint}");
            Console.WriteLine($"Agro Sanity: {agroSanity}");
            Console.WriteLine($"Agro Sanity Range: {agroSanityRange}");
            Console.WriteLine($"Agro Sanity Breakpoint: {agroSanityBreakPoint}");
            Console.WriteLine($"--- COLOSSI STATUS ---");
            foreach (int colossus in colossiInRun)
            {
                var sigilName = ItemHelpers.ColossusToSigilName[colossus];
                string hasSigil = items.Any(item => item.ItemName == sigilName) ? "UNLOCKED" : "-";
                Console.WriteLine($"Colossus {colossus}: {hasSigil}");
            }
            Console.WriteLine($"------------------");
        }
        public static void DebugInformation(ArchipelagoClient client)
        {

            ReadOnlyCollection<ItemInfo> items = client.CurrentSession.Items.AllItemsReceived;
            int index = App.ProcessedItemIndex - 1;

            Console.WriteLine($"--- DEBUG STATS GO HERE ---");
            Console.WriteLine($"------------------");
        }
    }
}
