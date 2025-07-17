using Microsoft.EntityFrameworkCore;
using SecurityPlusPractice.Models;

namespace SecurityPlusPractice.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Question> Questions { get; set; }
		public DbSet<Choice> Choices { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<Explanation> Explanations { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<SessionAnswer> SessionAnswers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Choice>()
				.HasOne<Question>()
				.WithOne()
				.HasForeignKey<Choice>(c => c.ID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Answer>()
				.HasOne<Question>()
				.WithOne()
				.HasForeignKey<Answer>(a => a.ID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Explanation>()
				.HasOne<Question>()
				.WithOne()
				.HasForeignKey<Explanation>(e => e.ID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SessionAnswer>()
				.HasOne<Session>()
				.WithMany()
				.HasForeignKey(sa => sa.SessionID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SessionAnswer>()
				.HasOne<Question>()
				.WithMany()
				.HasForeignKey(sa => sa.QuestionID)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
