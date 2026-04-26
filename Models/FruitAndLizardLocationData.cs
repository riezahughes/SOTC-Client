using Archipelago.Core.Models;

namespace SotcArchipelago.Models
{
    public class FruitAndLizardLocationData : GenericLocationData
    {
        public string GridLetter { get; set; }

        public string GridNumber { get; set; }

        public int BitsToChange { get; set; }

        public FruitAndLizardLocationData(string name, uint locationAddress, string gridLetter, string gridNumber, int bitsToChange) : base(name, locationAddress, string.Empty, default)
        {
            GridLetter = gridLetter;
            GridNumber = gridNumber;
            BitsToChange = bitsToChange;
        }
    }
}
