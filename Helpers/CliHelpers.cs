using System.Collections.ObjectModel;
using Archipelago.Core;
using Archipelago.MultiClient.Net.Models;

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

            ReadOnlyCollection<ItemInfo> items = client.CurrentSession.Items.AllItemsReceived;
            int index = App.ProcessedItemIndex - 1;

            Console.WriteLine($"--- RUN STATUS GOES HERE ---");
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
