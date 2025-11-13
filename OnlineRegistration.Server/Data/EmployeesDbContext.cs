//using Microsoft.EntityFrameworkCore;
//using OnlineRegistration.Server.Models;

//namespace OnlineRegistration.Server.Data
//{
//    public class EmployeesDbContext : DbContext
//    {
//        public EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) : base(options) { }
//        public DbSet<PersonalInformation> PersonalInformations { get; set; }
//        public DbSet<CivilServiceEligibility> CivilServiceEligibilities { get; set; }
//        public DbSet<VoluntaryWork> VoluntaryWorks { get; set; }
//        public DbSet<Training> Trainings { get; set; }
//        public DbSet<SkillHobby> SkillsHobbies { get; set; }
//        public DbSet<Distinction> Distinctions { get; set; }
//        public DbSet<Membership> Memberships { get; set; }
//        public DbSet<EmployeeIDApplication> EmployeeIDApplications { get; set; }

//        public DbSet<Gender> Gender { get; set; }
//        public DbSet<CivilStatus> CivilStatus { get; set; }

//        public DbSet<Department> Department { get; set; }

//        public DbSet<EmployeeAttendance> Attendance { get; set; }


//        // 🔑 Add this method
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<EmployeeAttendance>().ToTable("EmployeeAttendance");
//            // c1
//            modelBuilder.Entity<PersonalInformation>().ToTable("PersonalInformation");
//            modelBuilder.Entity<Child>().ToTable("PersonalInformation_Children");
//            modelBuilder.Entity<Education>().ToTable("PersonalInformation_Education");
//            // c2
//            modelBuilder.Entity<CivilServiceEligibility>().ToTable("PersonalInformation_CivilServiceEligibility");
//            // c3
//            modelBuilder.Entity<VoluntaryWork>().ToTable("PersonalInformation_VoluntaryWork");
//            modelBuilder.Entity<Training>().ToTable("PersonalInformation_Training");
//            modelBuilder.Entity<SkillHobby>().ToTable("PersonalInformation_SkillsHobbies");
//            modelBuilder.Entity<Distinction>().ToTable("PersonalInformation_Distinctions");
//            modelBuilder.Entity<Membership>().ToTable("PersonalInformation_Memberships");



//            modelBuilder.Entity<Gender>().ToTable("Gender");
//            modelBuilder.Entity<CivilStatus>().ToTable("CivilStatus");

//            modelBuilder.Entity<Employee>().ToTable("Employee");

//            // Explicitly set primary keys
//            modelBuilder.Entity<CivilServiceEligibility>().HasKey(e => e.EligibilityID);
//            modelBuilder.Entity<PersonalInformation>().HasKey(p => p.PersonID);
//            modelBuilder.Entity<Gender>().HasKey(r => r.GenderID);
//            modelBuilder.Entity<CivilStatus>().HasKey(r => r.CivilStatusID);
//            modelBuilder.Entity<VoluntaryWork>().HasKey(v => v.WorkId);
//            modelBuilder.Entity<Training>().HasKey(t => t.TrainingId);
//            modelBuilder.Entity<SkillHobby>().HasKey(s => s.SkillId);
//            modelBuilder.Entity<Distinction>().HasKey(d => d.DistinctionId);
//            modelBuilder.Entity<Membership>().HasKey(m => m.MembershipId);
//            modelBuilder.Entity<Employee>().HasKey(p => p.PersonID);

//            modelBuilder.Entity<BiometricDevice>().HasKey(p => p.BiometricDeviceID);
//            modelBuilder.Entity<EmployeeAttendance>().HasKey(p => new {p.EmployeeID,p.BiometricDeviceID,p.DateLog});

//            // Relationships
//            modelBuilder.Entity<EmployeeAttendance>()
//                .HasOne(a => a.BiometricDevice)
//                .WithMany()
//                .HasForeignKey(a => a.BiometricDeviceID);
//            modelBuilder.Entity<BiometricDevice>()
//                .HasOne(d => d.Department)
//                .WithMany()
//                .HasForeignKey(d => d.DepartmenID);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.Children)
//                .WithOne(c => c.Person)
//                .HasForeignKey(c => c.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.EducationRecords)
//                .WithOne(e => e.Person)
//                .HasForeignKey(e => e.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.CivilServiceEligibilities)
//                .WithOne(e => e.Person)
//                .HasForeignKey(e => e.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.VoluntaryWorks)
//                .WithOne(v => v.Person)
//                .HasForeignKey(v => v.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.Trainings)
//                .WithOne(t => t.Person)
//                .HasForeignKey(t => t.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.SkillsHobbies)
//                .WithOne(s => s.Person)
//                .HasForeignKey(s => s.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.Distinctions)
//                .WithOne(d => d.Person)
//                .HasForeignKey(d => d.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<PersonalInformation>()
//                .HasMany(p => p.Memberships)
//                .WithOne(m => m.Person)
//                .HasForeignKey(m => m.PersonID)
//                .OnDelete(DeleteBehavior.Cascade);



//            base.OnModelCreating(modelBuilder);
//        }

//    }
//}
