namespace Refterm
{
    public class Line
    {
        public int FirstP;
        public int OnePastLastP;
        public bool ContainsComplexChars;
        public GlyphProps StartingProps = new GlyphProps();

        public void Clear()
        {
            ContainsComplexChars = false;
            FirstP = 0;
            OnePastLastP = 0;
            StartingProps.Background = 0;
            StartingProps.Flags = 0;
            StartingProps.Foreground = 0;
        }
    }
}
