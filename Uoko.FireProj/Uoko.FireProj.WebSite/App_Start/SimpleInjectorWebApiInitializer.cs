[assembly: WebActivator.PostApplicationStartMethod(typeof(Uoko.FireProj.WebSite.App_Start.SimpleInjectorWebApiInitializer), "Initialize")]

namespace Uoko.FireProj.WebSite.App_Start
{
    using System.Web.Http;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using Concretes;
    using System.Linq;
    using Mehdime.Entity;

    public static class SimpleInjectorWebApiInitializer
    {
        /// <summary>Initialize the container and register it as Web API Dependency Resolver.</summary>
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();
            
            InitializeContainer(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
       
            container.Verify();
            
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
     
        private static void InitializeContainer(Container container)
        {
            container.Register<IDbContextScopeFactory>(() => new DbContextScopeFactory(), Lifestyle.Scoped);//×¢²áEF

            var serviceAssembly = typeof(ProjectSvc).Assembly;

            var registrations =
                from type in serviceAssembly.GetExportedTypes()
                where type.Namespace == "Uoko.FireProj.Concretes" && type.GetInterfaces().Any()
                select new { Service = type.GetInterfaces().First(), Implementation = type };

            foreach (var reg in registrations)
            {
                container.Register(reg.Service, reg.Implementation, Lifestyle.Scoped);
            }
        }
    }
}