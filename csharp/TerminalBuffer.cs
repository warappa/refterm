using System;

namespace Refterm
{
    public class TerminalBuffer : IDisposable
    {
        private readonly Terminal terminal;

        public RendererCell[] Cells { get; private set; }
        public uint DimX { get; set; }
        public uint DimY { get; set; }
        public uint FirstLineY { get; set; }

        public TerminalBuffer(Terminal terminal, uint dimX, uint dimY)
        {
            this.terminal = terminal;
            DimX = dimX;
            DimY = dimY;
            Cells = new RendererCell[DimX * DimY];
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
            var Background = terminal.DefaultBackgroundColor;
            for (var i = 0; i < Count; i++)
            {
                ref var cell = ref Cells[offset + i];
                cell.GlyphIndex = 0;
                cell.Foreground = Background; // TODO(casey): Should be able to set this to 0, but need to make sure cache slot 0 never gets filled
                cell.Background = Background;
            }
        }

        public static RendererCell Uninitialized = new();
        public ref RendererCell GetCell(Position point)
        {
            if (IsInBounds(point))
            {
                return ref Cells[point.Y * DimX + point.X];
            }

            return ref Uninitialized;
        }

        public void UpdateCell(Position point, RendererCell cell)
        {
            Cells[point.Y * DimX + point.X] = cell;
        }

        public void Dispose()
        {
            if (Cells is not null)
            {
                DimX = DimY = 0;
                Cells = null;
            }
        }
    }
}
