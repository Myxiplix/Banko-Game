using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BingoTest
{
    class BingoTicketGenerator
    {
        private RNGCryptoServiceProvider rngCryptoProvider = new RNGCryptoServiceProvider();

        private List<bool[]> BingoBoolRowsArray = null;
        private List<BingoRow> BingoRowsList = null;

        public List<BingoTicket> BingoTicketsList = null;
        public HashSet<uint> uniqueBingoTicketIdSet = null;

        private int ContinousBlankSpaceLimit = 2;

        public void BuildBingoTickets()
        {
            CreateValidBingoRows();
            GenerateValidBingoTickets();
        }

        public int BlankSpaceLimit
        {
            get
            {
                return ContinousBlankSpaceLimit;
            }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                if (value > 4)
                {
                    value = 4;
                }
                ContinousBlankSpaceLimit = value;
            }
        }

        public void SaveBingoTicketsToFile()
        {
            if (BingoTicketsList == null)
            {
                // Save generated rows, with default consecutive blank spaces of 2.
                BuildBingoTickets();
            }
            string fileName = "BingoTickets_With_Max_" + BlankSpaceLimit + "_Consecutive_Blank_Spaces.txt";
            using (StreamWriter writetext = new StreamWriter(fileName))
            {
                int index = 1;
                foreach (BingoTicket aBingoTicket in BingoTicketsList)
                {
                    string str1 = "";
                    string str2 = "";
                    string str3 = "";

                    for (int bitPosition = 0; bitPosition < 8; bitPosition++)
                    {
                        str1 += aBingoTicket.FirstBingoRow.BingoRowPattern[bitPosition] ? "1 " : "0 ";
                        str2 += aBingoTicket.SecondBingoRow.BingoRowPattern[bitPosition] ? "1 " : "0 ";
                        str3 += aBingoTicket.ThirdBingoRow.BingoRowPattern[bitPosition] ? "1 " : "0 ";
                    }
                    str1 += aBingoTicket.FirstBingoRow.BingoRowPattern[8] ? "1\n" : "0\n";
                    str2 += aBingoTicket.SecondBingoRow.BingoRowPattern[8] ? "1\n" : "0\n";
                    str3 += aBingoTicket.ThirdBingoRow.BingoRowPattern[8] ? "1\n" : "0\n";
                    writetext.WriteLine(string.Format("Ticket ID:{0:00000000} | Count:{1:000000}\n", aBingoTicket.TicketId, index++) + str1 + str2 + str3);
                }
            }
        }

        private void CreateValidBingoRows()
        {
            BingoBoolRowsArray = new List<bool[]>();
            for (uint bitValue = 0; bitValue < 512; bitValue++)
            {
                bool[] generatedBingoRow = GenerateRowPermutationByNumber(bitValue);
                if (IsBingoRowValid(generatedBingoRow))
                {
                    BingoBoolRowsArray.Add(generatedBingoRow);
                }
            }
        }

        private bool[] GenerateRowPermutationByNumber(uint aBitValue)
        {
            bool[] aGeneratedBingoRow = new bool[9];
            uint generatedBitMask = 1;
            for (int bitPosition = 0; bitPosition < 9; generatedBitMask <<= 1, bitPosition++)
            {
                aGeneratedBingoRow[bitPosition] = (aBitValue & generatedBitMask) != 0;
            }
            return aGeneratedBingoRow;
        }

        // Row must contain 5 numbers and filter out rows based on consecutive blank fields
        private bool IsBingoRowValid(bool[] aGeneratedRow)
        {
            int countBlanks = 1;
            int countHolding = 0;
            // Assume previous field is the opposite of current field, but only
            // if current field is false (blank). We are counting blank fields.
            bool previousField = aGeneratedRow[0] == false ? true : aGeneratedRow[0];

            for (int bitPosition = 0; bitPosition < 9; bitPosition++)
            {
                bool currentField = aGeneratedRow[bitPosition];
                if (currentField == false)
                {
                    if (currentField == previousField && ++countBlanks > BlankSpaceLimit)
                    {
                        return false;
                    }
                }
                else
                {
                    countHolding++;
                    countBlanks = 1;
                }
                previousField = currentField;
            }
            return countHolding == 5;
        }

        // Only combine rows, where the end ticket has atleast 1 number in each column.
        private void GenerateValidBingoTickets()
        {
            BingoRowsList = new List<BingoRow>();
            BingoTicketsList = new List<BingoTicket>();
            uniqueBingoTicketIdSet = new HashSet<uint>();

            uint bingoRowId = 0;

            foreach (bool[] aGeneratedRow in BingoBoolRowsArray)
            {
                BingoRowsList.Add(
                    new BingoRow(++bingoRowId, aGeneratedRow));
            }

            //Shuffle(BingoRowsList);

            foreach (BingoRow bingoRowCopy_1 in BingoRowsList)
            {
                foreach (BingoRow bingoRowCopy_2 in BingoRowsList)
                {
                    foreach (BingoRow bingoRowCopy_3 in BingoRowsList)
                    {
                        if (SafeTriplet(bingoRowCopy_1, bingoRowCopy_2, bingoRowCopy_3))
                        {
                            uint bingoCardId =
                                bingoRowCopy_1.BingoRowId |
                                (bingoRowCopy_2.BingoRowId << 8) |
                                (bingoRowCopy_3.BingoRowId << 16);

                            BingoTicket tmpBingoTicket = new BingoTicket();

                            tmpBingoTicket.FirstBingoRow = bingoRowCopy_1;
                            tmpBingoTicket.SecondBingoRow = bingoRowCopy_2;
                            tmpBingoTicket.ThirdBingoRow = bingoRowCopy_3;
                            tmpBingoTicket.TicketId = bingoCardId;

                            BingoTicketsList.Add(tmpBingoTicket);
                            uniqueBingoTicketIdSet.Add(bingoCardId);
                        }
                    }
                }
            }
            //Console.WriteLine(string.Format("Valid Bingo Tickets Count: {0}", validBingoCards.Count()));
            //Console.WriteLine(string.Format("Unique Bingo Tickets In HashSet: {0}", uniqueBingoCardIdSet.Count()));
        }

        // Bingo ticket must not contain a blank column across rows.
        private bool SafeTriplet(BingoRow aRow, BingoRow bRow, BingoRow cRow)
        {
            for (int bitPosition = 0; bitPosition < 9; bitPosition++)
            {
                if (!aRow.BingoRowPattern[bitPosition] &&
                    !bRow.BingoRowPattern[bitPosition] &&
                    !cRow.BingoRowPattern[bitPosition])
                {
                    // A column across rows are all blank fields.
                    return false;
                }
            }
            // All columns contain atleast 1 field, which should hold a number.
            return true;
        }

        private int RandomInteger(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                byte[] four_bytes = new byte[4];
                rngCryptoProvider.GetBytes(four_bytes);

                scale = BitConverter.ToUInt32(four_bytes, 0);
            }
            // max not inclusive
            return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
        }

        private void Shuffle<T>(IList<T> list)
        {
            int countDownIndex = list.Count;
            while (countDownIndex > 1)
            {
                countDownIndex--;
                int k = RandomInteger(0, countDownIndex + 1);
                T value = list[k];
                list[k] = list[countDownIndex];
                list[countDownIndex] = value;
            }
        }
    }
}
