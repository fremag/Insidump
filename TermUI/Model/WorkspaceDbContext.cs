using Microsoft.EntityFrameworkCore;

namespace TermUI.Model;

public class WorkspaceDbContext : DbContext
{
    public WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : base(options)
    {
    }

    public DbSet<ClrTypeInfo> ClrTypeInfos { get; set; }
    public DbSet<ClrValueInfo> ClrValueInfos { get; set; }
}