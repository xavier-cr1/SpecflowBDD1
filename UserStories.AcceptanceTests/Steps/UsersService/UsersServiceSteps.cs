﻿using APILayer.Client.Contracts;
using APILayer.Entities;
using APILayer.Entities.UserService;
using FluentAssertions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace UserStories.AcceptanceTests.Steps
{
    [Binding]
    public class UsersServiceSteps : StepBase
    {
        private readonly IUsersServiceRestApi usersServiceRestApi;
        private User userRequest;
        private SwaggerResponse response;
        private SwaggerResponse<UserListResponse> userListResponse;

        public UsersServiceSteps(IUsersServiceRestApi usersServiceRestApi)
        {
            this.usersServiceRestApi = usersServiceRestApi;
        }

        [Given(@"The forum receives a request for creating a user with the following properties")]
        public async Task GivenTheForumReceivesARequestForCreatingAUserWithTheFollowingProperties(Table table)
        {
            this.userRequest = table.CreateInstance<User>();
            this.response = await this.usersServiceRestApi.PostNewUserAsync(this.userRequest);
        }

        [When(@"The forum receives a request for obtaining the user list")]
        public async Task GivenTheForumReceivesARequestForCreatingAUserWithTheFollowingProperties()
        {
            this.userListResponse = await this.usersServiceRestApi.GetUserListAsync();
        }

        [When(@"The status code for getting the users list is '(.*)'")]
        public void ThenTheStatusCodeForGettingTheUsersListIs(string expectedStatusCode)
        {
            var realStatusCode = this.userListResponse.StatusCode;
            realStatusCode.Should().Be(expectedStatusCode, $"Real code {realStatusCode} --- Expected code {expectedStatusCode}");
        }

        [Then(@"The status code for creating a new user is '(.*)'")]
        public void ThenTheStatusCodeForCreatingNewUserIs(string expectedStatusCode)
        {
            var realStatusCode = this.response.StatusCode;
            realStatusCode.Should().Be(expectedStatusCode, $"Real code {realStatusCode} --- Expected code {expectedStatusCode}");
        }

        [Then(@"The user with the username '(.*)' is in the list")]
        public void ThenTheUniqueUserIsInTheList(string username)
        {
            var realUserList = this.userListResponse.Result.Users;
            realUserList.Should().Contain(user => user.Username.Equals(username));
        }
    }
}
