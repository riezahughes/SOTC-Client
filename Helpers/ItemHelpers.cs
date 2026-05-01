using Archipelago.Core;
using Archipelago.Core.Util;
using Archipelago.MultiClient.Net.Models;
using Helpers;
using Kokuban;

namespace SotcArchipelago.Helpers
{
    public class ItemHelpers
    {
        private static bool TryGiveItem(ItemInfo item, ArchipelagoClient client)
        {
            //// Try to handle the item based on type
            if (item.ItemName.Contains("Sliver of Hope HP"))
                return PlayerStateHelpers.UpdatePlayerHealth(client);
            else if (item.ItemName.Contains("Progressive Health Capacity"))
                return PlayerStateHelpers.UpdatePlayerHealth(client);
            else if (item.ItemName.Contains("Sliver of Courage Stamina"))
                return PlayerStateHelpers.UpdatePlayerStamina(client);
            else if (item.ItemName.Contains("Progressive Stamina Capacity"))
                return PlayerStateHelpers.UpdatePlayerStamina(client);
            else if (ItemBitflags.Keys.FirstOrDefault(itm => item.ItemName.Contains(itm)) is string match)
            {
                GiveItem(match);
                return true;
            }
            else if (item.ItemName.Contains("Sigil"))
            {
                SigilNameToColossus.TryGetValue(item.ItemName, out int colossusIndex);
                Console.WriteLine($"You can now fight Colossus {colossusIndex}!");
                return true;
            }

            else if (item.ItemName.Contains("Shard"))
            {
                Console.WriteLine("Recieved Shard!");
                return true;
            }
            else
            {
                Console.WriteLine($"Item not recognised. ({item.ItemName}) Skipping");
                return true; // Skip unrecognized items
            }
        }


        public struct ItemLocation
        {
            public uint Address;
            public int Bit;

            public ItemLocation(uint address, int bit)
            {
                Address = address;
                Bit = bit;
            }
        }

        public static bool GiveItem(string itemName)
        {
            ItemLocation itemData = ItemBitflags.GetValueOrDefault(itemName);
            Memory.WriteBit(itemData.Address, itemData.Bit, true);
            return true;
        }

        public static readonly Dictionary<string, int> SigilNameToColossus = new Dictionary<string, int>
        {
            { "Sigil of the First Awakening", 1 },
            { "Sigil of Burdened Earth", 2 },
            { "Sigil of the Fallen Oath", 3 },
            { "Sigil of Veiled Fear", 4 },
            { "Sigil of the Skybound Silence", 5 },
            { "Sigil of the Hollow Shrine", 6 },
            { "Sigil of the Sunken Pulse", 7 },
            { "Sigil of the Watching Walls", 8 },
            { "Sigil of the Sealed Core", 9 },
            { "Sigil of the Devouring Wind", 10 },
            { "Sigil of the Broken Courage", 11 },
            { "Sigil of the Drowned Throne", 12 },
            { "Sigil of Endless Horizon", 13 },
            { "Sigil of Ruined Pride", 14 },
            { "Sigil of the Bound Colossus", 15 }
        };

        public static Dictionary<string, ItemLocation> ItemBitflags = new Dictionary<string, ItemLocation>
            {
                // Address 0x12DA3DA
                { "Shaman's Mask",        new ItemLocation(Addresses.ItemArray1, 1) },
                { "Mask of Strength",     new ItemLocation(Addresses.ItemArray1, 2) },
                { "Mask of Power",        new ItemLocation(Addresses.ItemArray1, 3) },
                { "Shaman's Cloak",       new ItemLocation(Addresses.ItemArray1, 5) },
                { "Cloak of Force",       new ItemLocation(Addresses.ItemArray1, 6) },
                { "Cloak of Deception",   new ItemLocation(Addresses.ItemArray1, 7) },

                // Address 0x12DA3DC
                { "Flash Arrow",          new ItemLocation(Addresses.ItemArray2, 2) },
                { "Whistling Arrow",      new ItemLocation(Addresses.ItemArray2, 3) },
                { "Harpoon of Thunder",   new ItemLocation(Addresses.ItemArray2, 4) },
                { "Sword of the Sun",     new ItemLocation(Addresses.ItemArray2, 6) },
                { "Queen's Sword",        new ItemLocation(Addresses.ItemArray2, 7) },

                // Address 0x12DA3DD
                { "Cloth of Desperation", new ItemLocation(Addresses.ItemArray3, 0) },
                { "Eye of the Colossus",  new ItemLocation(Addresses.ItemArray3, 1) },
                { "Fruit Tree Map",       new ItemLocation(Addresses.ItemArray3, 2) },
                { "Lizard Detection Stone", new ItemLocation(Addresses.ItemArray3, 3) }
            };

        public static void ProcessPendingItems(ArchipelagoClient client)
        {
            if (client.CurrentSession == null)
            {
                return;
            }

            var allItems = client.CurrentSession.Items.AllItemsReceived;

            //int bloodSinCount = 0;
            //for (int i = 0; i < App.ProcessedItemIndex && i < allItems.Count; i++)
            //{
            //    if (allItems[i].ItemName.Contains("Blood Sin"))
            //        bloodSinCount++;
            //}

            // Process items from our last saved index up to what we've received
            while (App.ProcessedItemIndex < allItems.Count)
            {
                var itemToProcess = allItems[App.ProcessedItemIndex];

                // Try to give the item
                bool success = TryGiveItem(itemToProcess, client);

                if (success)
                {
                    // Only increment if we successfully gave the item
                    App.ProcessedItemIndex++;
                    Memory.Write(Addresses.ItemIndexStorage, (ushort)App.ProcessedItemIndex);
#if DEBUG
                    Console.WriteLine($"Successfully processed item {App.ProcessedItemIndex}/{allItems.Count}");
#endif
                }
                else
                {
                    // Inventory full - stop processing and wait
                    Kokuban.AnsiEscape.AnsiStyle bg = Chalk.BgMagenta;
                    Kokuban.AnsiEscape.AnsiStyle fg = Chalk.White;
                    Console.WriteLine(bg + (fg + $"⚠️ Cannot process item {itemToProcess.ItemName}"));
                    break; // Exit the loop, we'll try again later
                }
            }

            APHelpers.PROCESSING_ITEM_LIST = false;
        }
    }
}
