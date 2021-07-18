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

        public int AbsoluteP;
        public int Count;
        public Memory<char> Data;
    }
}
