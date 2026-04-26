using Archipelago.Core;
using Archipelago.Core.Util;
using ClientTemplate;
using SotcArchipelago.Helpers;

namespace Helpers
{
    public class PlayerStateHelpers
    {
        public static T GetPlayerOption<T>(Dictionary<string, object> options, string optionKey, T defaultValue = default) where T : struct, Enum
        {
            string optionValue = options?.GetValueOrDefault(optionKey, "0").ToString();

            if (Enum.TryParse<T>(optionValue, out T result))
            {
                return result;
            }

            return defaultValue;
        }

        public static int GetPlayerOptionCounts(Dictionary<string, object> options, string optionKey, int defaultValue = 0)
        {
            string optionValue = options?.GetValueOrDefault(optionKey, "0")?.ToString() ?? "0";

            if (Int32.TryParse(optionValue, out int result))
            {
                return result;
            }

            return defaultValue;
        }
        public static void KillPlayer()
        {
            //TODO: Kill the player logic goes here
            Console.WriteLine("Ur ded kiddo");
        }

        public static void UpdatePlayerState(ArchipelagoClient client)
        {
            //TODO: Player update logic
            ItemHelpers.ProcessPendingItems(client);
        }

        public static void OnSaveMenuDetected(ArchipelagoClient client)
        {
            // Write the current ProcessedItemIndex to a specific memory address
            Memory.Write(Addresses.ItemIndexStorage, (ushort)App.ProcessedItemIndex);
#if DEBUG
            Console.WriteLine($"Saved item index: {App.ProcessedItemIndex}");
#endif
        }

        public static void OnGameLoaded(ArchipelagoClient client)
        {
            // Read the saved index from memory
            var index = Memory.ReadUShort(Addresses.ItemIndexStorage);
            App.ProcessedItemIndex = index;
#if DEBUG
            Console.WriteLine($"Loaded item index: {App.ProcessedItemIndex}");
#endif
            // Immediately try to process any pending items
            UpdatePlayerState(client);

        }
    }

}
