using Archipelago.Core;
using Archipelago.Core.Util;
using SotcArchipelago;
using SotcArchipelago.Helpers;

namespace Helpers
{
    public class GameStateHelpers
    {
        private static byte _gridPreviousValue = 0xFF;
        internal static void CheckColossiWinCondition(ArchipelagoClient client)
        {
            var gridLetter = Memory.ReadByte(Addresses.GridMapLetter);
            var gridNumber = Memory.ReadByte(Addresses.GridMapNumber);
            var murderCount = Memory.ReadByte(Addresses.NumberOfColossiKilled);
            if (APHelpers.isInTheGame() && gridLetter == 0x46 && gridNumber == 0x08 && murderCount > 16)
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
                { "E6", ( "Sigil of Endless Horizon", 0x0d) },
                { "E7", ( "", 0x00) },
                { "E8", ( "", 0x00) },
                { "F1", ( "Sigil of the Broken Courage", 0x0b) },
                { "F2", ( "", 0x00) },
                { "F3", ( "Sigil of Burdened Earth", 0x02) },
                { "F4", ( "", 0x00) },
                { "F5", ( "Sigil of the First Awakening", 0x01) },
                { "F6", ( "", 0x00) },
                { "F7", ( "", 0x00) },
                { "F8", ( "Sigil of the Bound Colossus", 0x0f) },
                { "G1", ( "", 0x00) },
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

        // the in-game check is used as a current colossus value 
        // if it's set to the number of the colossus, the grave won't appear.
        // If

        public static void SetCurrentColossiState(ArchipelagoClient client)
        {
            var gridLetterInHex = Memory.ReadByte(Addresses.GridMapLetter);
            string gridLetter = LocationHelpers.BytesToCharacter[gridLetterInHex];
            string gridNumber = Memory.ReadByte(Addresses.GridMapNumber).ToString();

            var dictVal = BossStateInGridDictionary[gridLetter + gridNumber];

            var sigilItems = client.CurrentSession.Items.AllItemsReceived
                .Where(item => item.ItemName.Contains(dictVal.sigil, StringComparison.OrdinalIgnoreCase));


            foreach (var sigilName in sigilItems)
            {
                if (ItemHelpers.SigilNameToColossus.TryGetValue(sigilName.ItemName, out int bossId))
                {

                    if (BossToBitDictionary.TryGetValue(bossId, out var colChoice))
                    {
#if DEBUG
                        Console.WriteLine($"Setting Collosi {bossId} ");
#endif
                        Memory.WriteBit(colChoice.address, colChoice.bit, true);
                        Memory.Write(Addresses.InGameCheck, dictVal.value);
                    }
                }
            };
        }

        public static void SetCurrentSigilState(ArchipelagoClient client)
        {


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
                SetCurrentColossiState(client);
            }

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
                    // 1. Grab the new value immediately
                    var gridValue = Memory.ReadByte(Addresses.GridMapFull);

                    // 2. Update the tracking variable BEFORE re-registering
                    // This is the most important step to prevent the "double fire"
                    _gridPreviousValue = gridValue;

                    // 3. Perform your logic
                    SetCurrentSigilState(client);

                    // 4. Re-register the monitor for the NEXT change
                    SetColossiGridUpdate(client, cts);
                },
                value => value != _gridPreviousValue);
        }

        public static void CheckStatues(ArchipelagoClient client, CancellationTokenSource cts)
        {
            if (cts.Token.IsCancellationRequested) return;

            //Memory.MonitorAddressForAction<byte>(
            //    Addresses.ColossusVisibility1,
            //    () =>
            //    {
            //        // Collosi load state. As you get keys, these should change
            //        Memory.Write(Addresses.ColossusVisibility1, 0xFF);
            //        Memory.Write(Addresses.ColossusVisibility2, 0xFF);

            //    },
            //value => value != 0xFF);

            //Memory.MonitorAddressForAction<byte>(
            //    Addresses.ColossusVisibility2,
            //    () =>
            //    {
            //        // Collosi load state. As you get keys, these should change
            //        Memory.Write(Addresses.ColossusVisibility1, 0xFF);
            //        Memory.Write(Addresses.ColossusVisibility2, 0xFF);
            //        SetCurrentSigilState(client);
            //        CheckStatues(client, cts);
            //    },
            //value => value != 0xFF);
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