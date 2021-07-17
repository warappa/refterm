namespace Refterm
{
    public class Partitioner
    {
        // TODO(casey): Get rid of Uniscribe so this garbage doesn't have to happen

        public Uniscribe.SCRIPT_DIGITSUBSTITUTE UniscribeDigitSubstitution = new Uniscribe.SCRIPT_DIGITSUBSTITUTE();
        public Uniscribe.SCRIPT_CONTROL UniControl = new Uniscribe.SCRIPT_CONTROL();
        public Uniscribe.SCRIPT_STATE UniState = new Uniscribe.SCRIPT_STATE();
        //public Uniscribe.SCRIPT_CACHE UniCache;

        public char[] Expansion = new char[1024];
        public Uniscribe.SCRIPT_ITEM[] Items = new Uniscribe.SCRIPT_ITEM[1024];
        public Uniscribe.SCRIPT_LOGATTR[] Log = new Uniscribe.SCRIPT_LOGATTR[1024];
        public uint[] SegP = new uint[1026];
    }
}
