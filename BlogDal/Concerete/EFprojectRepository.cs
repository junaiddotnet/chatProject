using BlogDal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFprojectRepository : IprojectRepository
    {
        private EFDbcontext context = new EFDbcontext();
        public IQueryable<project> projects
        {
            get
            {
                return context.projects;
            }
        }

        public void addProject(project prmProject)
        {
            throw new NotImplementedException();
        }

        public void editProject(project prmProject)
        {
            project dbProject = context.projects.Where(c => c.projectId == prmProject.projectId).FirstOrDefault();
            dbProject = prmProject;
            context.SaveChanges();
        }
    }
}
