using GameOnTonight.App.Services;
using GameOnTonight.RestClient.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GameOnTonight.App.Pages.Groups;

public partial class GroupsPage : ComponentBase
{
    private IReadOnlyList<GroupViewModel> _groups = [];
    private bool _isLoading = true;
    
    private static readonly Color[] GroupColors = 
    [
        Color.Primary,
        Color.Secondary,
        Color.Tertiary,
        Color.Info,
        Color.Success,
        Color.Warning
    ];

    protected override async Task OnInitializedAsync()
    {
        await LoadGroupsAsync();
    }

    private async Task LoadGroupsAsync()
    {
        _isLoading = true;
        StateHasChanged();
        
        try
        {
            _groups = await GroupsService.GetMyGroupsAsync();
        }
        catch (Exception)
        {
            Snackbar.Add("Erreur lors du chargement des groupes", Severity.Error);
            _groups = [];
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void NavigateToCreate()
    {
        Navigation.NavigateTo("/groups/create");
    }

    private void NavigateToJoin()
    {
        Navigation.NavigateTo("/groups/join");
    }

    private void NavigateToGroup(int? groupId)
    {
        if (groupId.HasValue)
        {
            Navigation.NavigateTo($"/groups/{groupId.Value}");
        }
    }

    private static Color GetGroupColor(int groupId)
    {
        return GroupColors[Math.Abs(groupId) % GroupColors.Length];
    }
}
