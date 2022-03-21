using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazor.Ninja.Common.Client;
using Blazor.Ninja.Common.Data;
using Blazor.Ninja.KickStart.Common;
using Blazor.Ninja.Sdk.AspNetCore;

namespace Blazor.Ninja.KickStart.App.Pages.UserPages
{
    public partial class UserListPage
    {
        private IUserProxy<CustomUser> _proxy;

        private BlazorNinjaComponentState _state = BlazorNinjaComponentState.Loading;

        private CustomUser _currentUser = new CustomUser();

        private UserFollow _userFollow = new UserFollow();

        private List<CustomUser> _users = new List<CustomUser>();

        private long _userCounts;

        private int _pageSize = 3;

        private int _pageNumber = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var token = await GetContextTokenAsync();

            _proxy = ProxyFactory.GetUserProxy<CustomUser>(token);

            await GetFollowDataAsync();

            _users = await LoadAsync();

            _userCounts = await _proxy.GetCountAsync(Filter<CustomUser>.Empty);

            _state = BlazorNinjaComponentState.WaitingForInput;
        }

        private async Task<List<CustomUser>> LoadAsync()
        {
            var userPage = await _proxy.GetPageAsync(Filter<CustomUser>.Empty, _pageNumber, _pageSize, SortOrder<CustomUser>.Empty);

            var users = userPage.Items;

            var customUsers = await Task.WhenAll(users.Select(async x => new CustomUser
            {
                Id = x.Id,
                PhotoUrl = x.PhotoUrl,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Followers = await _proxy.GetFollowersAsync(x.Id),
                Following = await _proxy.GetFollowingAsync(x.Id)

            }));

            return customUsers.ToList();
        }

        private async Task FollowAsync(string id)
        {
            _state = BlazorNinjaComponentState.Working;

            await _proxy.FollowAsync(_currentUser.Id, id);

            await GetFollowDataAsync();

            _users = await LoadAsync();

            _state = BlazorNinjaComponentState.WaitingForInput;
        }

        private async Task UnfollowAsync(string id)
        {
            _state = BlazorNinjaComponentState.Working;

            await _proxy.UnfollowAsync(_currentUser.Id, id);

            await GetFollowDataAsync();

            _users = await LoadAsync();

            _state = BlazorNinjaComponentState.WaitingForInput;
        }

        public async Task NavigateForward()
        {
            _pageNumber += 1;

            _users = await LoadAsync();
        }

        public async Task NavigateBack()
        {
            _pageNumber -= 1;

            _users = await LoadAsync();
        }


        public async Task OnPageChange(int page)
        {
            _pageNumber = page;

            _users = await LoadAsync();
        }

        private async Task GetFollowDataAsync()
        {
            var token = await GetContextTokenAsync();

            _proxy = ProxyFactory.GetUserProxy<CustomUser>(token);

            _currentUser = await _proxy.GetAsync();

            _userFollow = await _proxy.GetFollowDataAsync(_currentUser.Id);
        }
    }
}
