namespace Chambers.API.Model
{
    public class Document
    {
        public Document(string id, string name, string location, long size)
        {
            Id = id;
            Name = name;
            Location = location;
            Size = size;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public long Size { get; private set; }
    }
}
