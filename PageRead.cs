using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    // This class handles a paginated read operation from a database
    internal class PageRead : State
    {
        int pageSize;                               //How big are the pages to be read
        int currentPage;                            //The current page that will be read

        MySqlConnection? conn;
        int divRes;                                 //How many chunks of MAXIMUM_ELEMENTS size exist
        int modRes;                                 //The size of the non-standard data chunk, which may not exist if the remainder of the division between the total elements in a database and the standard chunk size is zero

        public override Type Update()
        {
            this.pageSize = Program.PAGE_SIZE;
            this.currentPage = 1;

            this.conn = Program.conn;
            if (conn == null)
                throw new Exception("Failed to connect to database");

            this.divRes = Program.TOTAL_INSERTS / pageSize;
            this.modRes = Program.TOTAL_INSERTS % pageSize;

            BigRead bigRead = new BigRead(conn, Program.MAXIMUM_ELEMENTS, divRes, modRes);
            ListPeople(bigRead);                    //Immediately performs a read operation

            bool continueRead = true;
            while (continueRead)
            {
                string? input = Console.ReadLine();
                if (input == null)
                    throw new Exception("Failed to read from console");

                switch (input)
                {
                    case "1":
                        if (Back())                 //If trying to go back one page succeeds, then read that page
                            ListPeople(bigRead);
                        break;
                    case "0":
                        continueRead = false;
                        break;
                    default:
                        if(Next())                  //If trying to go forward one page succeeds, then read that page
                            ListPeople(bigRead);
                        break;
                }
            }


            return typeof(MainMenu);
        }

        //Tries to go forward one page
        bool Next()
        {
            //The criterion to determine whether you can go forward.
            if (currentPage + 1 > GetTotalPages())
            {
                Console.WriteLine("Cannot continue reading forward. Insert 1 to go back one page, or 0 to return");

                return false;
            }
            else
            {
                currentPage++;

                return true;
            }
        }

        //Tries to go back one page
        bool Back()
        {
            //The criterion to determine whether you can go backward. Pages start at 1.
            if (currentPage - 1 < 1)
            {
                Console.WriteLine("Cannot continue reading backward. Insert anything to go forward, or 0 to return");

                return false;
            }
            else
            {
                currentPage--;

                return true;
            }
        }

        //Lists people by determining how big the data chunk should be, and calling a read operation
        void ListPeople(BigRead bigRead)
        {
            int limit = GetLimit();                                             //How big the read is going to be
            int offset = pageSize * (currentPage - 1);                          //In which row is it going to begin

            List<Person> people = bigRead.GetPeople(limit, offset);             //Calls a read operation
                
            bigRead.ListPeople(people);                                         //Lists the people

            Console.WriteLine(string.Format("Showing page {0} of {1}. Insert anything to continue, 1 to go back one page, or 0 to return", currentPage, GetTotalPages()));
        }

        //There are divRes full pages and one page with modRes size if modRes greater than 0, otherwise there are divRes pages with pageSize size
        int GetTotalPages()
        {
            return (modRes > 0) ? divRes + 1 : divRes;
        }

        //Gets the correct limits based on whether the modRes is zero or not
        int GetLimit()
        {
            if (modRes > 0)
            {
                return (currentPage < GetTotalPages()) ? pageSize : modRes;
            } 
            else
            {
                return pageSize;
            }
        }
    }
}