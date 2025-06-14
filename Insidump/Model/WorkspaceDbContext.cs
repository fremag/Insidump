using Microsoft.EntityFrameworkCore;

namespace Insidump.Model;

public class WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : DbContext(options)
{
    public DbSet<ClrTypeInfo> ClrTypeInfos { get; set; }
    public DbSet<ClrValueInfo> ClrValueInfos { get; set; }
}