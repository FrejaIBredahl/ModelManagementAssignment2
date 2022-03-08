using Microsoft.EntityFrameworkCore;
using ModelManagementAssignment2.Models;

namespace ModelManagementAssignment2.Data
{
    public class ModelManagementDb : DbContext 
    {
        public ModelManagementDb(DbContextOptions<ModelManagementDb> options) : base(options) { }

        public DbSet<Model> Models => Set<Model>(); 

        public DbSet<Job> Jobs => Set<Job>();

        public DbSet<Expense> Expenses => Set<Expense>();
    }
}
