namespace Chambers.API.Model
{
    public class Document
    {
        public Document(string name, string location, long size)
        {
            Name = name;
            Location = location;
            Size = size;
        }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public long Size { get; private set; }
    }
}
