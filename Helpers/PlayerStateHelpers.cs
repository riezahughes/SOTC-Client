using Archipelago.Core;
using Archipelago.Core.Util;
using SotcArchipelago;
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

        public static bool UpdatePlayerStamina(ArchipelagoClient client)
        {

            int staminaMinorCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Sliver of Courage Stamina"));
            int staminaMajorCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Progressive Stamina Capacity")) * 8;

            float playerStamina = 100.0f + staminaMinorCount + staminaMajorCount;

            byte[] bytes = BitConverter.GetBytes(playerStamina);

            Memory.WriteByteArray(Addresses.MaxStamina, bytes);

            return true;

        }

        public static bool UpdatePlayerHealth(ArchipelagoClient client)
        {
            int healthMinorCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Sliver of Hope HP"));
            int healthMajorCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Progressive Health Capacity")) * 8;

            float playerHealth = 100.0f + healthMinorCount + healthMajorCount;

            byte[] bytes = BitConverter.GetBytes(playerHealth);

            Memory.WriteByteArray(Addresses.MaxHealth, bytes);

            return true;
        }

        public static void KillPlayer()
        {
            //TODO: Kill the player logic goes here
            Console.WriteLine("Ur ded kiddo");
            Memory.WriteByte(Addresses.KillPlayer, 0xfF);
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
