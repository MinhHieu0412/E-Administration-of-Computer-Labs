using E_Administration.Models;
using Microsoft.EntityFrameworkCore;
using E_Administration.Dto;

namespace E_Administration.Data
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Equipments> Equipments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<IssueReports> IssueReports { get; set; }
        public DbSet<LabRequests> LabRequests { get; set; }
        public DbSet<Elearnings> ELearning { get; set; }
        public DbSet<RepairAssignments> RepairAssignments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<MakeUpRequest> MakeUpRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveRequest>()
               .HasOne(lr => lr.User)
               .WithMany(u => u.LeaveRequests)
               .HasForeignKey(lr => lr.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            // User -> Department (Many-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentID);

            // Lab -> Department (Many-to-One)
            modelBuilder.Entity<Lab>()
                .HasOne(l => l.Department)
                .WithMany(d => d.Labs)
                .HasForeignKey(l => l.DepartmentID);

            // Lab -> Equipments(One-to-many)
            modelBuilder.Entity<Lab>()
     .HasMany(l => l.Equipments)
     .WithOne(ld => ld.Lab) // Sử dụng biểu thức lambda hợp lệ
     .HasForeignKey(ld => ld.LabID);


            // Assignment -> Lab (Many-to-One)
            modelBuilder.Entity<Assignments>()
    .HasOne(a => a.Lab)
    .WithMany(l => l.Assignments)
    .HasForeignKey(a => a.LabID)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignments>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // IssueReport -> Lab (Many-to-One)
            modelBuilder.Entity<IssueReports>()
     .HasOne(ir => ir.Lab)
     .WithMany(l => l.IssueReports)
     .HasForeignKey(ir => ir.LabID)
     .OnDelete(DeleteBehavior.NoAction);  // Ngừng cascade cho khóa ngoại LabID

            modelBuilder.Entity<IssueReports>()
                .HasOne(ir => ir.Reporter)
                .WithMany(u => u.IssueReports)
                .HasForeignKey(ir => ir.ReporterID)
                .OnDelete(DeleteBehavior.NoAction);  // Giữ cascade cho khóa ngoại ReporterID

            // LabRequest -> Department (Many-to-One)
            modelBuilder.Entity<LabRequests>()
                .HasOne(lr => lr.Department)
                .WithMany(d => d.LabRequests)
                .HasForeignKey(lr => lr.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // LabRequest -> User (Many-to-One)
            modelBuilder.Entity<LabRequests>()
                .HasOne(lr => lr.RequestedBy)
                .WithMany(u => u.LabRequests)
                .HasForeignKey(lr => lr.RequestedByID)
                .OnDelete(DeleteBehavior.NoAction);

            // RepairAssignment -> IssueReport (One-to-One)
            modelBuilder.Entity<RepairAssignments>()
                .HasOne(ra => ra.IssueReports)
                .WithOne(ir => ir.RepairAssignment)
                .HasForeignKey<RepairAssignments>(ra => ra.IssueReportID)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<IssueReports>()
                .HasOne(ir => ir.Department)
                .WithMany(d => d.IssueReports)
                .HasForeignKey(ir => ir.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
}

}
