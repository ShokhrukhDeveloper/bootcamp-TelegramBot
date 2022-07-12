using bot.Data;
using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Services;
public class UserService
{
    private readonly BotDbContext _context;

    public UserService(
        BotDbContext context        
    )
    {
        _context=context?? throw new ArgumentNullException(nameof(BotDbContext));
        
    }
    
    public async Task<User?> GetUserAsync(long? userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(_context.Users);
        var result=await _context.Users.FindAsync(userId);
        return result;
        
    }
    public async Task<string?> GetUserLanguageCodeAsync(long? userId)
    {
        var user=await GetUserAsync(userId);
        return user?.LanguageCode;


    }
    public async Task<(bool isSuccess,string?  ErrorMEssage)> UpdateUserLanguageCode(long? userId,string languageCode)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var user=await GetUserAsync(userId);
        if (user is null)
        {
            return (false,"User not Found");
            
        }
        user.LanguageCode=languageCode;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return (true,null);
        
    }
    public async Task<(bool IsSuccess,string? ErrorMEssage)> AddUserAsync(User user)
    {
        
        if (await Exits(user.UserId) )
        return (false,"User Exists");
        try
        {
             await _context.Users.AddAsync(user);
             await _context.SaveChangesAsync();
             return (true,null);
        }
        catch (Exception e)
        {
            
            return (false,e.Message);
        }
       
    }
    public async Task<bool> Exits(long userId)
        =>await _context.Users.AnyAsync(u=>u.UserId==userId);

}