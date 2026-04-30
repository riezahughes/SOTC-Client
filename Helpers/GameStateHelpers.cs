using Archipelago.Core;
using Archipelago.Core.Util;
using SotcArchipelago;

namespace Helpers
{
    public class GameStateHelpers
    {
        private static byte _gridPreviousValue = 0xFF;
        internal static void CheckColossiWinCondition(ArchipelagoClient client)
        {
            var gridLetter = Memory.ReadByte(Addresses.GridMapLetter);
            var gridNumber = Memory.ReadByte(Addresses.GridMapNumber);
            if (APHelpers.isInTheGame() && gridLetter == 0x46 && gridNumber == 0x08)
            {
                client.SendGoalCompletion();
            }
        }

        internal static bool CheckLizardHuntWinCondition(ArchipelagoClient client)
        {
            return true;
        }

        internal static bool CheckShardHuntWinCondition(ArchipelagoClient client)
        {
            return true;
        }

        public static bool CheckGoalCondition(ArchipelagoClient client)
        {
            // TODO Victory logic goes into each of these goal conditions

            int goalCondition = Int32.Parse(client.Options?.GetValueOrDefault("goal", "0").ToString());

            if (goalCondition == PlayerVictoryConditions.KILL_ALL_COLOSSI)
            {
                CheckColossiWinCondition(client);
            }
            else if (goalCondition == PlayerVictoryConditions.KILL_ALL_LIZARDS)
            {
                CheckLizardHuntWinCondition(client);
            }
            else if (goalCondition == PlayerVictoryConditions.COLLECTED_ALL_SHARDS)
            {
                CheckShardHuntWinCondition(client);
            }
            return false;
        }

        public static Dictionary<int, int> BossToBitDictionary = new Dictionary<int, int>()
        {
            { 1, 2 },   // Address 012D9B5C
            { 2, 1 },   // Address 012D9B5C
            { 3, 5 },   // Address 012D9B5B
            { 4, 5 },   // Address 012D9B5C
            { 5, 6 },   // Address 012D9B5B
            { 6, 3 },   // Address 012D9B5C
            { 7, 4 },   // Address 012D9B5D
            { 8, 3 },   // Address 012D9B5D
            { 9, 7 },   // Address 012D9B5B
            { 10, 0 },  // Address 012D9B5D
            { 11, 6 },  // Address 012D9B5C
            { 12, 7 },  // Address 012D9B5C
            { 13, 0 },  // Address 012D9B5C
            { 14, 2 },  // Address 012D9B5D
            { 15, 4 },  // Address 012D9B5C
            { 16, 5 }   // Address 012D9B5D
        };

        public static Dictionary<string, byte> BossStateInGridDictionary = new Dictionary<string, byte>()
        {
                { "A1", 0x00 },
                { "A2", 0x00 },
                { "A3", 0x00 },
                { "A4", 0x00 },
                { "A5", 0x00 },
                { "A6", 0x00 },
                { "A7", 0x00 },
                { "A8", 0x00 },
                { "B1", 0x00 },
                { "B2", 0x00 },
                { "B3", 0x00 },
                { "B4", 0x0a },
                { "B5", 0x00 },
                { "B6", 0x00 },
                { "B7", 0x00 },
                { "B8", 0x00 },
                { "C1", 0x00 },
                { "C2", 0x0e },
                { "C3", 0x00 },
                { "C4", 0x00 },
                { "C5", 0x00 },
                { "C6", 0x00 },
                { "C7", 0x00 },
                { "C8", 0x00 },
                { "D1", 0x07 },
                { "D2", 0x00 },
                { "D3", 0x09 },
                { "D4", 0x00 },
                { "D5", 0x00 },
                { "D6", 0x06 },
                { "D7", 0x00 },
                { "D8", 0x00 },
                { "E1", 0x00 },
                { "E2", 0x03 },
                { "E3", 0x00 },
                { "E4", 0x00 },
                { "E5", 0x00 },
                { "E6", 0x0d },
                { "E7", 0x00 },
                { "E8", 0x00 },
                { "F1", 0x0b },
                { "F2", 0x00 },
                { "F3", 0x02 },
                { "F4", 0x00 },
                { "F5", 0x01 },
                { "F6", 0x00 },
                { "F7", 0x00 },
                { "F8", 0x0f },
                { "G1", 0x00 },
                { "G2", 0x0c },
                { "G3", 0x00 },
                { "G4", 0x00 },
                { "G5", 0x04 },
                { "G6", 0x08 },
                { "G7", 0x00 },
                { "G8", 0x00 },
                { "H1", 0x00 },
                { "H2", 0x00 },
                { "H3", 0x00 },
                { "H4", 0x05 },
                { "H5", 0x04 },
                { "H6", 0x00 },
                { "H7", 0x00 },
                { "H8", 0x00 },
                { "I1", 0x00 },
                { "I2", 0x00 },
                { "I3", 0x00 },
                { "I4", 0x00 },
                { "I5", 0x00 },
                { "I6", 0x00 },
                { "I7", 0x00 },
                { "I8", 0x00 },
                { "J1", 0x00 },
                { "J2", 0x00 },
                { "J3", 0x00 },
                { "J4", 0x00 },
                { "J5", 0x00 },
                { "J6", 0x00 },
                { "J7", 0x00 },
                { "J8", 0x00 },

        };

        public static void SetCurrentColossiState()
        {
            var gridLetterInHex = Memory.ReadByte(Addresses.GridMapLetter);
            string gridLetter = LocationHelpers.BytesToCharacter[gridLetterInHex];
            string gridNumber = Memory.ReadByte(Addresses.GridMapNumber).ToString();

            Memory.Write(Addresses.InGameCheck, BossStateInGridDictionary[gridLetter + gridNumber]);

        }

        public static uint SetCurrentSigilState(ArchipelagoClient client)
        {
            // this is clearly firing too much. need to solve this issue so that
            // i can successfully hide a colossus corpse. Maybe set it to specific grid points instead of 
            // constantly changing this like eveyrthing else.
            var sigilItems = client.CurrentSession.Items.AllItemsReceived
                .Where(item => item.ItemName.Contains("sigil", StringComparison.OrdinalIgnoreCase));

            var check = Memory.ReadByte(Addresses.InGameCheck);

            Memory.WriteByte(Addresses.CheckStatues1, 0x00);
            Memory.WriteByte(Addresses.CheckStatues2, 0x80);

            // show all graves
            //Memory.WriteByte(Addresses.ColossusGraves1, 0xFF);
            //Memory.WriteByte(Addresses.ColossusGraves2, 0xFF);
            //Memory.WriteByte(Addresses.ColossusGraves3, 0xDF);

            Memory.WriteByte(Addresses.ColossusGraves1, 0xE0);
            Memory.WriteByte(Addresses.ColossusGraves2, 0xFF);
            Memory.WriteByte(Addresses.ColossusGraves3, 0x3D);


            if (check != 0xFF)
            {
                SetCurrentColossiState();
            }

            //Memory.Write(Addresses.SkipCutscenes1, 0x0839);
            //Memory.Write(Addresses.SkipCutscenes2, 0x01);

            foreach (var sigilItem in sigilItems)
            {

                //switch (sigilItem.ItemName)
                //{
                //    case "Sigil of the First Awakening":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 2, true);
                //        break;
                //    case "Sigil of Burdened Earth":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 1, true);
                //        break;
                //    case "Sigil of the Fallen Oath":
                //        Memory.WriteBit(Addresses.ColossusGraves1, 5, true);
                //        break;
                //    case "Sigil of Veiled Fear":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 5, true);
                //        break;
                //    case "Sigil of the Skybound Silence":
                //        Memory.WriteBit(Addresses.ColossusGraves1, 6, true);
                //        break;
                //    case "Sigil of the Hollow Shrine":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 3, true);
                //        break;
                //    case "Sigil of the Sunken Pulse":
                //        Memory.WriteBit(Addresses.ColossusGraves3, 4, true);
                //        break;
                //    case "Sigil of the Watching Walls":
                //        Memory.WriteBit(Addresses.ColossusGraves3, 3, true);
                //        break;
                //    case "Sigil of the Sealed Core":
                //        Memory.WriteBit(Addresses.ColossusGraves1, 7, true);
                //        break;
                //    case "Sigil of the Devouring Wind":
                //        Memory.WriteBit(Addresses.ColossusGraves3, 0, true);
                //        break;
                //    case "Sigil of the Broken Courage":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 6, true);
                //        break;
                //    case "Sigil of the Drowned Throne":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 7, true);
                //        break;
                //    case "Sigil of Endless Horizon":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 0, true);
                //        break;
                //    case "Sigil of Ruined Pride":
                //        Memory.WriteBit(Addresses.ColossusGraves3, 2, true);
                //        break;
                //    case "Sigil of the Bound Colossus":
                //        Memory.WriteBit(Addresses.ColossusGraves2, 4, true);
                //        break;
                //    default:
                //        break;

                //}
            }

            return 0x00;
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
                Addresses.NumberOfColossiKilled,
                () =>
                {
                    Memory.Write(Addresses.NumberOfColossiKilled, 0x0e);
                    SetColossiKilled(cts);
                },
            value => value != 0x0e);
        }

        public static void SetColossiGridUpdate(ArchipelagoClient client, CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.GridMapFull,
                () =>
                {
                    var gridValue = Memory.ReadByte(Addresses.GridMapFull);
                    SetCurrentSigilState(client);
                    SetColossiGridUpdate(client, cts);
                    _gridPreviousValue = gridValue;
                },
            value => value != _gridPreviousValue);
        }

        public static void CheckStatues(ArchipelagoClient client, CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            Memory.MonitorAddressForAction<byte>(
                Addresses.ColossusVisibility1,
                () =>
                {
                    // Collosi load state. As you get keys, these should change
                    Memory.Write(Addresses.ColossusVisibility1, 0xFF);
                    Memory.Write(Addresses.ColossusVisibility2, 0xFF);

                },
            value => value != 0xFF);

            Memory.MonitorAddressForAction<byte>(
                Addresses.ColossusVisibility2,
                () =>
                {
                    // Collosi load state. As you get keys, these should change
                    Memory.Write(Addresses.ColossusVisibility1, 0xFF);
                    Memory.Write(Addresses.ColossusVisibility2, 0xFF);
                    SetCurrentSigilState(client);
                    CheckStatues(client, cts);
                },
            value => value != 0xFF);
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