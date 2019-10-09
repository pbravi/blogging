using System;
using System.ComponentModel.DataAnnotations;

namespace blogging.model
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Autor { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
