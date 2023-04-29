namespace HabitTrackerAPI.Infrastructure;

public class TokenService : ITokenService
{
    /// <summary>
    /// Replace this with a functionality that will retrieve the user id from the provided token 
    /// </summary>
    /// <returns></returns>
    public string GetUserId()
    {
        return Guid.Empty.ToString();
    }
}