using Domain.Common;

namespace Domain.UserInput
{
    public class MapField : Field
    {
        // Added by NG
        // Modified by EKT & NVZ
        public Image PinImage { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public bool Searchable { get; set; }
        
    }
}