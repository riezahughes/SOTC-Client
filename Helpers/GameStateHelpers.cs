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