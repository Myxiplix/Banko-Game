namespace BingoTest
{

    /* 
    * BingoTicket layout:
    * 
    *   C1 C2 C3 C4 C5 C6 C7 C8 C9
    *  ----------------------------
    *  |01|04|07|10|13|16|19|22|25| R1
    *  ----------------------------
    *  |02|05|08|11|14|17|20|23|26| R2
    *  ----------------------------
    *  |03|06|09|12|15|18|21|24|27| R3
    *  ----------------------------
    *  C = Column, R = Row
    * 
    * 9 columns by 3 row, which gives 27 fields per bingo ticket
    * 15 numbers per ticket, 5 numbers per row
    * 12 blank fields per ticket, 4 blank fields per row
    * 
    * There can only be 1-4 consecutive blank fields per row (My rule)
    * Each column must contain atleast 1 number, no empty columns
    * 
    * Numbers 1 to 90 are to be placed in C1-C9 like this:
    * 
    * C1[1–9], C2[10–19], C3[20–29], C4[30–39], C5[40–49],
    * C6[50–59], C7[60–69], C8[70–79] and C9[80–90]
    * 
    * Numbers in a columns must be placed in an increasing order (top down)
    * Eg. field 01 is 4, field 02 is blank, field 03 is 8
    * 
    * 
    * A Bingo strip contains 6 unique tickets, so that
    * every number from 1 to 90 to appear across all 6 tickets.
    * Note that two tickets can have exactly the same numbers, but
    * placed at different fields (not on the same strip though).
    */

    class Program
    {
        static void Main(string[] args)
        {
            BingoTicketGenerator bcg = new BingoTicketGenerator();
            for (int i = 0; i < 4; i++)
            {
                System.Console.WriteLine(string.Format("Writing to file, tickets with {0} consecutive blank spaces...", (i + 1)));
                bcg.BlankSpaceLimit = (i + 1);
                bcg.BuildBingoTickets();
                bcg.SaveBingoTicketsToFile();
                System.Console.WriteLine(string.Format("Done writing to file, {0} tickets.", bcg.BingoTicketsList.Count));
            }
            System.Console.Write("\nPress Enter to continue... ");
            System.Console.ReadLine();
        }
    }
}
