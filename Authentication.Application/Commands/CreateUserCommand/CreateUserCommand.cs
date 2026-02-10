using MediatR;

namespace Authentication.Application.Commands.CreateUserCommand
{
    public class CreateUserCommand : IRequest<string>
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}