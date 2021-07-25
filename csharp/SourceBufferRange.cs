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

        public SourceBufferRange(SourceBufferRange source, int count)
        {
            AbsoluteP = source.AbsoluteP;
            Count = count;
            Data = source.Data.Slice(0, count);
        }

        public ulong AbsoluteP;
        public int Count;
        public Memory<char> Data;

        internal void Skip(int skipCount)
        {
            AbsoluteP += (ulong)skipCount;
            Count = Count - skipCount;
            Data = Data.Slice(skipCount, Count);
        }
    }
}
