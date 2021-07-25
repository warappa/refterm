namespace Refterm
{
    public class Line
    {
        public ulong FirstP { get; set; }
        public ulong OnePastLastP { get; set; }
        public bool ContainsComplexChars { get; set; }
        public GlyphProps StartingProps { get; set; } = new GlyphProps();

        public void Clear(Terminal terminal)
        {
            ContainsComplexChars = false;
            FirstP = 0;
            OnePastLastP = 0;
            StartingProps.Background = terminal.DefaultBackgroundColor;
            StartingProps.Flags = 0;
            StartingProps.Foreground = terminal.DefaultForegroundColor;
        }
    }
}
