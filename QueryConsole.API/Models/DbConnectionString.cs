namespace QueryConsole.API.Models
{
    public class DbConnectionString
    {
        public DbConnectionString(string name, string connString)
        {
            this.Name = name;
            this.Value = connString;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
