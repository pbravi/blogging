using blogging.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace blogging.data
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository(AppDbContext context) : base(context) { }
    }
}
