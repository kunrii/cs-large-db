using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    //This class handles the reading of chunks of data from a database.
    //There may be divRes chunks of MAXIMUM_ELEMENTS size. There may be one chunk of modRes size. There will always be one chunk of data.
    internal class BigRead
    {
        MySqlConnection conn;
        int readSize;
        int divRes;
        int modRes;

        public BigRead(MySqlConnection? conn, int MAXIMUM_ELEMENTS, int divRes, int modRes)
        {
            if (conn == null)
                throw new Exception("Failed to connect to database");

            this.conn = conn;
            this.readSize = MAXIMUM_ELEMENTS;
            this.divRes = divRes;
            this.modRes = modRes;
        }

        /*
            These methods build a query string and query fields, and return the output from the read methods. 
            Two of these methods rely on this class's method to chunk up data, the other assumes the chunk is already known
        */
        public List<Person> GetPeople()
        {
            return ReadPersonTable(conn, readSize, divRes, modRes, "SELECT idperson FROM person LIMIT {0} OFFSET {1};");
        }

        public List<Person> GetPeople(string query)
        {
            return ReadPersonTable(conn, readSize, divRes, modRes, string.Format("SELECT * FROM person WHERE person.idperson LIKE \"%{0}%\" OR person.name LIKE \"%{0}%\" OR person.date LIKE \"%{0}%\" OR person.notes LIKE \"%{0}%\" ", query) + " LIMIT {0} OFFSET {1};");
        }

        //Used when the chunk is already known. In this case, the chunk has size limit and starts at offset
        public List<Person> GetPeople(int limit, int offset)
        {

            List<Person> people = new List<Person>();

            ReadPerson(conn, limit, offset, people, "SELECT * FROM person LIMIT {0} OFFSET {1};");

            return people;
        }

        /*
            This method queries a chunk from the database
        */

        //Reads a single chunk of data of size limit starting at offset, with a query contained in query
        void ReadPerson(MySqlConnection conn, int limit, int offset, List<Person> people, string query)
        {
            if (limit == 0)
                return;

            string strSQL = string.Format(query, limit, offset);

            MySqlCommand cmd = new MySqlCommand(strSQL, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                people.Add(new Person(reader["idperson"].ToString(), reader["name"].ToString(), reader["date"].ToString(), reader["notes"].ToString()));
            }
            reader.Close();
        }

        /*
            This method chunks up the data and calls read operations
        */

        //Chunks up the data and calls the correct read operation for each one
        //Remember: There may be divRes chunks of MAXIMUM_ELEMENTS size. There may be one chunk of modRes size. There will always be one chunk of data.
        List<Person> ReadPersonTable(MySqlConnection conn, int readSize, int divRes, int modRes, string query)
        {
            List<Person> people = new List<Person>();
            int count = 0;

            //Standard size chunks
            for (int j = 0; j < divRes; j++)
            {
                ReadPerson(conn, readSize, count * readSize, people, query);

                count++;
            }

            //modRes size chunk
            ReadPerson(conn, modRes, count * readSize, people, query);

            return people;
        }

        /*
            This method lists a person
        */

        public void ListPeople(List<Person> people)
        {
            foreach (Person p in people)
                Console.WriteLine(p.ToString());
        }
    }
}
