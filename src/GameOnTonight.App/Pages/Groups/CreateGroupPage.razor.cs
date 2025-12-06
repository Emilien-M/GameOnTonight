using System.ComponentModel.DataAnnotations;
using GameOnTonight.App.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GameOnTonight.App.Pages.Groups;

public partial class CreateGroupPage : ComponentBase
{
    private readonly CreateGroupModel _model = new();
    private bool _isSubmitting;

    private async Task HandleSubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(_model.Name))
            return;
        
        _isSubmitting = true;
        StateHasChanged();
        
        try
        {
            var createdGroup = await GroupsService.CreateAsync(_model.Name, _model.Description);
            
            Snackbar.Add($"Groupe \"{createdGroup.Name}\" créé avec succès !", Severity.Success);
            
            // Naviguer vers le détail du groupe créé
            var groupId = createdGroup.Id;
            Navigation.NavigateTo($"/groups/{groupId}");
        }
        catch (Exception)
        {
            Snackbar.Add("Erreur lors de la création du groupe", Severity.Error);
            _isSubmitting = false;
            StateHasChanged();
        }
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/groups");
    }

    private class CreateGroupModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
    }
}
