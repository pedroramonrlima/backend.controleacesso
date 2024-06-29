using ControleAcesso.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleAcesso.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<EmployeeStatus> EmployeeStatuses { get; set; }
        public DbSet<GroupApproval> GroupAprovall { get; set; }
        public DbSet<GroupAd> GroupAds { get; set; }
        public DbSet<AcesseRequest> AcesseRequests { get; set; }
        public DbSet<AcesseRequestDetail> AcesseRequestDetails { get; set; }
        public DbSet<PriorApproval> PriorApprovals { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Cpf)
                      .HasMaxLength(45);

                entity.Property(e => e.Registration)
                      .HasMaxLength(45);

                entity.Property(e => e.Name)
                      .HasMaxLength(45);

                entity.Property(e => e.BomDate)
                      .HasColumnType("date");

                entity.Property(e => e.ContractDate)
                      .HasColumnType("date");

                entity.HasOne(e => e.Office)
                      .WithMany(o => o.Employees)
                      .HasForeignKey(e => e.OfficeId);

                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId);

                entity.HasOne(e => e.EmployeeStatus)
                      .WithMany(es => es.Employees)
                      .HasForeignKey(e => e.EmployeeStatusId);                
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.HasKey(o => o.Id);

                entity.Property(o => o.Name)
                      .HasMaxLength(45);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasKey(d => d.Id);

                entity.Property(d => d.Name)
                      .HasMaxLength(45);

                entity.HasOne(d => d.Manager)
                      .WithMany()
                      .HasForeignKey(d => d.ManagerId);
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("manager");

                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Employee)
                      .WithMany()
                      .HasForeignKey(m => m.EmployeeId);
            });

            modelBuilder.Entity<EmployeeStatus>(entity =>
            {
                entity.ToTable("employee_status");

                entity.HasKey(es => es.Id);

                entity.Property(es => es.Name)
                      .HasMaxLength(45);

                entity.HasData(
                    new EmployeeStatus { Id = 1, Name = "Ativo" },
                    new EmployeeStatus { Id = 2, Name = "Desligado" },
                    new EmployeeStatus { Id = 3, Name = "Férias" }
                );
            });

            modelBuilder.Entity<GroupApproval>(entity =>
            {
                entity.ToTable("group_approvall");
                
                entity.HasKey(ga => ga.Id);

                entity.HasOne(ga => ga.Employee)
                      .WithMany()
                      .HasForeignKey(ga => ga.EmployeeId);

                entity.HasOne(ga => ga.GroupAd)
                      .WithMany()
                      .HasForeignKey(ga => ga.GroupAdId);
            });

            modelBuilder.Entity<GroupAd>(entity =>
            {
                entity.ToTable("group_ad");

                entity.HasKey(ga => ga.Id);

                entity.Property(ga => ga.Name)
                      .HasMaxLength(45);

                entity.Property(ga => ga.Dn)
                      .HasMaxLength(255);
            });

            modelBuilder.Entity<AcesseRequest>(entity =>
            {
                entity.ToTable("accesse_request");

                entity.HasKey(ar => ar.Id);

                entity.HasOne(ar => ar.Employee)
                      .WithMany(e => e.AcesseRequests)
                      .HasForeignKey(ar => ar.EmployeeId);

                entity.HasOne(ar => ar.GroupAd)
                      .WithMany()
                      .HasForeignKey(ar => ar.GroupAdId);

                entity.HasOne(e => e.RequestType)
                      .WithMany()
                      .HasForeignKey(e => e.RequestTypeId);

            });

            modelBuilder.Entity<AcesseRequestDetail>(entity =>
            {
                entity.ToTable("accesse_request_detail");

                entity.HasKey(ard => ard.Id);

                entity.HasOne(ard => ard.RequesterEmployee)
                      .WithMany(e => e.AcesseRequestDetails)
                      .HasForeignKey(ard => ard.RequesterEmployeeId);

                entity.HasOne(ard => ard.ManagerApproval)
                      .WithMany()
                      .HasForeignKey(ard => ard.ManagerApprovalId)
                      .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(ard => ard.Status)
                      .WithMany(s => s.AcesseRequestDetails)
                      .HasForeignKey(ard => ard.StatusId);
                        

                entity.HasOne(ard => ard.AcesseRequest)
                      .WithMany(ar => ar.AcesseRequestDetails)
                      .HasForeignKey(ard => ard.AcesseRequestId);
                
                entity.HasOne(ard => ard.RequesterEmployee)
                      .WithMany(e => e.AcesseRequestDetails)
                      .HasForeignKey(ard => ard.RequesterEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PriorApproval>(entity =>
            {
                entity.ToTable("prior_approval");

                entity.HasKey(pa => pa.Id);

                entity.HasOne(pa => pa.GroupApproval)
                      .WithMany()
                      .HasForeignKey(pa => pa.GroupApprovalId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pa => pa.AcesseRequestDetail)
                      .WithMany()
                      .HasForeignKey(pa => pa.AcesseRequestDetailId);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                      .HasMaxLength(45);
                entity.HasData(
                    new Status {Id=1, Name = "Aprovado"},
                    new Status {Id=2, Name = "Reprovado"},
                    new Status {Id=3, Name = "Processando"},
                    new Status {Id=4, Name = "Error" }
                );
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.ToTable("request_type");

                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.HasPriorApproval)
                      .IsRequired();

                entity.HasData(
                    new RequestType { Id = 1, HasPriorApproval = true },
                    new RequestType { Id = 2, HasPriorApproval = false }
                );
            });
        }
    }
}
