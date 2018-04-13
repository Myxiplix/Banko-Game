namespace BingoTest
{
    class BingoTicket
    {
        // FirstRowId OR (SecondRowId << 8) OR (ThirdRowId << 16)
        private uint combinedTicketsId = 0;
        private BingoRow firstBingoRow = null;
        private BingoRow secondBingoRow = null;
        private BingoRow thirdBingoRow = null;

        public uint TicketId
        {
            get
            {
                return combinedTicketsId;
            }
            set
            {
                combinedTicketsId = value;
            }
        }

        public BingoRow FirstBingoRow
        {
            get
            {
                return firstBingoRow;
            }
            set
            {
                firstBingoRow = value;
            }
        }

        public BingoRow SecondBingoRow
        {
            get
            {
                return secondBingoRow;
            }
            set
            {
                secondBingoRow = value;
            }
        }

        public BingoRow ThirdBingoRow
        {
            get
            {
                return thirdBingoRow;
            }
            set
            {
                thirdBingoRow = value;
            }
        }
    }
}
