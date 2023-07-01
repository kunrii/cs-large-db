using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    internal class BulkDelete : State
    {
        // This class is in charge of handling a bulk deletion from a database
        public override Type Update()
        {
            MySqlConnection? conn = Program.conn;
            if (conn == null)
                throw new Exception("");

            Console.WriteLine("Deleting bulk data from the database");

            string strSQL;
            MySqlCommand cmd;

            //Deletes can be handled by a single call at this scale since data isn't being moved in and out of the database, if that were not the case special attention would be needed
            strSQL = "DELETE FROM person;";
            cmd = new MySqlCommand(strSQL, conn);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Successfully deleted bulk data from the database");

            return typeof(MainMenu);
        }
    }
}
