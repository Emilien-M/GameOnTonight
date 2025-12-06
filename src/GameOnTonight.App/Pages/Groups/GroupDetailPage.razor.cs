using GameOnTonight.App.Components.Groups;
using GameOnTonight.App.Services;
using GameOnTonight.RestClient.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GameOnTonight.App.Pages.Groups;

public partial class GroupDetailPage : ComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    
    [Parameter]
    public int GroupId { get; set; }
    
    private GroupDetailViewModel? _group;
    private IReadOnlyList<GroupMemberViewModel> _members = [];
    private IReadOnlyList<GroupInviteCodeViewModel> _inviteCodes = [];
    private bool _isLoading = true;
    private bool _isDeleting;
    private bool _isLeaving;

    protected override async Task OnParametersSetAsync()
    {
        await LoadGroupAsync();
    }

    private async Task LoadGroupAsync()
    {
        _isLoading = true;
        StateHasChanged();
        
        try
        {
            // Charger les données en parallèle
            var groupTask = GroupsService.GetByIdAsync(GroupId);
            var membersTask = GroupsService.GetMembersAsync(GroupId);
            
            await Task.WhenAll(groupTask, membersTask);
            
            _group = await groupTask;
            _members = await membersTask;
            
            // Charger les codes d'invitation si propriétaire
            if (_group?.IsOwner == true)
            {
                _inviteCodes = await GroupsService.GetInviteCodesAsync(GroupId);
            }
        }
        catch (Exception)
        {
            Snackbar.Add("Erreur lors du chargement du groupe", Severity.Error);
            _group = null;
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task RemoveMemberAsync(GroupMemberViewModel member)
    {
        var result = await DialogService.ShowMessageBox(
            "Confirmer",
            $"Voulez-vous vraiment retirer {member.DisplayName} du groupe ?",
            yesText: "Retirer",
            cancelText: "Annuler");
        
        if (result == true && member.UserId != null)
        {
            try
            {
                await GroupsService.RemoveMemberAsync(GroupId, member.UserId);
                _members = await GroupsService.GetMembersAsync(GroupId);
                Snackbar.Add("Membre retiré du groupe", Severity.Success);
            }
            catch (Exception)
            {
                Snackbar.Add("Erreur lors du retrait du membre", Severity.Error);
            }
            StateHasChanged();
        }
    }

    private async Task CreateInviteCodeAsync()
    {
        var parameters = new DialogParameters<CreateInviteCodeDialog>
        {
            { x => x.GroupId, GroupId }
        };
        
        var dialog = await DialogService.ShowAsync<CreateInviteCodeDialog>(
            "Créer un code d'invitation", 
            parameters);
        
        var result = await dialog.Result;
        
        if (result is { Canceled: false })
        {
            _inviteCodes = await GroupsService.GetInviteCodesAsync(GroupId);
            Snackbar.Add("Code d'invitation créé", Severity.Success);
            StateHasChanged();
        }
    }

    private async Task CopyCodeAsync(string? code)
    {
        if (string.IsNullOrEmpty(code)) return;
        
        try
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", code);
            Snackbar.Add("Code copié dans le presse-papiers", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Impossible de copier le code", Severity.Error);
        }
    }

    private async Task RevokeCodeAsync(GroupInviteCodeViewModel code)
    {
        var codeId = code.Id;
        if (!codeId.HasValue) return;
        
        try
        {
            await GroupsService.RevokeInviteCodeAsync(GroupId, codeId.Value);
            _inviteCodes = await GroupsService.GetInviteCodesAsync(GroupId);
            Snackbar.Add("Code d'invitation révoqué", Severity.Success);
        }
        catch (Exception)
        {
            Snackbar.Add("Erreur lors de la révocation", Severity.Error);
        }
        StateHasChanged();
    }

    private async Task DeleteGroupAsync()
    {
        var result = await DialogService.ShowMessageBox(
            "Supprimer le groupe",
            $"Voulez-vous vraiment supprimer le groupe \"{_group?.Name}\" ? Cette action est irréversible.",
            yesText: "Supprimer",
            cancelText: "Annuler");
        
        if (result == true)
        {
            _isDeleting = true;
            StateHasChanged();
            
            try
            {
                await GroupsService.DeleteAsync(GroupId);
                Snackbar.Add("Groupe supprimé", Severity.Success);
                Navigation.NavigateTo("/groups");
            }
            catch (Exception)
            {
                Snackbar.Add("Erreur lors de la suppression du groupe", Severity.Error);
                _isDeleting = false;
                StateHasChanged();
            }
        }
    }

    private async Task LeaveGroupAsync()
    {
        var result = await DialogService.ShowMessageBox(
            "Quitter le groupe",
            $"Voulez-vous vraiment quitter le groupe \"{_group?.Name}\" ?",
            yesText: "Quitter",
            cancelText: "Annuler");
        
        if (result == true)
        {
            _isLeaving = true;
            StateHasChanged();
            
            try
            {
                await GroupsService.LeaveAsync(GroupId);
                Snackbar.Add("Vous avez quitté le groupe", Severity.Success);
                Navigation.NavigateTo("/groups");
            }
            catch (Exception)
            {
                Snackbar.Add("Erreur lors de la sortie du groupe", Severity.Error);
                _isLeaving = false;
                StateHasChanged();
            }
        }
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/groups");
    }
    
    private static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "?";
        
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..Math.Min(2, parts[0].Length)].ToUpper(),
            _ => $"{parts[0][0]}{parts[^1][0]}".ToUpper()
        };
    }
    
    private static string GetRoleDisplayName(int? role)
    {
        return role switch
        {
            0 => "Propriétaire",
            1 => "Membre",
            _ => "Membre"
        };
    }
}
