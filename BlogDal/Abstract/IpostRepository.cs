using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Abstract
{
    public interface IpostRepository
    {
        IQueryable<post> posts { get; }
        IQueryable<favPost> favPosts { get; }
        IQueryable<categorie> categories { get; }
        IQueryable<comment> comments { get; }

        void savePostCategory(categorie prmCategory);
        void addPosttoFavourite(int userId, int postId);
       void  RemovePosttoFavourite(int userId, int postId);
        void Save(post prmPost);
        void Delete(int postId);
        void delComment(int commentId);
        void addComment(comment prmComment);
    }
}
