using bot.Entity;
using Microsoft.EntityFrameworkCore;

namespace bot.Data;
public class BotDbContext:DbContext
{
    public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options) 
    { 
        
    }
    public DbSet<User> Users { get; set; }
     
}