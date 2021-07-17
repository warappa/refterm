namespace Refterm
{
    public class Position
    {
        public Position() { }
        public Position(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public uint X { get; set; }
        public uint Y { get; set; }
    }
}
