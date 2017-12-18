using SimpleInjector;
using System;
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

            string[] fieldList = { "System.Id", "System.Title", "System.State", "System.IterationPath", "System.AssignedTo", "System.WorkItemType", "System.CreatedDate", "System.CreatedBy", "System.Description" };

            //TODO: Fill variables before run
            var ApiVersion = "";
            var TeamProject = "";
            var Account = "";
            var Token = "";

            var vsProvider = vstsProviderFactory.Create(ApiVersion, Account, TeamProject, Token);

            try
            {
                var TestList = vsProvider.GetWorkItemIdAsync($"[System.TeamProject] = '{TeamProject}' AND NOT [State] = 'Removed' AND NOT [State] = 'Closed' AND NOT [State] = 'New'").Result;
                int[] ids = { TestList.WorkItems[0].Id };
                
                var TestWorkItem = vsProvider.GetWorkItemAsync(ids, fieldList).Result;
            }
            catch (System.Exception ex)
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
