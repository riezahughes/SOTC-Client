using Archipelago.Core.Models;

namespace SotcArchipelago.Models
{
    public class FruitAndLizardLocationData : GenericLocationData
    {
        public string Name { get; set; }

        public uint Address { get; set; }

        public string GridLetter { get; set; }

        public string GridNumber { get; set; }

        public int BitsToChange { get; set; }

        public LocationCheckType CheckType { get; set; }

        public FruitAndLizardLocationData(string name, uint locationAddress, string gridLetter, string gridNumber, int bitsToChange) : base(name, 0, string.Empty, default)
        {
            Name = name;
            Address = locationAddress;
            GridLetter = gridLetter;
            GridNumber = gridNumber;
            BitsToChange = bitsToChange;
        }
    }
}
