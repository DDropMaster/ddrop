using System.Threading.Tasks;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.User
{
    public class UserBl : IUserBl
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public UserBl(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task<BE.Models.User> GetUserByLogin(string userEmail)
        {
            var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));

            return _mapper.Map<DbUser, BE.Models.User>(dbUser);
        }

        public void CreateUser(BE.Models.User userToCreate)
        {
            _dDropRepository.CreateUserAsync(_mapper.Map<BE.Models.User, DbUser>(userToCreate));
        }

        public void UpdateUserAsync(BE.Models.User dbUserForUpdate)
        {
            _dDropRepository.UpdateUserAsync(_mapper.Map<BE.Models.User, DbUser>(dbUserForUpdate));
        }
    }
}