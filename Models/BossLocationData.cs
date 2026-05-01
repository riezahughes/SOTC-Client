namespace SotcArchipelago.Models
{
    public class BossLocationData : GenericLocationData
    {
        public string GridLetter { get; set; }

        public string GridNumber { get; set; }

        public BossLocationData(string name, uint locationAddress, string gridLetter, string gridNumber) : base(name, locationAddress, string.Empty, default)
        {
            GridLetter = gridLetter;
            GridNumber = gridNumber;
        }
    }
}
