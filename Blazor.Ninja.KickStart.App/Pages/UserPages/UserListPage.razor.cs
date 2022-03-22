using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazor.Ninja.Common.Client;
using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Sdk.AspNetCore;

namespace Blazor.Ninja.KickStart.App.Pages.UserPages
{
    public partial class UserListPage
    {
	    private const int PageSize = 3;

        private IUserProxy<GenericUser> _proxy;

        private GenericUser _currentUser = new();
        private UserFollow _currentUserFollowData = new();

        private int _pageNumber;
        private List<GenericUser> _users = new();
        private long _total;
        private List<Tuple<string, long, long>> _map;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
	            State = BlazorNinjaComponentState.Loading;

                var token = await GetContextTokenAsync();

	            _proxy = ProxyFactory.GetUserProxy<GenericUser>(token);

	            _currentUser = await _proxy.GetAsync();

                await LoadAsync();

	            State = BlazorNinjaComponentState.WaitingForInput;
            }
            catch (Exception ex)
            {
	            State = BlazorNinjaComponentState.Failed;

	            await HandleExceptionAsync(ex);
            }
        }

        protected override async Task LoadAsync()
        {
	        var filter = Builders<GenericUser>.Filter.Ne(it => it.Id, _currentUser.Id);

            var userPage = await _proxy.GetPageAsync(
	            filter, 
	            _pageNumber, 
	            PageSize, 
	            SortOrder<GenericUser>.Empty);

            _users = userPage.Items;

            _map = new List<Tuple<string, long, long>>();

            foreach (var user in _users)
            {
	            var followers = await _proxy.GetFollowersAsync(user.Id);
	            var following = await _proxy.GetFollowingAsync(user.Id);
	            var tuple = new Tuple<string, long, long>(user.Id, followers, following);
                _map.Add(tuple);
            }

            _currentUserFollowData = await _proxy.GetFollowDataAsync(_currentUser.Id);

            _total = await _proxy.GetCountAsync(Filter<GenericUser>.Empty);
        }

        private static string GetName(
	        IUser user)
        {
	        var name = $"{user.FirstName} {user.LastName}";

	        if (!string.IsNullOrWhiteSpace(name)) return name;

	        if (!string.IsNullOrWhiteSpace(user.Username)) return user.Username;

            if (!string.IsNullOrWhiteSpace(user.Email)) return user.Email;

            if (!string.IsNullOrWhiteSpace(user.Phone)) return user.Phone;

            return user.Id;
        }

        private long GetFollowers(
	        string userId)
        {
	        return _map.First(it => it.Item1 == userId).Item2;
        }

        private long GetFollowing(
	        string userId)
        {
	        return _map.First(it => it.Item1 == userId).Item3;
        }

        private bool IsFollowing(
	        string userId)
        {
	        return _currentUserFollowData.Following.Any(x => x == userId);
        }

        private async Task FollowAsync(
	        string userId)
        {
	        try
            {
	            State = BlazorNinjaComponentState.Working;

	            await _proxy.FollowAsync(_currentUser.Id, userId);

	            await LoadAsync();

	            State = BlazorNinjaComponentState.WaitingForInput;
            }
            catch (Exception ex)
            {
	            State = BlazorNinjaComponentState.WaitingForInput;

	            await HandleExceptionAsync(ex);
            }
        }

        private async Task UnfollowAsync(
	        string userId)
        {
	        try
            {
	            State = BlazorNinjaComponentState.Working;

	            await _proxy.UnfollowAsync(_currentUser.Id, userId);

                await LoadAsync();

	            State = BlazorNinjaComponentState.WaitingForInput;
            }
            catch (Exception ex)
            {
	            State = BlazorNinjaComponentState.WaitingForInput;

	            await HandleExceptionAsync(ex);
            }
        }

        private async Task OnNextClickedAsync()
        {
            _pageNumber += 1;

            await LoadAsync();
        }

        private async Task OnPreviousClickedAsync()
        {
            _pageNumber -= 1;

            await LoadAsync();
        }

        private async Task OnPageClickedAsync(
	        int page)
        {
            _pageNumber = page;

            await LoadAsync();
        }
    }
}
