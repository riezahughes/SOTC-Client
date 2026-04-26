using Archipelago.Core;
using Archipelago.Core.Util;
using Archipelago.MultiClient.Net.Models;
using ClientTemplate;
using Helpers;
using Kokuban;

namespace SotcArchipelago.Helpers
{
    public class ItemHelpers
    {
        private static bool TryGiveItem(ItemInfo item, ArchipelagoClient client)
        {
            //// Try to handle the item based on type
            //if (result.Contains("Teleport"))
            //    return handleTeleportItem();
            //else if (result.Contains("Rood Inverse"))
            //    return handleRoodInverseItem();
            //else if (result.Contains("Blood Sin"))
            //    return handleBloodSinProgession();
            //else if (item.ItemName.Contains("Progressive"))
            //    return ItemHelpers.handleGrimoireUnlock(item, client.CurrentSession.Items.AllItemsReceived);
            //else if (ItemHelpers.ItemReference.Any(itm => itm.Value == item.ItemName && itm.Value.Contains("Grimoire")))
            //    return ItemHelpers.handleGrimoireUnlock(item, client.CurrentSession.Items.AllItemsReceived);
            //else if (ItemHelpers.ItemReference.Any(itm => itm.Value == item.ItemName))
            //    return ItemHelpers.handleInventoryItem(item);
            //else if (ItemHelpers.GemReference.Any(itm => itm.Value == item.ItemName))
            //    return ItemHelpers.handleInventoryGem(item);
            //else if (ItemHelpers.ArmorReference.Any(itm => itm.Value == result))
            //    return ItemHelpers.handleInventoryArmor(item);
            //else if (ItemHelpers.ShieldReference.Any(itm => itm.Value == result))
            //    return ItemHelpers.handleInventoryShield(item);
            //else if (ItemHelpers.CraftingBladeReference.Any(itm => itm.Value == result))
            //    return ItemHelpers.handleInventoryCraftingBlade(item);
            //else if (ItemHelpers.CraftingGripReference.Any(itm => itm.Value == item.ItemName))
            //    return ItemHelpers.handleInventoryCraftingGrip(item);
            //else if (ItemHelpers.ChainAbilityUnlockReference.Any(itm => itm.Key == item.ItemName))
            //    return ItemHelpers.handleChainAbility(item.ItemName);
            //else if (ItemHelpers.DefenceAbilityUnlockReference.Any(itm => itm.Key == item.ItemName))
            //    return ItemHelpers.handleDefenceAbility(item.ItemName);
            //else if (ItemHelpers.BreakArtsFlattenedDictionary.Any(itm => itm.Key == item.ItemName))
            //    return ItemHelpers.handleBreakArt(item.ItemName);
            //else
            //{
            //    Console.WriteLine($"Item not recognised. ({item.ItemName}) Skipping");
            //    return true; // Skip unrecognized items
            //}
            return true;
        }

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
                    Console.WriteLine(bg + (fg + $"⚠️ Cannot process item {itemToProcess.ItemName} - inventory full. Will retry later."));
                    break; // Exit the loop, we'll try again later
                }
            }

            APHelpers.PROCESSING_ITEM_LIST = false;
        }
    }
}
