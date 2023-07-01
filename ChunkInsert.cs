using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    // This class is in charge of handling the generation and bulk insertion of data into the database
    internal class ChunkInsert : State
    {
        public override Type Update()
        {
            MySqlConnection? conn = Program.conn;
            if (conn == null)
                throw new Exception("Failed to connect to database");

            int divRes = Program.TOTAL_INSERTS / Program.MAXIMUM_ELEMENTS;      //How many full chunks of data of standard size will exist
            int modRes = Program.TOTAL_INSERTS % Program.MAXIMUM_ELEMENTS;      //The size of a chunk of data corresponding to the remainder of the division of the total elements in the database by the size of a standard chunk

            Console.WriteLine("Inserting bulk data into person table");

            InsertPersonTable(conn, Program.MAXIMUM_ELEMENTS, divRes, modRes);

            Console.WriteLine("Successfully inserted bulk data into person table, returning to main menu");

            return typeof(MainMenu);
        }

        //Inserts a single chunk of data into the person table, by formating a very large string
        void InsertPerson(MySqlConnection conn, int amount)
        {
            if (amount == 0)
                return;

            //Strings in C# are immutable, this avoids creating a new string in order to append a string to another
            StringBuilder strSQL = new StringBuilder("INSERT INTO person (name, date, notes) VALUES");
            for (int i = 0; i < amount; i++)
            {
                //Format matches that of the table
                strSQL.AppendFormat(" (\"{0}\",\"{1}\",\"{2}\")", GenerateName(), GenerateDate(1990, 30), GenerateNote(0.2f));

                //If not the last value, insert a ','
                if (i < amount - 1)
                {
                    strSQL.Append(",");
                }
            }
            //Finishes the SQL query string with a ';'
            strSQL.Append(";");

            //Executes this query
            MySqlCommand cmd = new MySqlCommand(strSQL.ToString(), conn);
            cmd.ExecuteNonQuery();
        }

        //Is in charge of managing chunk insertion of data into person table; chunks are broken down into size of divRes chunks of MAXIMUM_ELEMENTS size, and one chunk of modRes size
        void InsertPersonTable(MySqlConnection conn, int MAXIMUM_ELEMENTS, int divRes, int modRes)
        {
            for (int j = 0; j < divRes; j++)
            {
                InsertPerson(conn, MAXIMUM_ELEMENTS);
            }
            InsertPerson(conn, modRes);
        }

        ///////////////////////////////////////////////////////////////////////
        //The functions below generate a name, a date and a note
        ///////////////////////////////////////////////////////////////////////

        //These are lists of names and occupations
        public string[] givenNames = { "David", "John", "Michael", "Paul", "Andrew", "Peter", "James", "Robert", "Mark", "Richard" };
        public string[] surnames = { "Smith", "Jones", "Williams", "Taylor", "Brown", "Davies", "Evans", "Thomas", "Wilson", "Johnson" };
        public string[] occupations = { "Software Engineer", "Business Analyst", "Human Resources", "Marketing", "Manager", "Janitor" };

        //Generates a random name
        string GenerateName()
        {
            Random rand = new Random();
            return string.Format("{0} {1} {2}", givenNames[rand.Next(0, givenNames.Length)], givenNames[rand.Next(0, givenNames.Length)], surnames[rand.Next(0, surnames.Length)]);

        }

        //Generates a random date from starting year to startingYear + years
        string GenerateDate(int startingYear, int years)
        {
            Random rand = new Random();
            DateTime start = new DateTime(startingYear, 1, 1);
            DateTime registerTime = start.AddDays(rand.Next(365 * years));
            return registerTime.ToShortDateString();
        }

        //Generates a random note with probability of odds, otherwise the note is empty (but not null, the MySQL column is not nullable)
        string GenerateNote(float odds)
        {
            Random rand = new Random();
            int trial = rand.Next(0, 101);
            float normedTrial = ((float)trial) / 100f;

            return (normedTrial < odds) ? string.Format("Note #{0}", rand.Next(1000000, 9999999)) : "";
        }
    }
}