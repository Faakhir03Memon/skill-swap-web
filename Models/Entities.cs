using System.ComponentModel.DataAnnotations;

namespace SkillSwapAI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Student"; // Student or Admin

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }

    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UserSkill
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public int SkillId { get; set; }
        public Skill? Skill { get; set; }

        public bool IsTeaching { get; set; } // True if user can teach, False if wants to learn
        public int Level { get; set; } // 1: Beginner, 2: Intermediate, 3: Expert
    }

    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty; // A, B, C, or D
    }

    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.Now;
    }

    public class Certificate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.Now;
    }
}
