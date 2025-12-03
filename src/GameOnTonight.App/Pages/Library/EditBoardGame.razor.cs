using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using GameOnTonight.App.Services;

namespace GameOnTonight.App.Pages.Library;

public partial class EditBoardGame : IDisposable
{
    [Parameter]
    public int? Id { get; set; }

    [Inject]
    private IBoardGamesService BoardGamesService { get; set; } = default!;

    [Inject]
    private IErrorService ErrorService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool IsEdit => Id.HasValue;
    private string? errorMessage;
    private bool isSubmitting;
    private IReadOnlyList<string> _gameTypes = [];
    private string? _newGameType;
    private IEnumerable<string> _filteredGameTypes = [];

    private BoardGameForm form = new();
    private EditContext _editContext = default!;
    private ValidationMessageStore _messageStore = default!;
    private CancellationTokenSource? _cts;

    protected override void OnInitialized()
    {
        _cts = new CancellationTokenSource();
        InitializeEditContext();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Load game types for autocomplete
        await LoadGameTypesAsync();
        
        if (IsEdit && Id.HasValue)
        {
            // Cancel any pending request before loading the new one
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            errorMessage = null;
            await LoadAsync(Id.Value, _cts.Token);
        }
    }

    private async Task LoadGameTypesAsync()
    {
        try
        {
            _gameTypes = await BoardGamesService.GetGameTypesAsync();
            UpdateFilteredGameTypes();
        }
        catch
        {
            _gameTypes = [];
            _filteredGameTypes = [];
        }
    }

    private void UpdateFilteredGameTypes()
    {
        if (string.IsNullOrEmpty(_newGameType))
        {
            _filteredGameTypes = _gameTypes
                .Where(x => !form.GameTypes.Contains(x, StringComparer.OrdinalIgnoreCase));
        }
        else
        {
            _filteredGameTypes = _gameTypes
                .Where(x => x.Contains(_newGameType, StringComparison.InvariantCultureIgnoreCase))
                .Where(x => !form.GameTypes.Contains(x, StringComparer.OrdinalIgnoreCase));
        }
    }

    private void AddGameType()
    {
        if (!string.IsNullOrWhiteSpace(_newGameType) && 
            !form.GameTypes.Contains(_newGameType, StringComparer.OrdinalIgnoreCase))
        {
            form.GameTypes.Add(_newGameType.Trim());
            _newGameType = null;
            UpdateFilteredGameTypes();
        }
    }

    private void SelectGameType(string gameType)
    {
        if (!form.GameTypes.Contains(gameType, StringComparer.OrdinalIgnoreCase))
        {
            form.GameTypes.Add(gameType);
            _newGameType = null;
            UpdateFilteredGameTypes();
        }
    }

    private void RemoveGameType(string gameType)
    {
        form.GameTypes.Remove(gameType);
        UpdateFilteredGameTypes();
    }

    private void OnGameTypeKeyUp(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            AddGameType();
        }
        else
        {
            UpdateFilteredGameTypes();
        }
    }

    private void InitializeEditContext()
    {
        _editContext = new EditContext(form);
        _messageStore = new ValidationMessageStore(_editContext);
        _editContext.OnFieldChanged += (_, __) => { /* keep messages unless changed explicitly */ };
    }

    private async Task LoadAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var vm = await BoardGamesService.GetByIdAsync(id, cancellationToken);
            if (vm != null)
            {
                form = new BoardGameForm
                {
                    Name = vm.Name ?? string.Empty,
                    MinPlayers = vm.MinPlayers ?? 1,
                    MaxPlayers = vm.MaxPlayers ?? Math.Max(vm.MinPlayers ?? 1, 1),
                    DurationMinutes = vm.DurationMinutes ?? 5,
                    GameTypes = vm.GameTypes?.ToList() ?? []
                };
                InitializeEditContext();
                ClearServerErrors();
            }
        }
        catch (Exception ex)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return; // ignore cancelled operations
            }
            errorMessage = ErrorService.GetErrorMessage(ex);
        }
    }

    private async Task OnSubmitAsync()
    {
        isSubmitting = true;
        errorMessage = null;
        ClearServerErrors();
        try
        {
            if (IsEdit)
            {
                var request = new GameOnTonight.RestClient.Models.UpdateBoardGameCommand
                {
                    Id = Id,
                    Name = form.Name,
                    MinPlayers = form.MinPlayers,
                    MaxPlayers = form.MaxPlayers,
                    DurationMinutes = form.DurationMinutes,
                    GameTypes = form.GameTypes
                };
                await BoardGamesService.UpdateAsync(Id!.Value, request, _cts?.Token ?? CancellationToken.None);
            }
            else
            {
                var request = new GameOnTonight.RestClient.Models.CreateBoardGameCommand
                {
                    Name = form.Name,
                    MinPlayers = form.MinPlayers,
                    MaxPlayers = form.MaxPlayers,
                    DurationMinutes = form.DurationMinutes,
                    GameTypes = form.GameTypes
                };
                await BoardGamesService.CreateAsync(request, _cts?.Token ?? CancellationToken.None);
            }

            Navigation.NavigateTo("/library", forceLoad: true);
        }
        catch (Exception ex)
        {
            if (_cts?.IsCancellationRequested == true)
            {
                return;
            }

            // Map field-level errors
            var fieldErrors = ErrorService.GetFieldErrors(ex);
            if (fieldErrors.Count > 0)
            {
                foreach (var kvp in fieldErrors)
                {
                    var fieldId = new FieldIdentifier(form, kvp.Key);
                    _messageStore.Add(fieldId, kvp.Value);
                }
                _editContext.NotifyValidationStateChanged();
                // Avoid duplicating global message if field-level details exist
                errorMessage = null;
            }
            else
            {
                errorMessage = ErrorService.GetErrorMessage(ex);
            }
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void ClearServerErrors()
    {
        _messageStore.Clear();
        _editContext.NotifyValidationStateChanged();
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/library");
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private sealed class BoardGameForm : IValidatableObject
    {
        [Required(ErrorMessage = "Le nom est requis")] 
        [StringLength(200, ErrorMessage = "Le nom est trop long (max 200)")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 99, ErrorMessage = "Minimum 1 joueur")] 
        public int MinPlayers { get; set; } = 1;

        [Range(1, 99, ErrorMessage = "Maximum 99 joueurs")] 
        public int MaxPlayers { get; set; } = 4;

        [Range(1, 600, ErrorMessage = "Durée invalide")] 
        public int DurationMinutes { get; set; } = 30;

        public List<string> GameTypes { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxPlayers < MinPlayers)
            {
                yield return new ValidationResult("Le nombre maximum de joueurs doit être supérieur ou égal au minimum.", new[] { nameof(MaxPlayers) });
            }
        }
    }
}
