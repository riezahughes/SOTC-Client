using Archipelago.Core;
using Archipelago.Core.Util;
using SotcArchipelago;

namespace Helpers
{
    public class GameStateHelpers
    {
        internal static bool CheckFirstWinCondition(ArchipelagoClient client)
        {
            return true;
        }

        internal static bool CheckSecondWinCondition(ArchipelagoClient client)
        {
            return true;
        }

        public static bool CheckGoalCondition(ArchipelagoClient client)
        {
            // TODO Victory logic goes into each of these goal conditions

            int goalCondition = Int32.Parse(client.Options?.GetValueOrDefault("goal", "0").ToString());

            if (goalCondition == PlayerVictoryConditions.VICTORY_1)
            {
                Console.WriteLine("Cleared 1");
                return true;
            }
            else if (goalCondition == PlayerVictoryConditions.VICTORY_2)
            {
                Console.WriteLine("Cleared 2");
                return true;
            }
            else if (goalCondition == PlayerVictoryConditions.VICTORY_3)
            {
                Console.WriteLine("Cleared 1 and 2");
                return true;
            }
            return false;
        }

        public static uint SetCurrentSigilState(ArchipelagoClient client)
        {
            // this is clearly firing too much. need to solve this issue so that
            // i can successfully hide a colossus corpse. Maybe set it to specific grid points instead of 
            // constantly changing this like eveyrthing else.
            var sigilItems = client.CurrentSession.Items.AllItemsReceived
                .Where(item => item.ItemName.Contains("sigil", StringComparison.OrdinalIgnoreCase));

            Memory.WriteBit(Addresses.ColossusGraves2, 2, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 1, false);
            Memory.WriteBit(Addresses.ColossusGraves1, 5, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 5, false);
            Memory.WriteBit(Addresses.ColossusGraves1, 6, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 3, false);
            Memory.WriteBit(Addresses.ColossusGraves3, 4, false);
            Memory.WriteBit(Addresses.ColossusGraves3, 3, false);
            Memory.WriteBit(Addresses.ColossusGraves1, 7, false);
            Memory.WriteBit(Addresses.ColossusGraves3, 0, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 6, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 7, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 0, false);
            Memory.WriteBit(Addresses.ColossusGraves3, 2, false);
            Memory.WriteBit(Addresses.ColossusGraves2, 4, false);

            foreach (var sigilItem in sigilItems)
            {
                switch (sigilItem.ItemName)
                {
                    case "Sigil of the First Awakening":
                        Memory.WriteBit(Addresses.ColossusGraves2, 2, true);
                        break;
                    case "Sigil of Burdened Earth":
                        Memory.WriteBit(Addresses.ColossusGraves2, 1, true);
                        break;
                    case "Sigil of the Fallen Oath":
                        Memory.WriteBit(Addresses.ColossusGraves1, 5, true);
                        break;
                    case "Sigil of Veiled Fear":
                        Memory.WriteBit(Addresses.ColossusGraves2, 5, true);
                        break;
                    case "Sigil of the Skybound Silence":
                        Memory.WriteBit(Addresses.ColossusGraves1, 6, true);
                        break;
                    case "Sigil of the Hollow Shrine":
                        Memory.WriteBit(Addresses.ColossusGraves2, 3, true);
                        break;
                    case "Sigil of the Sunken Pulse":
                        Memory.WriteBit(Addresses.ColossusGraves3, 4, true);
                        break;
                    case "Sigil of the Watching Walls":
                        Memory.WriteBit(Addresses.ColossusGraves3, 3, true);
                        break;
                    case "Sigil of the Sealed Core":
                        Memory.WriteBit(Addresses.ColossusGraves1, 7, true);
                        break;
                    case "Sigil of the Devouring Wind":
                        Memory.WriteBit(Addresses.ColossusGraves3, 0, true);
                        break;
                    case "Sigil of the Broken Courage":
                        Memory.WriteBit(Addresses.ColossusGraves2, 6, true);
                        break;
                    case "Sigil of the Drowned Throne":
                        Memory.WriteBit(Addresses.ColossusGraves2, 7, true);
                        break;
                    case "Sigil of Endless Horizon":
                        Memory.WriteBit(Addresses.ColossusGraves2, 0, true);
                        break;
                    case "Sigil of Ruined Pride":
                        Memory.WriteBit(Addresses.ColossusGraves3, 2, true);
                        break;
                    case "Sigil of the Bound Colossus":
                        Memory.WriteBit(Addresses.ColossusGraves2, 4, true);
                        break;
                    default:
                        break;

                }
            }

            return 0x00;
        }
        public static void SetColossiKilled(CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.NumberOfColossiKilled,
                () =>
                {
                    Memory.Write(Addresses.NumberOfColossiKilled, 0x0e);
                    SetColossiKilled(cts);
                },
            value => value != 0x0e);
        }

        public static void CheckStatues(ArchipelagoClient client, CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.ColossusVisibility1,
                () =>
                {
                    // Keep Colossi Dead
                    Memory.Write(Addresses.ColossusVisibility1, 0x00);
                    Memory.Write(Addresses.ColossusVisibility2, 0x80);
                    SetCurrentSigilState(client);

                    CheckStatues(client, cts);
                },
            value => value != 0x20);
        }

        public static void SetNewGamePlus(CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.NewGamePlusFlag,
                () =>
                {
                    Memory.Write(Addresses.NewGamePlusFlag, 0x01);
                    SetNewGamePlus(cts);
                },
            value => value == 0);
        }

        public static void SetGameBeaten(CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.GameClearedCount,
                () =>
                {
                    Memory.Write(Addresses.GameClearedCount, 0x01);
                    SetGameBeaten(cts);
                },
            value => value == 0);
        }

    }
}