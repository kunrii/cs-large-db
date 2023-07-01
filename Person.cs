namespace ConsoleApp
{
    internal class Person
    {

        public int Idperson { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string? Notes { get; set; }

        public Person(string? idperson, string? name, string? date, string? notes)
        {
            if (idperson == null || name == null || date == null || notes == null)
                throw new System.Exception("One or more fields of Person are null - check the reading functionality");

            Idperson = int.Parse(idperson);
            Name = name;
            Date = date;
            Notes = (notes == "") ? null : notes;
        }

        public override string ToString()
        {
            return string.Format("Person id {0}: {1}, joined date {2}", Idperson, Name, Date) + ((Notes == null) ? "" : " - note: " + Notes);
        }
    }
}
