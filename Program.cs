using MySql.Data.MySqlClient;

namespace ConsoleApp
{
    internal class Program
    {
        //The following are just suggestions, as long as MAXIMUM_ELEMENTS (the number of elements in a chunk) is kept reasonable, this application can handle a very large number of values
        public static int MAXIMUM_ELEMENTS = 100;           //How many elements can be read in a chunk of data
        public static int TOTAL_INSERTS = 236;              //How many elements are inserted in the database
        public static int PAGE_SIZE = 15;                   //Page size when reading
        public static MySqlConnection? conn = null;
     
        // The Main function is in charge of opening a connection to a MySQL server and hosting a menu for each use case
        static void Main(string[] args)
        {
            //Insert the name of the user, the password and the name of the database as an argument when the program is called
            if (args.Length < 4)
                throw new Exception("Insufficient arguments");

            if (PAGE_SIZE > MAXIMUM_ELEMENTS || PAGE_SIZE == 0)
                throw new Exception("Invalid page size");

            //The various controllers and the current controller
            Dictionary<Type, State> controllers = SetupControllers();
            State currentController = controllers[typeof(MainMenu)];

            //Opens up a connection to a server
            conn = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3};", "localhost", args[1], args[2], args[3]));
            conn.Open();

            //Simple menu logic emulating a finite state machine
            Type state = typeof(MainMenu);
            while (state != typeof(Exit))
            {
                state = currentController.Update();
                currentController = controllers[state];
            }

            //Closes the connection before exiting
            conn.Close();
        }

        // Setup for various controller objects that represent different use cases. 
        // These objects are set up and live independently of each other and the menu logic.
        // This way, there's no need to constantly be creating and destroying objects whenever a new use case is desired.
        static Dictionary<Type, State> SetupControllers()
        {
            Dictionary<Type, State> controllers = new Dictionary<Type, State>();

            State mainMenu = new MainMenu();
            State bulkInsert = new ChunkInsert();
            State bulkDelete = new BulkDelete();
            State bulkRead = new FullRead();
            State pageRead = new PageRead();
            State exit = new Exit();

            controllers.Add(typeof(MainMenu), mainMenu);
            controllers.Add(typeof(ChunkInsert), bulkInsert);
            controllers.Add(typeof(BulkDelete), bulkDelete);
            controllers.Add(typeof(FullRead), bulkRead);
            controllers.Add(typeof(PageRead), pageRead);
            controllers.Add(typeof(Exit), exit);

            return controllers;
        }
    }
}