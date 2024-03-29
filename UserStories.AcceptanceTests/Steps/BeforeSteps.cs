using BoDi;
using CrossLayer.Containers;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace UserStories.AcceptanceTests
{
    [Binding]
    public class BeforeSteps
    {
        private readonly IObjectContainer objectContainer;
        private readonly IAppContainer appContainers;

        public BeforeSteps(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;

            // Inject app containers.
            this.RegisterAppContainerToObjectContainer();
            this.appContainers = this.objectContainer.Resolve<IAppContainer>();

            // Inject configuration to object container
            this.RegisterConfigurationToObjectContainer();

            //Inject httpClient
            this.RegisterHttpClient();
            this.objectContainer.Resolve<HttpClient>();
        }

        /// <summary>
        /// Sets up API scenarios. Triggers appcontainers
        /// </summary>
        [BeforeScenario]
        [Scope(Tag = "Type:API")]
        public void SetUpAPIScenarios()
        {
            this.appContainers.RegisterAPIs(this.objectContainer);
        }

        private void RegisterConfigurationToObjectContainer()
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            this.objectContainer.RegisterInstanceAs(configurationRoot);
        }

        private void RegisterHttpClient()
        {
            this.objectContainer.RegisterInstanceAs(new HttpClient() { Timeout = TimeSpan.FromSeconds(30) });
        }

        private void RegisterAppContainerToObjectContainer()
        {
            this.objectContainer.RegisterTypeAs<AppContainer, IAppContainer>();
        }
    }
}
