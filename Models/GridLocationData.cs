namespace SotcArchipelago.Models
{
    public class GridLocationData : GenericLocationData
    {
        public string Name { get; set; }
        public string GridLetter { get; set; }
        public string GridNumber { get; set; }

        public GridLocationData(string name, string gridLetter, string gridNumber) : base(name, 0, string.Empty, default)
        {
            Name = name;
            GridLetter = gridLetter;
            GridNumber = gridNumber;
        }
    }
}
