using Mediator;
using Microsoft.AspNetCore.Identity;

namespace GameOnTonight.Application.Auth.Commands;

public record RegisterUserCommand(string Email, string Password, string ConfirmPassword) : IRequest;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterUserCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async ValueTask<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        
        return Unit.Value;
    }
}