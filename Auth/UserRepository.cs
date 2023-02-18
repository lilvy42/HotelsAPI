public class UserRepository : IUserRepository
{
    public List<UserDto> _users = new()
    {
        new UserDto("dubita", "balota"),
        new UserDto("nagets", "CUMstantin"),
        new UserDto("шылкин", "г даске")
    };
    public UserDto GetUser(UserModel userModel) => 
        _users.FirstOrDefault(u =>
            string.Equals(u.UserName, userModel.UserName) &&
            string.Equals(u.Password, userModel.Password)) ??
            throw new Exception();
}