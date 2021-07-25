namespace Refterm
{
    public class Partitioner
    {
        // TODO(casey): Get rid of Uniscribe so this garbage doesn't have to happen

        public Uniscribe.SCRIPT_DIGITSUBSTITUTE UniscribeDigitSubstitution = new Uniscribe.SCRIPT_DIGITSUBSTITUTE();
        public Uniscribe.SCRIPT_CONTROL UniControl = new Uniscribe.SCRIPT_CONTROL();
        public Uniscribe.SCRIPT_STATE UniState = new Uniscribe.SCRIPT_STATE();

        public char[] Expansion { get; private set; } = new char[16 * 1024];
        public Uniscribe.SCRIPT_ITEM[] Items { get; private set; } = new Uniscribe.SCRIPT_ITEM[16 * 1024];
        public Uniscribe.SCRIPT_LOGATTR[] Log { get; private set; } = new Uniscribe.SCRIPT_LOGATTR[16 * 1024];
        public uint[] SegP { get; private set; } = new uint[16 * 1024 + 2];
    }
}
