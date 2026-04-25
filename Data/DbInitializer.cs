using SkillSwapAI.Models;
using BCrypt.Net;

namespace SkillSwapAI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var admin = new User
            {
                Name = "Admin",
                Email = "skill@admin.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("skill@access.com"),
                Role = "Admin"
            };

            context.Users.Add(admin);

            // Add some default skills
            var skills = new Skill[]
            {
                new Skill { Name = "C# Programming", Description = "Backend development with C#" },
                new Skill { Name = "Web Design", Description = "UI/UX and Frontend design" },
                new Skill { Name = "Data Science", Description = "Data analysis and machine learning" },
                new Skill { Name = "Digital Marketing", Description = "SEO, SEM and Social Media" }
            };

            foreach (var s in skills)
            {
                context.Skills.Add(s);
            }

            context.SaveChanges();
        }
    }
}
