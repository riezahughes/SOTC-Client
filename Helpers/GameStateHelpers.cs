using Archipelago.Core;
using Archipelago.Core.Util;
using SotcArchipelago;
using SotcArchipelago.Helpers;
using SotcArchipelago.Options;

namespace Helpers
{
    public class GameStateHelpers
    {
        private static byte _gridPreviousValue = 0xFF;
        internal static void CheckColossiWinCondition(ArchipelagoClient client)
        {
            var gridLetter = Memory.ReadByte(Addresses.GridMapLetter);
            var gridNumber = Memory.ReadByte(Addresses.GridMapNumber);
            var currentKills = Memory.ReadByte(Addresses.NumberOfColossiKilled);
            var total = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "colossi_quantity");

            if (APHelpers.isInTheGame() && gridLetter == 0x46 && gridNumber == 0x08 && currentKills == total + 1)
            {
                client.SendGoalCompletion();
            }
        }

        internal static bool CheckLizardHuntWinCondition(ArchipelagoClient client)
        {
            int tailCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Lizard Tail"));
            int maxCount = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "lizard_quantity");
            return tailCount >= maxCount;
        }

        internal static bool CheckShardHuntWinCondition(ArchipelagoClient client)
        {
            int shardCount = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Soul Shard"));
            int maxCount = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "soul_shard_quantity");
            return shardCount >= maxCount;
        }

        public static bool CheckGoalCondition(ArchipelagoClient client)
        {
            // TODO Victory logic goes into each of these goal conditions

            PlayerVictoryConditions goalCondition = PlayerStateHelpers.GetPlayerOption<PlayerVictoryConditions>(client.Options, "goal");

            if (goalCondition == PlayerVictoryConditions.KILL_ALL_COLOSSI)
            {
                CheckColossiWinCondition(client);
            }
            else if (goalCondition == PlayerVictoryConditions.HUNT_ALL_LIZARDS)
            {
                CheckLizardHuntWinCondition(client);
            }
            else if (goalCondition == PlayerVictoryConditions.SOUL_SHARD_SEARCH)
            {
                CheckShardHuntWinCondition(client);
            }
            return false;
        }

        public static Dictionary<int, (uint address, int bit)> BossToBitDictionary = new Dictionary<int, (uint address, int bit)>
        {
            { 1,  (Addresses.ColossusGraves2, 2) },
            { 2,  (Addresses.ColossusGraves2, 1) },
            { 3,  (Addresses.ColossusGraves1, 5) },
            { 4,  (Addresses.ColossusGraves2, 5) },
            { 5,  (Addresses.ColossusGraves1, 6) },
            { 6,  (Addresses.ColossusGraves2, 3) },
            { 7,  (Addresses.ColossusGraves3, 4) },
            { 8,  (Addresses.ColossusGraves3, 3) },
            { 9,  (Addresses.ColossusGraves1, 7) },
            { 10, (Addresses.ColossusGraves3, 0) },
            { 11, (Addresses.ColossusGraves2, 6) },
            { 12, (Addresses.ColossusGraves2, 7) },
            { 13, (Addresses.ColossusGraves2, 0) },
            { 14, (Addresses.ColossusGraves3, 2) },
            { 15, (Addresses.ColossusGraves2, 4) },
            { 16, (Addresses.ColossusGraves3, 5) }
        };



        public static Dictionary<string, (string sigil, byte value)> BossStateInGridDictionary = new Dictionary<string, (string sigil, byte value)>
        {
                { "A1", ( "", 0x00) },
                { "A2", ( "", 0x00) },
                { "A3", ( "", 0x00) },
                { "A4", ( "", 0x00) },
                { "A5", ( "", 0x00) },
                { "A6", ( "", 0x00) },
                { "A7", ( "", 0x00) },
                { "A8", ( "", 0x00) },
                { "B1", ( "", 0x00) },
                { "B2", ( "", 0x00) },
                { "B3", ( "", 0x00) },
                { "B4", ( "Sigil of the Devouring Wind", 0x0a) },
                { "B5", ( "", 0x00) },
                { "B6", ( "", 0x00) },
                { "B7", ( "", 0x00) },
                { "B8", ( "", 0x00) },
                { "C1", ( "", 0x00) },
                { "C2", ( "Sigil of Ruined Pride", 0x0e) },
                { "C3", ( "", 0x00) },
                { "C4", ( "", 0x00) },
                { "C5", ( "", 0x00) },
                { "C6", ( "", 0x00) },
                { "C7", ( "", 0x00) },
                { "C8", ( "", 0x00) },
                { "D1", ( "Sigil of the Sunken Pulse", 0x07) },
                { "D2", ( "", 0x00) },
                { "D3", ( "Sigil of the Sealed Core", 0x09) },
                { "D4", ( "", 0x00) },
                { "D5", ( "", 0x00) },
                { "D6", ( "Sigil of the Hollow Shrine", 0x06) },
                { "D7", ( "", 0x00) },
                { "D8", ( "", 0x00) },
                { "E1", ( "", 0x00) },
                { "E2", ( "Sigil of the Fallen Oath", 0x03) },
                { "E3", ( "", 0x00) },
                { "E4", ( "", 0x00) },
                { "E5", ( "", 0x00) },
                { "E6", ( "Sigil of Endless Horizon", 0x0e) },
                { "E7", ( "", 0x00) },
                { "E8", ( "", 0x00) },
                { "F1", ( "Sigil of the Broken Courage", 0x0b) },
                { "F2", ( "", 0x00) },
                { "F3", ( "Sigil of Burdened Earth", 0x02) },
                { "F4", ( "", 0x00) },
                { "F5", ( "Sigil of the First Awakening", 0x01) },
                { "F6", ( "", 0x00) },
                { "F7", ( "", 0x00) },
                { "F8", ( "", 0x0f) },
                { "G1", ( "Sigil of the Bound Colossus", 0x00) },
                { "G2", ( "Sigil of the Drowned Throne", 0x0c) },
                { "G3", ( "", 0x00) },
                { "G4", ( "", 0x00) },
                { "G5", ( "Sigil of Veiled Fear", 0x04) },
                { "G6", ( "Sigil of the Watching Walls", 0x08) },
                { "G7", ( "", 0x00) },
                { "G8", ( "", 0x00) },
                { "H1", ( "", 0x00) },
                { "H2", ( "", 0x00) },
                { "H3", ( "", 0x00) },
                { "H4", ( "Sigil of the Skybound Silence", 0x05) },
                { "H5", ( "", 0x00) },
                { "H6", ( "", 0x00) },
                { "H7", ( "", 0x00) },
                { "H8", ( "", 0x00) },
                { "I1", ( "", 0x00) },
                { "I2", ( "", 0x00) },
                { "I3", ( "", 0x00) },
                { "I4", ( "", 0x00) },
                { "I5", ( "", 0x00) },
                { "I6", ( "", 0x00) },
                { "I7", ( "", 0x00) },
                { "I8", ( "", 0x00) },
                { "J1", ( "", 0x00) },
                { "J2", ( "", 0x00) },
                { "J3", ( "", 0x00) },
                { "J4", ( "", 0x00) },
                { "J5", ( "", 0x00) },
                { "J6", ( "", 0x00) },
                { "J7", ( "", 0x00) },
                { "J8", ( "", 0x00) },
        };

        public static readonly Dictionary<int, (byte graves1, byte graves2, byte graves3)> ColossusLoadedBytes
            = new Dictionary<int, (byte, byte, byte)>
        {
            { 1,  (0xE0, 0xFB, 0x3F) },
            { 2,  (0xE0, 0xFD, 0x3F) },
            { 3,  (0xC0, 0xFF, 0x3F) },
            { 4,  (0xE0, 0xDF, 0x3F) },
            { 5,  (0xA0, 0xFF, 0x3F) },
            { 6,  (0xE0, 0xF7, 0x3F) },
            { 7,  (0xE0, 0xFF, 0x2F) },
            { 8,  (0xE0, 0xFF, 0x37) },
            { 9,  (0x60, 0xFF, 0x3F) },
            { 10, (0xE0, 0xFF, 0x3E) },
            { 11, (0xE0, 0xBF, 0x3F) },
            { 12, (0xE0, 0x7F, 0x3F) },
            { 13, (0xE0, 0xFE, 0x3F) },
            { 14, (0xE0, 0xFF, 0x3B) },
            { 15, (0xE0, 0xEF, 0x3F) },
            { 16, (0xFF, 0xFF, 0xDF) },
        };

        public const byte GravesAllUnloaded1 = 0xE0;
        public const byte GravesAllUnloaded2 = 0xFF;
        public const byte GravesAllUnloaded3 = 0x3F;

        // the in-game check is used as a current colossus value 
        // if it's set to the number of the colossus, the grave won't appear.
        // If

        public static void SetCurrentColossiState(ArchipelagoClient client)
        {
            // 1. Identify current grid
            var gridLetterInHex = Memory.ReadByte(Addresses.GridMapLetter);
            string gridLetter = LocationHelpers.BytesToCharacter[gridLetterInHex];
            string gridNumber = Memory.ReadByte(Addresses.GridMapNumber).ToString();

            if (!BossStateInGridDictionary.TryGetValue(gridLetter + gridNumber, out var dictVal))
                return;

            if ((gridLetter + gridNumber) == "F8" && PlayerStateHelpers.HasKilledAllColossi(client))
            {

                var finalBoss = ColossusLoadedBytes[16];

                Memory.WriteByte(Addresses.ColossusGraves1, finalBoss.graves1);
                Memory.WriteByte(Addresses.ColossusGraves2, finalBoss.graves2);
                Memory.WriteByte(Addresses.ColossusGraves3, finalBoss.graves3);
                Memory.WriteByte(Addresses.CheckStatues1, 0x00);
                Memory.WriteByte(Addresses.CheckStatues2, 0x80);
                Memory.Write(Addresses.FinalZone, 0x0000839);

                Memory.WriteByte(Addresses.InGameCheck, 0x0f);
                return;
            }
            else if ((gridLetter + gridNumber) == "F8" && !PlayerStateHelpers.HasKilledAllColossi(client))
            {
                int count = client.CurrentSession.Items.AllItemsReceived.Count(item => item.ItemName.Contains("Idol Shard"));
                int expectedCount = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "colossi_quantity");
                Console.WriteLine($"Not enough Idol Shard Collected. {count}/{expectedCount}");
            }

            // 2. Non-colossus grid → leave state alone, the previous colossus grid's
            //    write still applies and nothing in this area cares
            if (string.IsNullOrEmpty(dictVal.sigil))
                return;

            if (!ItemHelpers.SigilNameToColossus.TryGetValue(dictVal.sigil, out int currentBossId))
                return;

            // 3. Sigil ownership check
            bool hasCurrentSigil = client.CurrentSession.Items.AllItemsReceived
                .Any(item => item.ItemName.Equals(dictVal.sigil, StringComparison.OrdinalIgnoreCase));

            // 4. ALWAYS write InGameCheck to this grid's colossus (0-indexed).
            //    The locked-invisible state requires InGameCheck to match the grid's colossus.
            //    Without this write, no-sigil players see a grave instead of nothing.
            Memory.WriteByte(Addresses.InGameCheck, (byte)(dictVal.value - 1));

            // 5. State table:
            //    has sigil  → bit cleared (boss loads, no grave)
            //    no sigil   → bit set + matching InGameCheck (nothing visible)
            if (hasCurrentSigil && ColossusLoadedBytes.TryGetValue(currentBossId, out var b))
            {
                Memory.WriteByte(Addresses.ColossusGraves1, b.graves1);
                Memory.WriteByte(Addresses.ColossusGraves2, b.graves2);
                Memory.WriteByte(Addresses.ColossusGraves3, b.graves3);
#if DEBUG
                Console.WriteLine($"Grid {gridLetter}{gridNumber}: Col {currentBossId} ALIVE ({dictVal.sigil})");
#endif
            }
            else
            {
                Memory.WriteByte(Addresses.ColossusGraves1, GravesAllUnloaded1);
                Memory.WriteByte(Addresses.ColossusGraves2, GravesAllUnloaded2);
                Memory.WriteByte(Addresses.ColossusGraves3, GravesAllUnloaded3);
#if DEBUG
                Console.WriteLine($"Grid {gridLetter}{gridNumber}: Col {currentBossId} LOCKED ({dictVal.sigil})");
#endif
            }
        }

        public static void SetCurrentSigilState(ArchipelagoClient client)
        {
            var check = Memory.ReadByte(Addresses.InGameCheck);

            if (check == 0xFF) return;
            SetCurrentColossiState(client);
        }

        public static void SetUpNewGameListener(CancellationTokenSource cts, ArchipelagoClient client)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<ushort>(
                Addresses.InGameCheck,
                () =>
                {
                    GameLoaded(client);
                    Memory.MonitorAddressForAction<ushort>(
                        Addresses.InGameCheck,
                        () =>
                        {
                            SetUpNewGameListener(cts, client);
                        },
                    val => val == 0xFF);

                }, value => value != 0xFF);
        }

        public static void SetUpSaveGameListener(CancellationTokenSource cts, ArchipelagoClient client)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<ushort>(
                Addresses.SaveDataBar,
                () =>
                {
                    Memory.Write(Addresses.ItemIndexStorage, (ushort)App.ProcessedItemIndex);
                    SetUpSaveGameListener(cts, client);
                }, value => value == 0xCC);
        }

        public static void GameLoaded(ArchipelagoClient client)
        {
            var index = Memory.ReadUShort(Addresses.ItemIndexStorage);
            App.ProcessedItemIndex = index;
#if DEBUG
            Console.WriteLine($"Loaded item index: {App.ProcessedItemIndex}");
#endif
            // Immediately try to process any pending items
            PlayerStateHelpers.UpdatePlayerState(client);
        }

        public static void SetColossiKilled(CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.CheckStatues1,
                () =>
                {
                    Memory.WriteByte(Addresses.CheckStatues1, 0x00);
                    Memory.WriteByte(Addresses.CheckStatues2, 0x80);
                },
            value => value != 0xFF);
        }

        public static void SetColossiGridUpdate(ArchipelagoClient client, CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.GridMapFull,
                () =>
                {
                    CheckGoalCondition(client);
                    var gridValue = Memory.ReadByte(Addresses.GridMapFull);
                    _gridPreviousValue = gridValue;
                    SetCurrentSigilState(client);
                    SetColossiGridUpdate(client, cts);
                },
                value => value != _gridPreviousValue);
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