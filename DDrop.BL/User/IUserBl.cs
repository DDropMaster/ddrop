using System.Threading.Tasks;

namespace DDrop.BL.User
{
    public interface IUserBl
    {
        Task<BE.Models.User> GetUserByLogin(string userEmail);
        void CreateUser(BE.Models.User userToCreate);
        void UpdateUserAsync(BE.Models.User dbUserForUpdate);
    }
}