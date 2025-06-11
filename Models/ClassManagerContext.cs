using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;

namespace ProjektCRUD20510.Models
{
    public class ClassManagerContext : DbContext
    {
        public ClassManagerContext(DbContextOptions<ClassManagerContext> options) : base(options)
        {
        }

        public DbSet<Class> Uzytkownik { get; set; }
        public DbSet<Kategorie> Kategorie { get; set; }
        public DbSet<Testy> Testy { get; set; }
        public DbSet<Fiszki> Fiszki { get; set; }
        public DbSet<Wynik> Wynik { get; set; }
        public DbSet<Testy_Uzytkownik> Testy_Uzytkownik { get; set; }
        public DbSet<Testy_Fiszki> Testy_Fiszki { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kategorie
            modelBuilder.Entity<Kategorie>()
                .ToTable("Kategorie")
                .Property(k => k.Id).HasColumnName("id");
            modelBuilder.Entity<Kategorie>()
                .Property(k => k.Nazwa).HasColumnName("nazwa");

            // Uzytkownik
            modelBuilder.Entity<Class>()
                .ToTable("Uzytkownik")
                .Property(u => u.Id).HasColumnName("id");
            modelBuilder.Entity<Class>()
                .Property(u => u.Nazwa).HasColumnName("nazwa");
            modelBuilder.Entity<Class>()
                .Property(u => u.Haslo).HasColumnName("haslo");
            modelBuilder.Entity<Class>()
                .Property(u => u.Role).HasColumnName("role");
            // Testy
            modelBuilder.Entity<Testy>()
                .ToTable("Testy")
                .Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<Testy>()
                .Property(t => t.Id_Kategorii).HasColumnName("id_kategorii");
            modelBuilder.Entity<Testy>()
                .Property(t => t.NazwaTestu).HasColumnName("nazwaTestu");
            modelBuilder.Entity<Testy>()
                .HasOne(t => t.Kategoria)
                .WithMany()
                .HasForeignKey(t => t.Id_Kategorii)
                .OnDelete(DeleteBehavior.Cascade);

            // Fiszki
            modelBuilder.Entity<Fiszki>()
                .ToTable("Fiszki")
                .Property(f => f.Id).HasColumnName("id");
            modelBuilder.Entity<Fiszki>()
                .Property(f => f.Id_Uzytkownika).HasColumnName("id_uzytkownika");
            modelBuilder.Entity<Fiszki>()
                .Property(f => f.Nazwa_PL).HasColumnName("nazwa_PL");
            modelBuilder.Entity<Fiszki>()
                .Property(f => f.Nazwa_ENGLISH).HasColumnName("nazwa_ENGLISH");
            modelBuilder.Entity<Fiszki>()
                .HasOne(f => f.Uzytkownik)
                .WithMany()
                .HasForeignKey(f => f.Id_Uzytkownika)
                .OnDelete(DeleteBehavior.Cascade);

            // Wynik
            modelBuilder.Entity<Wynik>()
                .ToTable("Wynik")
                .Property(w => w.Id).HasColumnName("id");
            modelBuilder.Entity<Wynik>()
                .Property(w => w.Id_Testu).HasColumnName("id_testu");
            modelBuilder.Entity<Wynik>()
                .Property(w => w.Id_Uzytkownika).HasColumnName("id_uzytkownika");
            modelBuilder.Entity<Wynik>()
                .Property(w => w.Id_Kategorii).HasColumnName("id_kategorii");
            modelBuilder.Entity<Wynik>()
                .Property(w => w.wynik).HasColumnName("wynik");
            modelBuilder.Entity<Wynik>()
                .HasOne(w => w.Test)
                .WithMany()
                .HasForeignKey(w => w.Id_Testu)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Wynik>()
                .HasOne(w => w.Uzytkownik)
                .WithMany()
                .HasForeignKey(w => w.Id_Uzytkownika)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Wynik>()
                .HasOne(w => w.Kategoria)
                .WithMany()
                .HasForeignKey(w => w.Id_Kategorii)
                .OnDelete(DeleteBehavior.Restrict);

            // Testy_Uzytkownik
            modelBuilder.Entity<Testy_Uzytkownik>()
                .ToTable("Testy_Uzytkownik")
                .HasKey(tu => new { tu.Id_Testu, tu.Id_Uzytkownika });
            modelBuilder.Entity<Testy_Uzytkownik>()
                .Property(tu => tu.Id_Testu).HasColumnName("id_testu");
            modelBuilder.Entity<Testy_Uzytkownik>()
                .Property(tu => tu.Id_Uzytkownika).HasColumnName("id_uzytkownik");
            modelBuilder.Entity<Testy_Uzytkownik>()
                .HasOne(tu => tu.Test)
                .WithMany()
                .HasForeignKey(tu => tu.Id_Testu)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Testy_Uzytkownik>()
                .HasOne(tu => tu.Uzytkownik)
                .WithMany()
                .HasForeignKey(tu => tu.Id_Uzytkownika)
                .OnDelete(DeleteBehavior.Cascade);

            // Testy_Fiszki
            modelBuilder.Entity<Testy_Fiszki>()
                .HasKey(tf => new { tf.Id_Testu, tf.Id_Fiszki }); // Composite primary key

            modelBuilder.Entity<Testy_Fiszki>()
                .HasOne(tf => tf.Test)
                .WithMany(t => t.Testy_Fiszki)
                .HasForeignKey(tf => tf.Id_Testu);

            modelBuilder.Entity<Testy_Fiszki>()
                .HasOne(tf => tf.Fiszka)
                .WithMany() // No inverse navigation in Fiszki
                .HasForeignKey(tf => tf.Id_Fiszki);
        }
    }
}