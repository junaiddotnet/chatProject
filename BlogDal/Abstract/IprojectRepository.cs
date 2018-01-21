using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Abstract
{
    public interface IprojectRepository
    {
        IQueryable<project> projects { get; }
        void addProject(project prmProject);
        void editProject(project prmProject);
    }
}
