using Archipelago.Core;
using Archipelago.Core.Models;
using Archipelago.Core.Util;
using Serilog;
using SotcArchipelago;
using SotcArchipelago.Models;
using SotcArchipelago.Options;

namespace Helpers
{
    public class LocationHelpers
    {

        public static List<int> GetCurrentColossiInRun(ArchipelagoClient client)
        {
            List<int> colList = new List<int>();

            var allLocationIds = client.CurrentSession.Locations.AllLocations;

            foreach (long id in allLocationIds)
            {
                if (ColossusLocationReferenceByAPID.TryGetValue((int)id, out int colossiNumber))
                {
                    colList.Add(colossiNumber);
                }
            }

            return colList.OrderBy(i => i).ToList();
        }

        public static Dictionary<string, int> ColossusLocationReferenceByLocName = new Dictionary<string, int>()
        {
            { "Mintaur A Kill - Col. 1", 1 },
            { "Mammoth Kill - Col. 2", 2 },
            { "Knight Kill - Col. 3", 3 },
            { "Kirin Kill - Col. 4", 4 },
            { "Bird Kill - Col. 5", 5 },
            { "Minotaur B Kill - Col. 6", 6 },
            { "Eel Kill - Col. 7", 7 },
            { "Yamori B Kill - Col. 8", 8 },
            { "Kame Kill - Col. 9", 9 },
            { "Narga Kill - Col. 10", 10 },
            { "Leo Kill - Col. 11", 11 },
            { "Poseidon Kill - Col. 12", 12 },
            { "Snake Kill - Col. 13", 13 },
            { "Cerberus Kill - Col. 14", 14 },
            { "Minotaur C Kill - Col. 15", 15 },
            { "Evis Kill - Col. 16", 16 }
        };

        public static Dictionary<int, int> ColossusLocationReferenceByAPID = new Dictionary<int, int>()
        {
            { 90056000, 1 },  // Mintaur A Kill - Col. 1
            { 90053000, 2 },  // Mammoth Kill - Col. 2
            { 90050000, 3 },  // Knight Kill - Col. 3
            { 90057000, 4 },  // Kirin Kill - Col. 4
            { 90055000, 5 },  // Bird Kill - Col. 5
            { 90058000, 6 },  // Minotaur B Kill - Col. 6
            { 90046000, 7 },  // Eel Kill - Col. 7
            { 90060000, 8 },  // Yamori B Kill - Col. 8
            { 90052000, 9 },  // Kame Kill - Col. 9
            { 90054000, 10 }, // Narga Kill - Col. 10
            { 90047000, 11 }, // Leo Kill - Col. 11
            { 90051000, 12 }, // Poseidon Kill - Col. 12
            { 90059000, 13 }, // Snake Kill - Col. 13
            { 90049000, 14 }, // Cerberus Kill - Col. 14
            { 90048000, 15 }, // Minotaur C Kill - Col. 15
            { 90061000, 16 }  // Evis Kill - Col. 16
        };

        private static List<int> GenerateDistanceMilestones(int range, int breakpoints)
        {
            var milestones = new List<int>();
            for (int i = 1; i <= range / breakpoints; i++)
                milestones.Add(i * breakpoints);
            if (range % breakpoints != 0)
                milestones.Add(range);
            return milestones;
        }

        public static async Task StartTraversalMonitor(ArchipelagoClient client, CancellationToken cancellationToken)
        {
            var climbSanitySetting = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "climbsanity");
            var agroSanitySetting = PlayerStateHelpers.GetPlayerOption<ToggleOptions>(client.Options, "agrosanity");

            bool climbsanityOn = climbSanitySetting == ToggleOptions.TRUE;
            bool agrosanityOn = agroSanitySetting == ToggleOptions.TRUE;

            if (!climbsanityOn && !agrosanityOn) return;

            int climbRange = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "climbsanity_range", 5000);
            int climbBp = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "climbsanity_break_points", 100);
            int rideRange = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "argosanity_range", 50000);
            int rideBp = PlayerStateHelpers.GetPlayerOptionCounts(client.Options, "argosanity_break_points", 1000);

            var climbMilestones = climbsanityOn ? GenerateDistanceMilestones(climbRange, climbBp) : new List<int>();
            var rideMilestones = agrosanityOn ? GenerateDistanceMilestones(rideRange, rideBp) : new List<int>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(5000, cancellationToken);

                    if (!APHelpers.isInTheGame()) continue;

                    var checkedLocations = client.CurrentSession.Locations.AllLocationsChecked;

                    if (climbsanityOn)
                    {
                        var climbActivity = Memory.ReadUShort(Addresses.ControllerBytes);
                        if (climbActivity == 0xF7FF)
                            App.ClimbDistanceAccumulated += 20;
#if DEBUG
                        Console.WriteLine($"Climbsanity Distance: {App.ClimbDistanceAccumulated}");
#endif
                        foreach (var milestone in climbMilestones)
                        {
                            if (App.ClimbDistanceAccumulated >= milestone)
                            {
                                long locationId = 98000000L + milestone - 1;
                                if (!checkedLocations.Contains(locationId))
                                {

                                    Console.WriteLine("Sending Climb Location");

                                    client.SendLocationAsync(new Location
                                    {
                                        Id = (int)locationId,
                                        Name = $"Climbing Distance: {milestone}",
                                        CheckType = LocationCheckType.Byte
                                    });
                                }
                            }
                        }
                    }

                    if (agrosanityOn)
                    {
                        var rideActivity = Memory.ReadByte(Addresses.AgroListener);
                        if (rideActivity >= 0x60)
                            App.RideDistanceAccumulated += 50;
#if DEBUG
                        Console.WriteLine($"ArgoSanity Distance: {App.RideDistanceAccumulated}");
#endif
                        foreach (var milestone in rideMilestones)
                        {
                            if (App.RideDistanceAccumulated >= milestone)
                            {
                                long locationId = 99000000L + milestone - 1;
                                if (!checkedLocations.Contains(locationId))
                                {
                                    Console.WriteLine("Sending Agro Location");
                                    client.SendLocationAsync(new Location
                                    {
                                        Id = (int)locationId,
                                        Name = $"Riding Distance: {milestone}",
                                        CheckType = LocationCheckType.Byte
                                    });
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error($"Traversal monitor error: {ex.Message}");
                }
            }
        }

        public static Dictionary<string, byte> CharacterToBytes = new Dictionary<string, byte>()
        {
            {"A", 0x41},
            {"B", 0x42},
            {"C", 0x43},
            {"D", 0x44},
            {"E", 0x45},
            {"F", 0x46},
            {"G", 0x47},
            {"H", 0x48},
            {"J", 0x49},
        };

        public static Dictionary<byte, string> BytesToCharacter = new Dictionary<byte, string>()
        {
            {0x41, "A" },
            {0x42, "B" },
            {0x43, "C" },
            {0x44, "D" },
            {0x45, "E" },
            {0x46, "F" },
            {0x47, "G" },
            {0x48, "H" },
            {0x49, "J" },
        };

        public static List<ILocation> BuildLocationList(Dictionary<string, object> options)
        {
            int base_id = 90000000;
            int region_offset = 1000;

            int climb_id_base = 98000000;
            int ride_id_base = 99000000;

            var regional_index = 0;

            List<string> table_order = [
            "Grid F0",
            "Grid C1",
            "Grid D1",
            "Grid E1",
            "Grid F1",
            "Grid G1",
            "Grid C2",
            "Grid D2",
            "Grid E2",
            "Grid F2",
            "Grid G2",
            "Grid B3",
            "Grid C3",
            "Grid D3",
            "Grid E3",
            "Grid F3",
            "Grid G3",
            "Grid A4",
            "Grid B4",
            "Grid C4",
            "Grid D4",
            "Grid E4",
            "Grid F4",
            "Grid G4",
            "Grid H4",
            "Grid A5",
            "Grid B5",
            "Grid C5",
            "Grid D5",
            "Grid E5",
            "Grid F5",
            "Grid G5",
            "Grid C6",
            "Grid D6",
            "Grid E6",
            "Grid F6",
            "Grid G6",
            "Grid H6",
            "Grid D7",
            "Grid E7",
            "Grid F7",
            "Grid G7",
            "Grid H7",
            "Grid E8",
            "Grid F8",
            "Grid G8",
            "Boss D1",
            "Boss F1",
            "Boss G1",
            "Boss C2",
            "Boss E2",
            "Boss G2",
            "Boss D3",
            "Boss F3",
            "Boss B4",
            "Boss H4",
            "Boss F5",
            "Boss G5",
            "Boss D6",
            "Boss E6",
            "Boss G6",
            "Boss F8",
            ];

            List<ILocation> locations = new List<ILocation>();

            Dictionary<string, List<GenericLocationData>> allLevelLocations = new Dictionary<string, List<GenericLocationData>>();

            // Grids
            allLevelLocations.Add("Grid F0", GetGridF0Data(options));
            allLevelLocations.Add("Grid C1", GetGridC1Data(options));
            allLevelLocations.Add("Grid D1", GetGridD1Data(options));
            allLevelLocations.Add("Grid E1", GetGridE1Data(options));
            allLevelLocations.Add("Grid F1", GetGridF1Data(options));
            allLevelLocations.Add("Grid G1", GetGridG1Data(options));
            allLevelLocations.Add("Grid C2", GetGridC2Data(options));
            allLevelLocations.Add("Grid D2", GetGridD2Data(options));
            allLevelLocations.Add("Grid E2", GetGridE2Data(options));
            allLevelLocations.Add("Grid F2", GetGridF2Data(options));
            allLevelLocations.Add("Grid G2", GetGridG2Data(options));
            allLevelLocations.Add("Grid B3", GetGridB3Data(options));
            allLevelLocations.Add("Grid C3", GetGridC3Data(options));
            allLevelLocations.Add("Grid D3", GetGridD3Data(options));
            allLevelLocations.Add("Grid E3", GetGridE3Data(options));
            allLevelLocations.Add("Grid F3", GetGridF3Data(options));
            allLevelLocations.Add("Grid G3", GetGridG3Data(options));
            allLevelLocations.Add("Grid A4", GetGridA4Data(options));
            allLevelLocations.Add("Grid B4", GetGridB4Data(options));
            allLevelLocations.Add("Grid C4", GetGridC4Data(options));
            allLevelLocations.Add("Grid D4", GetGridD4Data(options));
            allLevelLocations.Add("Grid E4", GetGridE4Data(options));
            allLevelLocations.Add("Grid F4", GetGridF4Data(options));
            allLevelLocations.Add("Grid G4", GetGridG4Data(options));
            allLevelLocations.Add("Grid H4", GetGridH4Data(options));
            allLevelLocations.Add("Grid A5", GetGridA5Data(options));
            allLevelLocations.Add("Grid B5", GetGridB5Data(options));
            allLevelLocations.Add("Grid C5", GetGridC5Data(options));
            allLevelLocations.Add("Grid D5", GetGridD5Data(options));
            allLevelLocations.Add("Grid E5", GetGridE5Data(options));
            allLevelLocations.Add("Grid F5", GetGridF5Data(options));
            allLevelLocations.Add("Grid G5", GetGridG5Data(options));
            allLevelLocations.Add("Grid C6", GetGridC6Data(options));
            allLevelLocations.Add("Grid D6", GetGridD6Data(options));
            allLevelLocations.Add("Grid E6", GetGridE6Data(options));
            allLevelLocations.Add("Grid F6", GetGridF6Data(options));
            allLevelLocations.Add("Grid G6", GetGridG6Data(options));
            allLevelLocations.Add("Grid H6", GetGridH6Data(options));
            allLevelLocations.Add("Grid D7", GetGridD7Data(options));
            allLevelLocations.Add("Grid E7", GetGridE7Data(options));
            allLevelLocations.Add("Grid F7", GetGridF7Data(options));
            allLevelLocations.Add("Grid G7", GetGridG7Data(options));
            allLevelLocations.Add("Grid H7", GetGridH7Data(options));
            allLevelLocations.Add("Grid E8", GetGridE8Data(options));
            allLevelLocations.Add("Grid F8", GetGridF8Data(options));
            allLevelLocations.Add("Grid G8", GetGridG8Data(options));

            // Bosses
            allLevelLocations.Add("Boss D1", GetBossD1Data(options));
            allLevelLocations.Add("Boss F1", GetBossF1Data(options));
            allLevelLocations.Add("Boss G1", GetBossG1Data(options));
            allLevelLocations.Add("Boss C2", GetBossC2Data(options));
            allLevelLocations.Add("Boss E2", GetBossE2Data(options));
            allLevelLocations.Add("Boss G2", GetBossG2Data(options));
            allLevelLocations.Add("Boss D3", GetBossD3Data(options));
            allLevelLocations.Add("Boss F3", GetBossF3Data(options));
            allLevelLocations.Add("Boss B4", GetBossB4Data(options));
            allLevelLocations.Add("Boss H4", GetBossH4Data(options));
            allLevelLocations.Add("Boss F5", GetBossF5Data(options));
            allLevelLocations.Add("Boss G5", GetBossG5Data(options));
            allLevelLocations.Add("Boss D6", GetBossD6Data(options));
            allLevelLocations.Add("Boss E6", GetBossE6Data(options));
            allLevelLocations.Add("Boss G6", GetBossG6Data(options));
            allLevelLocations.Add("Boss F8", GetBossF8Data(options));

            foreach (var region_name in table_order.ToList())
            {
                long currentRegionBaseId = base_id + regional_index * region_offset;

                if (allLevelLocations.ContainsKey(region_name))
                {
                    // Retrieve the list of locations for the current region
                    List<GenericLocationData> regionLocations = allLevelLocations[region_name];

                    var location_index = 0;

                    foreach (var loc in regionLocations)

                    {
                        int locationId = (int)currentRegionBaseId + location_index;

                        if (loc.Name.Contains("Map Grid") && loc is GridLocationData gridLocMap)
                        {
                            List<ILocation> conditionalChoice = new List<ILocation>();

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Letter",
                                Address = Addresses.GridMapLetter,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = CharacterToBytes[gridLocMap.GridLetter].ToString()
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Number",
                                Address = Addresses.GridMapNumber,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = gridLocMap.GridNumber
                            });

                            CompositeLocation location = new CompositeLocation()
                            {
                                Name = loc.Name,
                                Id = locationId,
                                CheckType = LocationCheckType.AND,
                                Conditions = conditionalChoice,
                            };

                            locations.Add(location);
                            location_index++;
                            continue;

                        }
                        else if (loc.Name.Contains("Shrine") && loc is GridLocationData gridLocShrine)
                        {
                            List<ILocation> conditionalChoice = new List<ILocation>();

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Letter",
                                Address = Addresses.GridMapLetter,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = CharacterToBytes[gridLocShrine.GridLetter].ToString()
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Number",
                                Address = Addresses.GridMapNumber,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = gridLocShrine.GridNumber
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Shrine Interaction",
                                Address = Addresses.ShrineDataPopup,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = "1"
                            });

                            CompositeLocation location = new CompositeLocation()
                            {
                                Name = loc.Name,
                                Id = locationId,
                                CheckType = LocationCheckType.AND,
                                Conditions = conditionalChoice,
                            };

                            locations.Add(location);
                            location_index++;
                            continue;

                        }
                        else if ((loc.Name.Contains("Lizard") || loc.Name.Contains("Fruit")) && loc is FruitAndLizardLocationData pickupLoc)
                        {
                            List<ILocation> conditionalChoice = new List<ILocation>();

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Letter",
                                Address = Addresses.GridMapLetter,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = CharacterToBytes[pickupLoc.GridLetter].ToString()
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Number",
                                Address = Addresses.GridMapNumber,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = pickupLoc.GridNumber
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Entity Check",
                                Address = loc.Address,
                                CheckType = LocationCheckType.Bit,
                                AddressBit = pickupLoc.BitsToChange
                            });

                            CompositeLocation location = new CompositeLocation()
                            {
                                Name = loc.Name,
                                Id = locationId,
                                CheckType = LocationCheckType.AND,
                                Conditions = conditionalChoice,
                            };

                            locations.Add(location);
                            location_index++;
                            continue;
                        }
                        else if (loc.Name.Contains("Kill") && loc is BossLocationData bossLoc)
                        {

                            List<ILocation> conditionalChoice = new List<ILocation>();

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Letter",
                                Address = Addresses.GridMapLetter,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = CharacterToBytes[bossLoc.GridLetter].ToString()
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Grid Check Number",
                                Address = Addresses.GridMapNumber,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = bossLoc.GridNumber
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "InBossFight",
                                Address = Addresses.IsFightingColossi,
                                CheckType = LocationCheckType.Byte,
                                CompareType = LocationCheckCompareType.GreaterThan,
                                CheckValue = "0"
                            });

                            conditionalChoice.Add(new Location()
                            {
                                Id = -1,
                                Name = "Boss HP",
                                Address = Addresses.ColossusHealth,
                                CheckType = LocationCheckType.UInt,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = "0"
                            });

                            CompositeLocation location = new CompositeLocation()
                            {
                                Name = loc.Name,
                                Id = locationId,
                                CheckType = LocationCheckType.AND,
                                Conditions = conditionalChoice,
                            };

                            locations.Add(location);
                            location_index++;
                            continue;

                        }
                        else
                        {

                            var location = new Location()
                            {
                                Id = locationId,
                                Name = loc.Name,
                                Address = loc.Address,
                                CheckType = loc.CheckType,
                                CompareType = LocationCheckCompareType.Match,
                                CheckValue = loc.Check
                            };
                            locations.Add(location);
                            location_index++;
                        }


                    }
                }
                regional_index++;
            }

            // regular example: A simple location and a simple change

            // loop, define and put your addresses together and at the end of this function make sure to return the original locations list we created

            return locations;
        }

        public static List<GenericLocationData> GetGridF0Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f0 = new List<GenericLocationData>() {
                new GridLocationData("Map Grid F0", "F", "0"),
            };
            return f0;
        }

        public static List<GenericLocationData> GetGridC1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c1 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C1", "C", "1"),
            new GridLocationData("C1 - Shrine", "C", "1"),
            new FruitAndLizardLocationData("C1 - Lizard - Center", Addresses.LizardC1Center, "C", "1",  0),
            new FruitAndLizardLocationData("C1 - Fruit - South 1", Addresses.FruitC1South1, "C", "1",  0),
            new FruitAndLizardLocationData("C1 - Fruit - South 2", Addresses.FruitC1South2, "C", "1",  1),
        };
            return c1;
        }

        public static List<GenericLocationData> GetGridD1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d1 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D1", "D", "1"),
        };
            return d1;
        }

        public static List<GenericLocationData> GetGridE1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e1 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E1", "E", "1"),
            new GridLocationData("E1 - Shrine", "E", "1"),
            new FruitAndLizardLocationData("E1 - Lizard - West", Addresses.LizardE1West, "E", "1", 0),
        };
            return e1;
        }

        public static List<GenericLocationData> GetGridF1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f1 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F1", "F", "1"),
            new FruitAndLizardLocationData("F1 - Lizard - Center", Addresses.LizardF1Center, "F", "1", 0),
            new FruitAndLizardLocationData("F1 - Fruit - South West", Addresses.FruitF1SouthWest, "F", "1", 0),
        };
            return f1;
        }

        public static List<GenericLocationData> GetGridG1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g1 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G1", "G", "1"),
            new GridLocationData("G1 - Shrine", "G", "1"),
            new FruitAndLizardLocationData("G1 - Lizard - Center", Addresses.LizardG1Center, "G", "1", 0),
            new FruitAndLizardLocationData("G1 - Lizard - West", Addresses.LizardG1West, "G", "1", 1),
        };
            return g1;
        }

        public static List<GenericLocationData> GetGridC2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c2 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C2", "C", "2"),
        };
            return c2;
        }

        public static List<GenericLocationData> GetGridD2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d2 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D2", "D", "2"),
            new GridLocationData("D2 - Shrine", "D", "2"),
            new FruitAndLizardLocationData("D2 - Lizard - North", Addresses.LizardD2North, "D", "2", 0),
            new FruitAndLizardLocationData("D2 - Lizard - West", Addresses.LizardD2West, "D", "2", 1),
        };
            return d2;
        }

        public static List<GenericLocationData> GetGridE2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e2 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E2", "E", "2"),
        };
            return e2;
        }

        public static List<GenericLocationData> GetGridF2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f2 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F2", "F", "2"),
            new GridLocationData("F2 - Shrine", "F", "2"),
            new FruitAndLizardLocationData("F2 - Lizard - Center", Addresses.LizardF2Center, "F", "2", 0),
            new FruitAndLizardLocationData("F2 - Lizard - North", Addresses.LizardF2North, "F", "2", 1),
        };
            return f2;
        }

        public static List<GenericLocationData> GetGridG2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g2 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G2", "G", "2"),
            new GridLocationData("G2 - Shrine", "G", "2"),
            new FruitAndLizardLocationData("G2 - Lizard - Center", Addresses.LizardG2Center, "G", "2", 0),
        };
            return g2;
        }

        public static List<GenericLocationData> GetGridB3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> b3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid B3", "B", "3"),
            new FruitAndLizardLocationData("B3 - Lizard - South East", Addresses.LizardB3SouthEast, "B", "3", 1),
        };
            return b3;
        }

        public static List<GenericLocationData> GetGridC3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C3", "C", "3"),
            new GridLocationData("C3 - Shrine", "C", "3"),
            new FruitAndLizardLocationData("C3 - Lizard - South West", Addresses.LizardC3SouthWest, "C", "3", 0),
            new FruitAndLizardLocationData("C3 - Fruit - South", Addresses.FruitC3South, "C", "3", 0),
        };
            return c3;
        }

        public static List<GenericLocationData> GetGridD3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D3", "D", "3"),
        };
            return d3;
        }

        public static List<GenericLocationData> GetGridE3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E3", "E", "3"),
            new GridLocationData("E3 - Shrine North", "E", "3"),
            new GridLocationData("E3 - Shrine South", "E", "3"),
            new FruitAndLizardLocationData("E3 - Lizard - South East 1", Addresses.LizardE3SouthEast1, "E", "3", 0),
            new FruitAndLizardLocationData("E3 - Lizard - South East 2", Addresses.LizardE3SouthEast2, "E", "3", 1),
            new FruitAndLizardLocationData("E3 - Lizard - North West", Addresses.LizardE3NorthWest, "E", "3", 2),
            new FruitAndLizardLocationData("E3 - Fruit - North East 1", Addresses.FruitE3NorthEast1, "E", "3", 0),
            new FruitAndLizardLocationData("E3 - Fruit - North East 2", Addresses.FruitE3NorthEast2, "E", "3", 1),
        };
            return e3;
        }

        public static List<GenericLocationData> GetGridF3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F3", "F", "3"),
        };
            return f3;
        }

        public static List<GenericLocationData> GetGridG3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g3 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G3", "G", "3"),
            new GridLocationData("G3 - Shrine", "G", "3"),
            new FruitAndLizardLocationData("G3 - Lizard - North West", Addresses.LizardG3NorthWest, "G", "3", 0),
        };
            return g3;
        }

        public static List<GenericLocationData> GetGridA4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> a4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid A4", "A", "4"),
        };
            return a4;
        }

        public static List<GenericLocationData> GetGridB4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> b4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid B4", "B", "4"),
            new GridLocationData("B4 - Shrine North", "B", "4"),
            new GridLocationData("B4 - Shrine South", "B", "4"),
            new FruitAndLizardLocationData("B4 - Lizard - North East", Addresses.LizardB4NorthEast, "B", "4", 0),
            new FruitAndLizardLocationData("B4 - Lizard - South West", Addresses.LizardB4SouthWest, "B", "4", 1),
        };
            return b4;
        }

        public static List<GenericLocationData> GetGridC4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C4", "C", "4"),
            new GridLocationData("C4 - Shrine", "C", "4"),
            new FruitAndLizardLocationData("C4 - Lizard - Center", Addresses.LizardC4Center, "C", "4", 0),
            new FruitAndLizardLocationData("C4 - Lizard - North East", Addresses.LizardC4NorthEast, "C", "4", 1),
            new FruitAndLizardLocationData("C4 - Lizard - North West", Addresses.LizardC4NorthWest, "C", "4", 2),
            new FruitAndLizardLocationData("C4 - Fruit - Center 1", Addresses.FruitC4Center1, "C", "4", 0),
            new FruitAndLizardLocationData("C4 - Fruit - Center 2", Addresses.FruitC4Center2, "C", "4", 0),
        };
            return c4;
        }

        public static List<GenericLocationData> GetGridD4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D4", "D", "4"),
            new FruitAndLizardLocationData("D4 - Lizard - West", Addresses.LizardD4West, "D", "4", 0),
            new FruitAndLizardLocationData("D4 - Lizard - North West", Addresses.LizardD4NorthWest, "D", "4", 1),
            new FruitAndLizardLocationData("C4 - Fruit - North West 1", Addresses.FruitC4NorthWest1, "D", "4", 0),
            new FruitAndLizardLocationData("C4 - Fruit - North West 2", Addresses.FruitC4NorthWest2, "D", "4", 1),
        };
            return d4;
        }

        public static List<GenericLocationData> GetGridE4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E4", "E", "4"),
            new GridLocationData("E4 - Shrine", "E", "4"),
            new FruitAndLizardLocationData("E4 - Lizard - Center", Addresses.LizardE4Center,  "E", "4", 0),
            new FruitAndLizardLocationData("E4 - Lizard - West", Addresses.LizardE4West, "E", "4", 1),
        };
            return e4;
        }

        public static List<GenericLocationData> GetGridF4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F4", "F", "4"),
            new GridLocationData("F4 - Shrine", "F", "4"),
            new FruitAndLizardLocationData("F4 - Lizard - Center", Addresses.LizardF4Center, "F", "4", 0),
            new FruitAndLizardLocationData("F4 - Lizard - South West", Addresses.LizardF4SouthWest, "F", "4", 1),
        };
            return f4;
        }

        public static List<GenericLocationData> GetGridG4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G4", "G", "4"),
            new GridLocationData("G4 - Shrine", "G", "4"),
            new FruitAndLizardLocationData("G4 - Lizard - Center", Addresses.LizardG4Center, "G", "4", 0),
            new FruitAndLizardLocationData("G4 - Fruit - North East", Addresses.FruitG4NorthEast, "G", "4", 0),
        };
            return g4;
        }

        public static List<GenericLocationData> GetGridH4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> h4 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid H4", "H", "4"),
        };
            return h4;
        }

        public static List<GenericLocationData> GetGridA5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> a5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid A5", "A", "5"),
        };
            return a5;
        }

        public static List<GenericLocationData> GetGridB5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> b5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid B5", "B", "5"),
            new GridLocationData("B5 - Shrine", "B", "5"),
            new FruitAndLizardLocationData("B5 - Lizard - Center", Addresses.LizardB5Center, "B", "5", 0),
            new FruitAndLizardLocationData("B5 - Fruit - East 1", Addresses.FruitB5East1, "B", "5", 0),
            new FruitAndLizardLocationData("B5 - Fruit - East 2", Addresses.FruitB5East2, "B", "5", 1),
        };
            return b5;
        }

        public static List<GenericLocationData> GetGridC5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C5", "C", "5"),
            new GridLocationData("C5 - Shrine", "C", "5"),
            new FruitAndLizardLocationData("C5 - Lizard - South East", Addresses.LizardC5SouthEast, "C", "5", 0),
            new FruitAndLizardLocationData("C5 - Fruit - East", Addresses.FruitC5East, "C", "5", 0),
        };
            return c5;
        }

        public static List<GenericLocationData> GetGridD5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D5", "D", "5"),
            new GridLocationData("D5 - Shrine", "D", "5"),
            new FruitAndLizardLocationData("D5 - Lizard - Center", Addresses.LizardD5Center, "D", "5", 0),
            new FruitAndLizardLocationData("D5 - Lizard - East", Addresses.LizardD5East, "D", "5", 1),
        };
            return d5;
        }

        public static List<GenericLocationData> GetGridE5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E5", "E", "5"),
            new GridLocationData("E5 - Shrine", "E", "5"),
            new FruitAndLizardLocationData("E5 - Lizard - Center", Addresses.LizardE5Center, "E", "5", 0),
            new FruitAndLizardLocationData("E5 - Lizard - North East", Addresses.LizardE5NorthEast, "E", "5", 1),
            new FruitAndLizardLocationData("E5 - Fruit - Center 1", Addresses.FruitE5Center1, "E", "5", 0),
            new FruitAndLizardLocationData("E5 - Fruit - Center 2", Addresses.FruitE5Center2, "E", "5", 1),
            new FruitAndLizardLocationData("E5 - Fruit - East", Addresses.FruitE5East, "E", "5", 2),
        };
            return e5;
        }

        public static List<GenericLocationData> GetGridF5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F5", "F", "5"),
            new FruitAndLizardLocationData("E5 - Fruit - North", Addresses.FruitE5North, "F", "5", 0),
        };
            return f5;
        }

        public static List<GenericLocationData> GetGridG5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g5 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G5", "G", "5"),
        };
            return g5;
        }

        public static List<GenericLocationData> GetGridC6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> c6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid C6", "C", "6"),
            new FruitAndLizardLocationData("C6 - Lizard - North East", Addresses.LizardC6NorthEast, "C", "6", 0),
        };
            return c6;
        }

        public static List<GenericLocationData> GetGridD6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D6", "D", "6"),
            new FruitAndLizardLocationData("D6 - Lizard - North", Addresses.LizardD6North, "D", "6", 0),
            new FruitAndLizardLocationData("D6 - Lizard - North East", Addresses.LizardD6NorthEast,  "D", "6", 1),
            new FruitAndLizardLocationData("D6 - Fruit - North East 1", Addresses.FruitD6NorthEast1, "D", "6", 0),
            new FruitAndLizardLocationData("D6 - Fruit - North East 2", Addresses.FruitD6NorthEast2, "D", "6", 1),
        };
            return d6;
        }

        public static List<GenericLocationData> GetGridE6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E6", "E", "6"),
        };
            return e6;
        }

        public static List<GenericLocationData> GetGridF6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F6", "F", "6"),
            new FruitAndLizardLocationData("F6 - Lizard - South East", Addresses.LizardF6SouthEast, "F", "6", 0),
            new FruitAndLizardLocationData("F6 - Fruit - South East 1", Addresses.FruitF6SouthEast1, "F", "6", 1),
            new FruitAndLizardLocationData("F6 - Fruit - South East 2", Addresses.FruitF6SouthEast2, "F", "6", 2),
        };
            return f6;
        }

        public static List<GenericLocationData> GetGridG6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G6", "G", "6"),
            new GridLocationData("G6 - Shrine", "G", "6"),
            new FruitAndLizardLocationData("G6 - Lizard - North West", Addresses.LizardG6NorthWest, "G", "6", 0),
            new FruitAndLizardLocationData("G6 - Fruit - East", Addresses.FruitG6East, "G", "6", 0),
        };
            return g6;
        }

        public static List<GenericLocationData> GetGridH6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> h6 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid H6", "H", "6"),
            new GridLocationData("H6 - Shrine", "H", "6"),
            new FruitAndLizardLocationData("H6 - Lizard - Center 1", Addresses.LizardH6Center1, "H", "6", 0),
            new FruitAndLizardLocationData("H6 - Lizard - Center 2", Addresses.LizardH6Center2, "H", "6", 1),
            new FruitAndLizardLocationData("H6 - Lizard - North East", Addresses.LizardH6NorthEast, "H", "6", 2),
            new FruitAndLizardLocationData("H6 - Lizard - South", Addresses.LizardH6South, "H", "6", 3),
        };
            return h6;
        }

        public static List<GenericLocationData> GetGridD7Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> d7 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid D7", "D", "7"),
            new GridLocationData("D7 - Shrine", "D", "7"),
            new FruitAndLizardLocationData("D7 - Lizard - Center", Addresses.LizardD7Center, "D", "7", 0),
            new FruitAndLizardLocationData("D7 - Lizard - North", Addresses.LizardD7North, "D", "7", 1),
            new FruitAndLizardLocationData("D7 - Fruit - North East 1", Addresses.FruitD7NorthEast1, "D", "7", 0),
            new FruitAndLizardLocationData("D7 - Fruit - North East 2", Addresses.FruitD7NorthEast2, "D", "7", 1),
        };
            return d7;
        }

        public static List<GenericLocationData> GetGridE7Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e7 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E7", "E", "7"),
            new FruitAndLizardLocationData("E7 - Lizard - Center", Addresses.LizardE7Center, "E", "7", 0),
            new FruitAndLizardLocationData("E7 - Lizard - East", Addresses.LizardE7East, "E", "7", 1),
            new FruitAndLizardLocationData("E7 - Lizard - West", Addresses.LizardE7West, "E", "7", 2),
            new FruitAndLizardLocationData("E7 - Fruit - West 1", Addresses.FruitE7West1, "E", "7", 0),
            new FruitAndLizardLocationData("E7 - Fruit - West 2", Addresses.FruitE7West2, "E", "7", 1),
        };
            return e7;
        }

        public static List<GenericLocationData> GetGridF7Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f7 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F7", "F", "7"),
            new GridLocationData("F7 - Shrine", "F", "7"),
            new FruitAndLizardLocationData("F7 - Lizard - Center 1", Addresses.LizardF7Center1, "F", "7", 0),
            new FruitAndLizardLocationData("F7 - Lizard - Center 2", Addresses.LizardF7Center2, "F", "7", 1),
            new FruitAndLizardLocationData("F7 - Lizard - North", Addresses.LizardF7North, "F", "7", 2),
            new FruitAndLizardLocationData("F7 - Lizard - North East", Addresses.LizardF7NorthEast, "F", "7", 3),
            new FruitAndLizardLocationData("F7 - Lizard - South West", Addresses.LizardF7SouthWest, "F", "7", 4),
            new FruitAndLizardLocationData("F7 - Lizard - North West", Addresses.LizardF7NorthWest, "F", "7", 5),
        };
            return f7;
        }

        public static List<GenericLocationData> GetGridG7Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g7 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G7", "G", "7"),
            new FruitAndLizardLocationData("G7 - Lizard - Center", Addresses.LizardG7Center, "G", "7", 0),
            new FruitAndLizardLocationData("G7 - Lizard - North", Addresses.LizardG7North, "G", "7", 1),
            new FruitAndLizardLocationData("G7 - Lizard - North East", Addresses.LizardG7NorthEast, "G", "7", 2),
            new FruitAndLizardLocationData("G7 - Lizard - South East", Addresses.LizardG7SouthEast, "G", "7", 3),
            new FruitAndLizardLocationData("G7 - Lizard - North West", Addresses.LizardG7NorthWest, "G", "7", 4),
        };
            return g7;
        }

        public static List<GenericLocationData> GetGridH7Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> h7 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid H7", "H", "7"),
            new FruitAndLizardLocationData("H7 - Lizard - West", Addresses.LizardH7West, "H", "7", 0),
            new FruitAndLizardLocationData("H7 - Lizard - North West", Addresses.LizardH7NorthWest, "H", "7", 1),
        };
            return h7;
        }

        public static List<GenericLocationData> GetGridE8Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> e8 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid E8", "E", "8"),
            new FruitAndLizardLocationData("E8 - Lizard - North", Addresses.LizardE8North, "E", "8", 0),
        };
            return e8;
        }

        public static List<GenericLocationData> GetGridF8Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> f8 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid F8", "F", "8"),
            new GridLocationData("F8 - Shrine", "F", "8"),
            new FruitAndLizardLocationData("F8 - Lizard - Center", Addresses.LizardF8Center, "F", "8", 0),
        };
            return f8;
        }

        public static List<GenericLocationData> GetGridG8Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> g8 = new List<GenericLocationData>() {
            new GridLocationData("Map Grid G8", "G", "8"),
            new GridLocationData("G8 - Shrine", "G", "8"),
            new FruitAndLizardLocationData("G8 - Lizard - West", Addresses.LizardG8West, "G", "8", 0),
            new FruitAndLizardLocationData("G8 - Fruit - North East 1", Addresses.FruitG8NorthEast1, "G", "8", 0),
            new FruitAndLizardLocationData("G8 - Fruit - North East 2", Addresses.FruitG8NorthEast2, "G", "8", 1),
        };
            return g8;
        }

        //  BOSSES

        public static List<GenericLocationData> GetBossD1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossD1 = new List<GenericLocationData>() {
            new BossLocationData("Eel Kill - Col. 7", Addresses.BossCol7EelKill, "D", "1"),
        };
            return bossD1;
        }

        public static List<GenericLocationData> GetBossF1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossF1 = new List<GenericLocationData>() {
            new BossLocationData("Leo Kill - Col. 11", Addresses.BossCol11LeoKill, "F", "1"),
        };
            return bossF1;
        }

        public static List<GenericLocationData> GetBossG1Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossG1 = new List<GenericLocationData>() {
            new BossLocationData("Minotaur C Kill - Col. 15", Addresses.BossCol15MinotaurCKill, "G", "1"),
        };
            return bossG1;
        }

        public static List<GenericLocationData> GetBossC2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossC2 = new List<GenericLocationData>() {
            new BossLocationData("Cerberus Kill - Col. 14", Addresses.BossCol14CerberusKill, "C","2"),
        };
            return bossC2;
        }

        public static List<GenericLocationData> GetBossE2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossE2 = new List<GenericLocationData>() {
            new BossLocationData("Knight Kill - Col. 3", Addresses.BossCol3KnightKill, "E", "2"),
        };
            return bossE2;
        }

        public static List<GenericLocationData> GetBossG2Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossG2 = new List<GenericLocationData>() {
            new BossLocationData("Poseidon Kill - Col. 12", Addresses.BossCol12PoseidonKill, "G","2"),
        };
            return bossG2;
        }

        public static List<GenericLocationData> GetBossD3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossD3 = new List<GenericLocationData>() {
            new BossLocationData("Kame Kill - Col. 9", Addresses.BossCol9KameKill, "D", "3"),
        };
            return bossD3;
        }

        public static List<GenericLocationData> GetBossF3Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossF3 = new List<GenericLocationData>() {
            new BossLocationData("Mammoth Kill - Col. 2", Addresses.BossCol2MammothKill, "F", "3" ),
        };
            return bossF3;
        }

        public static List<GenericLocationData> GetBossB4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossB4 = new List<GenericLocationData>() {
            new BossLocationData("Narga Kill - Col. 10", Addresses.BossCol10NargaKill, "B", "4"),
        };
            return bossB4;
        }

        public static List<GenericLocationData> GetBossH4Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossH4 = new List<GenericLocationData>() {
            new BossLocationData("Bird Kill - Col. 5", Addresses.BossCol5BirdKill, "H","4"),
        };
            return bossH4;
        }

        public static List<GenericLocationData> GetBossF5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossF5 = new List<GenericLocationData>() {
            new BossLocationData("Mintaur A Kill - Col. 1", Addresses.BossCol1MintaurAKill, "F", "5"),
        };
            return bossF5;
        }

        public static List<GenericLocationData> GetBossG5Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossG5 = new List<GenericLocationData>() {
            new BossLocationData("Kirin Kill - Col. 4", Addresses.BossCol4KirinKill, "G", "5"),
        };
            return bossG5;
        }

        public static List<GenericLocationData> GetBossD6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossD6 = new List<GenericLocationData>() {
            new BossLocationData("Minotaur B Kill - Col. 6", Addresses.BossCol6MinotaurBKill, "D", "6"),
        };
            return bossD6;
        }

        public static List<GenericLocationData> GetBossE6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossE6 = new List<GenericLocationData>() {
            new BossLocationData("Snake Kill - Col. 13", Addresses.BossCol13SnakeKill, "E","6"),
        };
            return bossE6;
        }

        public static List<GenericLocationData> GetBossG6Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossG6 = new List<GenericLocationData>() {
            new BossLocationData("Yamori B Kill - Col. 8", Addresses.BossCol8YamoriBKill, "E", "6"),
        };
            return bossG6;
        }

        public static List<GenericLocationData> GetBossF8Data(Dictionary<string, object> options)
        {
            List<GenericLocationData> bossF8 = new List<GenericLocationData>() {
            new BossLocationData("Evis Kill - Col. 16", Addresses.BossCol16EvisKill, "G","6"),
        };
            return bossF8;
        }
    }
}