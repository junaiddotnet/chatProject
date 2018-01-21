using BlogDal.Abstract;
using BlogDal.Concerete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace blogProject.Infastructure
{
    public class ninjectControllerFactory:DefaultControllerFactory
    {
        public IKernel ninjectKernal;
        public ninjectControllerFactory()
        {
            ninjectKernal = new StandardKernel();
            Addbindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                                    ? null
                                    : (IController)ninjectKernal.Get(controllerType);

        }
        public void Addbindings()
        {
            ninjectKernal.Bind<IpostRepository>().To<EFpostRepository>();
            ninjectKernal.Bind<IuserRepository>().To<EFuserRepository>();
            ninjectKernal.Bind<IgroupRepository>().To<EFgroupRepository>();
            ninjectKernal.Bind<IadminRepository>().To<EFadminRepository>();
            ninjectKernal.Bind<IprojectRepository>().To<EFprojectRepository>();
        }
    }
}