using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using Vsts.Domain.Service.Factories;
using Vsts.Infra.Provider.Factories;

namespace Vsts.Test.Console
{
    class Program
    {
        private static Container container;
        
        static void Main(string[] args)
        {
            container = GetContainer();
            var vstsProviderFactory = container.GetInstance<IVstsProviderFactory>();
            
            //TODO: Fill variables before run
            var ApiVersion = "1.0";
            var Account = "";
            var Token = "";

            var vsProjectProvider = vstsProviderFactory.CreateProjectProvider(ApiVersion, Account, Token);

            var TeamProject = (vsProjectProvider.GetProjectsAsync()).Result[0].Name;

            var vsWorkItemProvider = vstsProviderFactory.CreateWorkItemProvider(ApiVersion, Account, TeamProject, Token);

            try
            {
                var fields = vsWorkItemProvider.GetWorkItemColumnsAsync().Result;
                var TestList = vsWorkItemProvider.GetWorkItemIdAsync($"[System.TeamProject] = '{TeamProject}' AND NOT [State] = 'Removed' AND NOT [State] = 'Closed' AND NOT [State] = 'New'").Result;
                List<int> ids = TestList.WorkItems.Select(x => x.Id).ToList();
                
                var TestWorkItem = vsWorkItemProvider.GetWorkItemAsync(ids, fields.GetRange(0, 10)).Result;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                System.Console.ReadKey();
            }
        }

        private static Container GetContainer()
        {
            var _container = new Container();
            _container.RegisterSingleton<IServiceProvider>(_container);
            _container.Register<IVstsProviderFactory, VstsProviderFactory>();

            return _container;
        }
    }
}
