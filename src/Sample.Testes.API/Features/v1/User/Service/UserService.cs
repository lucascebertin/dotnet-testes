namespace Sample.Testes.API.Features.v1.User.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using User.Model;

    public interface IUserService 
    {
        Task<List<UserModel>> GetUsers();
    }

    public class UserService : IUserService
    {
        public readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger) =>
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));

        public Task<List<UserModel>> GetUsers() =>
            Task.FromResult(new List<UserModel> 
            {
                new UserModel(1, "user 1", 25),
                new UserModel(2, "user 2", 25),
                new UserModel(3, "user 3", 25),
            });
    }
}

