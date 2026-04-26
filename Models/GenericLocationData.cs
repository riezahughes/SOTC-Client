using Archipelago.Core.Models;

namespace SotcArchipelago.Models
{
    public class GenericLocationData
    {
        public string Name { get; set; }

        public uint Address { get; set; }

        public string Check { get; set; }

        public LocationCheckType CheckType { get; set; }

        public uint RipperShiftAddress { get; set; }

        public GenericLocationData(string name, uint locationAddress, string check, LocationCheckType checkType)
        {
            Name = name;
            Address = locationAddress;
            Check = check;
            CheckType = checkType;
        }
    }
}
