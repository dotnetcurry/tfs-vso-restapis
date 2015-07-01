using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VisualStudioOnline.Api.Rest.Test.Properties;
using VisualStudioOnline.Api.Rest.V1.Client;

namespace VisualStudioOnline.Api.Rest.Test.V1
{
    [TestClass]
    public class ProjectRestClientTest : VsoTestBase
    {
        private IVsoProject _client;

        protected override void OnInitialize(VsoClient vsoClient)
        {
            _client = vsoClient.GetService<IVsoProject>();
        }

        [TestMethod]
        public void TestGetProjects()
        {
            var projects = _client.GetTeamProjects().Result;

            var project = _client.GetTeamProject(projects[0].Id.ToString(), true).Result;
            project.Description = DateTime.Now.Ticks.ToString();

            project = _client.UpdateTeamProject(project).Result;
        }

        [TestMethod]
        public void TestGetTeams()
        {
            var teams = _client.GetProjectTeams(Settings.Default.ProjectName).Result;

            var team = _client.GetProjectTeam(Settings.Default.ProjectName, teams[0].Id.ToString()).Result;
            var teamMembers = _client.GetTeamMembers(Settings.Default.ProjectName, team.Name).Result;
        }
    }
}
