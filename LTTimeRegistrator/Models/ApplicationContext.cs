using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
  /// <summary>
  /// 
  /// </summary>
  public class ApplicationContext : DbContext
  {
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
      : base(options) { }

    public DbSet<User> Users { get; set; }
  }
}