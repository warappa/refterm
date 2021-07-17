namespace Refterm
{
    public class TerminalBuffer
    {
        private readonly Terminal terminal;
        public RendererCell[] Cells;
        public uint DimX;
        public uint DimY;
        public uint FirstLineY;

        public TerminalBuffer(Terminal terminal)
        {
            this.terminal = terminal;
        }

        public bool IsInBounds(Position Point)
        {
            var Result = (Point.X >= 0) && (Point.X < (int)DimX) &&
                          (Point.Y >= 0) && (Point.Y < (int)DimY);
            return Result;
        }

        public void Clear()
        {
            ClearCellCount(0, DimX * DimY);
        }

        public void ClearLine(uint Y)
        {
            var Point = new Position(0, Y);
            if (IsInBounds(Point))
            {
                ClearCellCount(GetCellIndex(Point), DimX);
            }
        }

        public uint GetCellIndex(Position point)
        {
            if (!IsInBounds(point))
            {
                return 0;
            }

            return point.X + point.Y * DimX;
        }

        public void ClearCellCount(uint offset, uint Count)
        {
            uint Background = terminal.DefaultBackgroundColor;
            for (var i = 0; i < Count; i++)
            {
                var cell = Cells[offset + i];
                cell.GlyphIndex = 0;
                cell.Foreground = Background; // TODO(casey): Should be able to set this to 0, but need to make sure cache slot 0 never gets filled
                cell.Background = Background;
            }
        }

        public RendererCell? GetCell(Position point)
        {
            var Result = IsInBounds(point) ? Cells[point.Y * DimX + point.X] : (RendererCell?)null;
            return Result;
        }
    }
}
