namespace BingoTest
{
    class BingoRow
    {
        private uint bingoRowId = 0;
        private bool[] bingoRowPattern = null;

        public BingoRow(uint theRowId, bool[] theRowPattern)
        {
            this.bingoRowId = theRowId;
            this.bingoRowPattern = theRowPattern;
        }

        public uint BingoRowId
        {
            get
            {
                return bingoRowId;
            }
        }

        public bool[] BingoRowPattern
        {
            get
            {
                return bingoRowPattern;
            }
        }
    }
}
