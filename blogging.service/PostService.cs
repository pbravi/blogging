using blogging.data;
using blogging.model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blogging.service
{
    public class PostService : BaseService<Post>
    {
        public PostService(IRepository<Post> postRepository) : base(postRepository)
        {

        }
        public async override Task InsertAsync(Post entity)
        {
            entity.CreationDate = DateTime.Now;
            await base.InsertAsync(entity);
        }

        public async override Task<List<Post>> GetAllAsync(string textSearch)
        {
            if (string.IsNullOrEmpty(textSearch))
                return await base.GetAllAsync();
            else
                return await base.WhereAsync(p => p.Autor.Contains(textSearch) || p.Title.Contains(textSearch) || p.Body.Contains(textSearch));
        }
    }
}
