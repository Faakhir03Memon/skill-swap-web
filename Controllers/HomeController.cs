using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapAI.Data;
using SkillSwapAI.Models;
using System.Security.Claims;

namespace SkillSwapAI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue("UserId") ?? "0");
            var user = await _context.Users
                .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.User = user;
            ViewBag.Matches = await GetSkillMatches(userId);
            ViewBag.Leaderboard = await _context.Results
                .Include(r => r.User)
                .OrderByDescending(r => r.Score)
                .Take(10)
                .ToListAsync();

            return View();
        }

        private async Task<List<MatchViewModel>> GetSkillMatches(int userId)
        {
            // Simple AI-like matching logic:
            // 1. Find what the user wants to learn.
            // 2. Find users who can teach those skills.
            // 3. Rank them based on their level (Expert first).

            var userWants = await _context.UserSkills
                .Where(us => us.UserId == userId && !us.IsTeaching)
                .Select(us => us.SkillId)
                .ToListAsync();

            var matches = await _context.UserSkills
                .Include(us => us.User)
                .Include(us => us.Skill)
                .Where(us => userWants.Contains(us.SkillId) && us.IsTeaching && us.UserId != userId)
                .OrderByDescending(us => us.Level) // Ranking: Experts first
                .Select(us => new MatchViewModel
                {
                    UserName = us.User!.Name,
                    SkillName = us.Skill!.Name,
                    Level = us.Level == 3 ? "Expert" : (us.Level == 2 ? "Intermediate" : "Beginner"),
                    Email = us.User.Email
                })
                .ToListAsync();

            return matches;
        }

        [HttpGet]
        public async Task<IActionResult> Exams()
        {
            var exams = await _context.Exams.Include(e => e.Skill).ToListAsync();
            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> TakeExam(int id)
        {
            var exam = await _context.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == id);
            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, IFormCollection form)
        {
            var userId = int.Parse(User.FindFirstValue("UserId") ?? "0");
            var questions = await _context.Questions.Where(q => q.ExamId == examId).ToListAsync();
            int score = 0;

            foreach (var q in questions)
            {
                var answer = form["q_" + q.Id];
                if (answer == q.CorrectOption) score++;
            }

            var result = new Result
            {
                UserId = userId,
                ExamId = examId,
                Score = score,
                TotalQuestions = questions.Count
            };

            _context.Results.Add(result);

            // Certificate logic: Pass if >= 70%
            if (questions.Count > 0 && (double)score / questions.Count >= 0.7)
            {
                var exam = await _context.Exams.FindAsync(examId);
                var cert = new Certificate
                {
                    UserId = userId,
                    SkillId = exam!.SkillId,
                    CertificateNumber = "CERT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    IssuedAt = DateTime.Now
                };
                _context.Certificates.Add(cert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

    public class MatchViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
