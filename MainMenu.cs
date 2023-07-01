namespace ConsoleApp
{
    // This class shows the user a menu with various options for using the application
    internal class MainMenu : State
    {
        public override Type Update()
        {
            Console.WriteLine("Select case:");
            Console.WriteLine("1 : Chunked insert of data into database");
            Console.WriteLine("2 : Bulk delete data from database");
            Console.WriteLine("3 : Full (chunked) read data from database");
            Console.WriteLine("4 : Paginated read from database");
            Console.WriteLine("0 : Exit");
            Console.WriteLine("");

            string? usecase = Console.ReadLine();
            if (usecase == null)
                throw new Exception("Failed to read from console");

            switch (usecase)
            {
                case "0":
                    return typeof(Exit);
                case "1":
                    return typeof(ChunkInsert);
                case "2":
                    return typeof(BulkDelete);
                case "3":
                    return typeof(FullRead);
                case "4":
                    return typeof(PageRead);
                default:
                    return typeof(MainMenu);
            }
        }
    }
}
