namespace SotcArchipelago
{
    class Addresses
    {
        // mark all your addresses here for easy access. Then you can just use Addresses.test, as an example.
        public const uint test = 0x000123123;

        public const uint ItemIndexStorage = 0x00ABCDEF; // This is the address where we will store the current item index

        public const uint InGameCheck = 0x012d9bc0;
        public const uint SaveDataPopup = 0x012DA4F0;
        public const uint IsFightingColossi = 0x012de128; // Whenever it's NOT 0x00

        public const uint NumberOfColossiKilled = 0x012de128;

        public const uint KillPlayer = 0x012D9BCF; // Set to FF

        public const uint GridMapLetter = 0x01466420;
        /*
            0x42 = Column B
            0x43 = Column C
            0x44 = Column D
            0x45 = Column E
            0x46 = Column F
            0x47 = Column G
            0x48 = Column H
            0x49 = Column J
         */
        public const uint GridMapNumber = 0x0146643c;
        /*
            0x01 = Row 1
            0x02 = Row 2
            0x03 = Row 3
            0x04 = Row 4
            0x05 = Row 5
            0x06 = Row 6
            0x07 = Row 7
            0x08 = Row 8
         */

        public const uint GridMapFull = 0x0166ff60; // Top Left to Bottom Right Grid Count

        public const uint CoordinatesY = 0x01072090;
        public const uint CoordinatesZ = 0x01072094;
        public const uint CoordinatesX = 0x01072098;

        public const uint MaxHealth = 0x012d9bcc; // This number is +4 every colossus
        public const uint MaxStamina = 0x012d9bd0; // This number is +4 every colossus


        // Shrine Addresses
        public const uint ShrineSaving = 0x000000;

        // Lizard Addresses
        public const uint LizardC1Center = 0x12d9df8; // bit 0
        public const uint LizardE1West = 0x12d9e08; // bit 0
        public const uint LizardF1Center = 0x12d9e10; // bit 0
        public const uint LizardG1Center = 0x12d9e18; // bit 0
        public const uint LizardG1West = 0x12d9e18; // bit 1
        public const uint LizardD2North = 0x12d9e50; // bit 0
        public const uint LizardD2West = 0x12d9e50; // bit 1
        public const uint LizardF2Center = 0x12d9e60; // bit 0
        public const uint LizardF2North = 0x12d9e60; // bit 1
        public const uint LizardG2Center = 0x12d9e68; // bit 0
        public const uint LizardB3SouthEast = 0x12d9e90; // bit 0
        public const uint LizardC3SouthWest = 0x12d9e98; // bit 0
        public const uint LizardE3SouthEast1 = 0x12d9ea8; // bit 0
        public const uint LizardE3SouthEast2 = 0x12d9ea8; // bit 1
        public const uint LizardE3NorthWest = 0x12d9ea8; // bit 2
        public const uint LizardG3NorthWest = 0x12d9eb8; // bits 0-2
        public const uint LizardB4NorthEast = 0x12d9ee0; // bit 0
        public const uint LizardB4SouthWest = 0x12d9ee0; // bits 1-3
        public const uint LizardC4Center = 0x12d9ee8; // bit 0
        public const uint LizardC4NorthEast = 0x12d9ee8; // bit 1
        public const uint LizardC4NorthWest = 0x12d9ee8; // bit 2
        public const uint LizardD4West = 0x12d9ef0; // bit 0
        public const uint LizardD4NorthWest = 0x12d9ef0; // bit 1
        public const uint LizardE4Center = 0x12d9ef8; // bit 0
        public const uint LizardE4West = 0x12d9ef8; // bit 1
        public const uint LizardF4SouthWest = 0x12d9f00; // bit 0
        public const uint LizardF4Center = 0x12d9f00; // bit 1
        public const uint LizardG4Center = 0x12d9f08; // bit 0
        public const uint LizardB5Center = 0x12d9f30; // bits 0-2
        public const uint LizardC5SouthEast = 0x12d9f38; // bits 0-2
        public const uint LizardD5Center = 0x12d9f40; // bit 0
        public const uint LizardD5East = 0x12d9f40; // bit 1
        public const uint LizardE5Center = 0x12d9f48; // bits 0-2
        public const uint LizardE5NorthEast = 0x12d9f48; // bit 3
        public const uint LizardC6NorthEast = 0x12d9f88; // bit 0
        public const uint LizardD6North = 0x12d9f90; // bit 0
        public const uint LizardD6NorthEast = 0x12d9f90; // bit 1
        public const uint LizardF6SouthEast = 0x12d9fa0; // bit 0
        public const uint LizardG6NorthWest = 0x12d9fa8; // bit 0
        public const uint LizardH6Center1 = 0x12d9fb0; // bit 0
        public const uint LizardH6Center2 = 0x12d9fb0; // bit 1
        public const uint LizardH6NorthEast = 0x12d9fb0; // bit 2
        public const uint LizardH6South = 0x12d9fb0; // bit 3
        public const uint LizardD7Center = 0x12d9fe0; // bit 0
        public const uint LizardD7North = 0x12d9fe0; // bit 1
        public const uint LizardE7East = 0x12d9fe8; // bit 0
        public const uint LizardE7Center = 0x12d9fe8; // bit 1
        public const uint LizardE7West = 0x12d9fe8; // bit 2
        public const uint LizardF7Center1 = 0x12d9ff0; // bit 0
        public const uint LizardF7Center2 = 0x12d9ff0; // bit 1
        public const uint LizardF7North = 0x12d9ff0; // bit 2
        public const uint LizardF7NorthEast = 0x12d9ff0; // bit 3
        public const uint LizardF7SouthWest = 0x12d9ff0; // bit 4
        public const uint LizardF7NorthWest = 0x12d9ff0; // bit 5
        public const uint LizardG7Center = 0x12d9ff8; // bit 0
        public const uint LizardG7North = 0x12d9ff8; // bit 1
        public const uint LizardG7NorthEast = 0x12d9ff8; // bit 2
        public const uint LizardG7SouthEast = 0x12d9ff8; // bit 3
        public const uint LizardG7NorthWest = 0x12d9ff8; // bit 3
        public const uint LizardH7West = 0x12da000; // bit 0
        public const uint LizardH7NorthWest = 0x12da000; // bit 1
        public const uint LizardE8North = 0x12da038; // bit 0
        public const uint LizardF8Center = 0x12da040; // bits 0-2
        public const uint LizardG8West = 0x12da048; // bit 0

        // Fruit Addresses
        public const uint FruitB5East1 = 0x12d9c10; // bits 0-2
        public const uint FruitB5East2 = 0x12d9c14; // bits 0-2
        public const uint FruitC1South1 = 0x12d9c18; // bits 0-2
        public const uint FruitC1South2 = 0x12d9c1c; // bits 0-3
        public const uint FruitC3South = 0x12d9c20; // bits 0-3
        public const uint FruitC4Center1 = 0x12d9c24; // bits 0-2
        public const uint FruitC4Center2 = 0x12d9c28; // bits 0-2
        public const uint FruitC5East = 0x12d9c2c; // bits 0-1
        public const uint FruitC4NorthWest1 = 0x12d9c38; // bits 0-5
        public const uint FruitC4NorthWest2 = 0x12d9c3c; // bits 0-2
        public const uint FruitC6NorthWest1 = 0x12d9c30; // bits 0-2
        public const uint FruitC6NorthWest2 = 0x12d9c34; // bits 0-2
        public const uint FruitD6NorthEast1 = 0x12d9c40; // bits 0-2
        public const uint FruitD6NorthEast2 = 0x12d9c44; // bits 0-2
        public const uint FruitD7NorthEast1 = 0x12d9c48; // bits 0-7
        public const uint FruitD7NorthEast2 = 0x12d9c4c; // bit 0
        public const uint FruitE3NorthEast1 = 0x12d9c50; // bits 0-3
        public const uint FruitE3NorthEast2 = 0x12d9c54; // bits 0-3
        public const uint FruitE5Center1 = 0x12d9c58; // bits 0-1
        public const uint FruitE5Center2 = 0x12d9c5c; // bits 0-1
        public const uint FruitE5East = 0x12d9c60; // bits 0-5
        public const uint FruitE7West1 = 0x12d9c64; // bits 0-4
        public const uint FruitE7West2 = 0x12d9c68; // bits 0-4
        public const uint FruitF1SouthWest = 0x12d9c6c; // bits 0-3
        public const uint FruitE5North = 0x12d9c70; // bit 0
        public const uint FruitF6SouthEast1 = 0x12d9c74; // bits 0-3
        public const uint FruitF6SouthEast2 = 0x12d9c78; // bits 0-2
        public const uint FruitG4NorthEast = 0x12d9c7c; // bits 0-7
        public const uint FruitG6East = 0x12d9c80; // bits 0-5
        public const uint FruitG8NorthEast1 = 0x12d9c84; // bits 0-3
        public const uint FruitG8NorthEast2 = 0x12d9c88; // bits 0-5

        // boss kill addresses

        public const uint BossCol1MintaurAKill = 0x00;
        public const uint BossCol2MammothKill = 0x00;
        public const uint BossCol3KnightKill = 0x00;
        public const uint BossCol4KirinKill = 0x00;
        public const uint BossCol5BirdKill = 0x00;
        public const uint BossCol6MinotaurBKill = 0x00;
        public const uint BossCol7EelKill = 0x00;
        public const uint BossCol8YamoriBKill = 0x00;
        public const uint BossCol9KameKill = 0x00;
        public const uint BossCol10NargaKill = 0x00;
        public const uint BossCol11LeoKill = 0x00;
        public const uint BossCol12PoseidonKill = 0x00;
        public const uint BossCol13SnakeKill = 0x00;
        public const uint BossCol14CerberusKill = 0x00;
        public const uint BossCol15MinotaurCKill = 0x00;
        public const uint BossCol16EvisKill = 0x00;
    }
}