using BlogDal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFpostRepository : IpostRepository
    {
        private EFDbcontext context = new EFDbcontext();
        public IQueryable<favPost> favPosts
        {
            get
            {
                return context.favPosts;
            }
        }
        public IQueryable<post> posts
        {
            get
            {
                return context.posts;
            }
        }
        public IQueryable<categorie> categories
        {
            get
            {
                return context.categories;
            }
        }
        public IQueryable<comment> comments
        {
            get
            {
                return context.comments;
            }
        }

        public void Delete(int postId)
        {
            throw new NotImplementedException();
        }

        public void Save(post prmPost)
        {
            if (prmPost.postId==0)
            {
                prmPost.createDate = DateTime.Now;
                
                context.posts.Add(prmPost);
            }
            else
            {
                post dbPost = context.posts.Where(c => c.postId == prmPost.postId).FirstOrDefault();
                dbPost.postText = prmPost.postText;
                dbPost.postName = prmPost.postName;
                dbPost.categoryId = prmPost.categoryId;
                context.SaveChanges();
            }

            context.SaveChanges();
        }
        public void addComment (comment prmComment)
        {
            context.comments.Add(prmComment);
            context.SaveChanges();
            
        }
        public  void delComment(int commentId)
        {
            comment dbComment = context.comments.Where(c => c.commentId == commentId).FirstOrDefault();
            context.comments.Remove(dbComment);
            context.SaveChanges();
        }

        public void savePostCategory(categorie prmCategory)
        {
            if (prmCategory.categoryId==0)
            {
                context.categories.Add(prmCategory);
            }
            else
            {
                categorie dbCategory = context.categories.Where(c => c.categoryId == prmCategory.categoryId).FirstOrDefault();
                dbCategory.categoryName = prmCategory.categoryName;
                
            }
            context.SaveChanges();
        }
        public void addPosttoFavourite(int userId, int postId)
        {
            if (userId!=0 && postId!=0)
            {
                context.favPosts.Add(new favPost
                {
                    userId = userId,
                    postId = postId,
                    comment="Just Added",
                    favDate=DateTime.Now
                    
                });
                context.SaveChanges();
            }
        }
        public void RemovePosttoFavourite(int userId, int postId)
        {
            favPost post = context.favPosts.Where(c => c.postId == postId && c.userId == userId).FirstOrDefault();
            context.favPosts.Remove(post);
            context.SaveChanges();
        }

    }
}
