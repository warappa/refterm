using System;

namespace Refterm
{
    public class SourceBufferRange
    {
        public SourceBufferRange()
        {

        }

        public SourceBufferRange(SourceBufferRange source)
        {
            AbsoluteP = source.AbsoluteP;
            Count = source.Count;
            Data = source.Data;
        }

        public ulong AbsoluteP;
        public int Count;
        public Memory<char> Data;
    }
}
