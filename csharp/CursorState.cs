namespace Refterm
{
    public class CursorState
    {
        private readonly Terminal terminal;

        public Position Position { get; set; } = new Position();
        public GlyphProps Props { get; internal set; } = new GlyphProps();

        public CursorState(Terminal terminal)
        {
            this.terminal = terminal;

            ClearCursor();
        }

        public void ClearCursor()
        {
            Position = new Position();
            ClearProps();
        }

        internal void ClearProps()
        {
            Props.Foreground = terminal.DefaultForegroundColor;
            Props.Background = terminal.DefaultBackgroundColor;
            Props.Flags = 0;
        }
    }
}
