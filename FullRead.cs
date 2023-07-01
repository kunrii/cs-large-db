using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    internal class FullRead : State
    {
        public override Type Update()
        {
            MySqlConnection? conn = Program.conn;
            if (conn == null)
                throw new Exception("Failed to connect to database");

            int divRes = Program.TOTAL_INSERTS / Program.MAXIMUM_ELEMENTS;
            int modRes = Program.TOTAL_INSERTS % Program.MAXIMUM_ELEMENTS;

            Console.WriteLine("Insert the query string:");
            string? queryString = Console.ReadLine();
            if (queryString == null)
                throw new Exception("Failed to read from console");

            BigRead bigRead = new BigRead(conn, Program.MAXIMUM_ELEMENTS, divRes, modRes);
            List<Person> people = bigRead.GetPeople(queryString);

            bigRead.ListPeople(people);

            Console.WriteLine("Read over, returning to main menu");

            return typeof(MainMenu);
        }
    }
}
