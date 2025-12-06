using GameOnTonight.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace GameOnTonight.App.Pages.Groups;

public partial class JoinGroupPage : ComponentBase
{
    /// <summary>
    /// Code d'invitation passé en paramètre d'URL.
    /// </summary>
    [Parameter]
    public string? InviteCode { get; set; }
    
    private string _inviteCode = string.Empty;
    private bool _isJoining;
    private string? _errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        // Si un code est passé en paramètre, l'utiliser et rejoindre automatiquement
        if (!string.IsNullOrWhiteSpace(InviteCode) && _inviteCode != InviteCode)
        {
            _inviteCode = InviteCode.ToUpperInvariant();
            await JoinGroupAsync();
        }
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(_inviteCode))
        {
            await JoinGroupAsync();
        }
    }

    private async Task JoinGroupAsync()
    {
        if (string.IsNullOrWhiteSpace(_inviteCode))
            return;
        
        _isJoining = true;
        _errorMessage = null;
        StateHasChanged();
        
        try
        {
            var joinedGroup = await GroupsService.JoinAsync(_inviteCode.Trim().ToUpperInvariant());
            
            Snackbar.Add($"Vous avez rejoint le groupe \"{joinedGroup.Name}\" !", Severity.Success);
            
            // Naviguer vers le détail du groupe
            var groupId = joinedGroup.Id;
            Navigation.NavigateTo($"/groups/{groupId}");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            _errorMessage = "Code d'invitation invalide ou expiré.";
            _isJoining = false;
            StateHasChanged();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("409"))
        {
            _errorMessage = "Vous êtes déjà membre de ce groupe.";
            _isJoining = false;
            StateHasChanged();
        }
        catch (Exception)
        {
            _errorMessage = "Erreur lors de l'adhésion au groupe.";
            _isJoining = false;
            StateHasChanged();
        }
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/groups");
    }
}
