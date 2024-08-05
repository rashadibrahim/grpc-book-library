using System;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string isbn { get; set; }
        public int publication_year { get; set; }
        public string genre { get; set; }
        public bool available { get; set; } = true;
        public int borrowerId { get; set; } = 0;


    }
}
