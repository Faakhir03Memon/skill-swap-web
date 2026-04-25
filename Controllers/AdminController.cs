using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwapAI.Data;
using SkillSwapAI.Models;

namespace SkillSwapAI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.SkillCount = await _context.Skills.CountAsync();
            ViewBag.ExamCount = await _context.Exams.CountAsync();
            ViewBag.CertificateCount = await _context.Certificates.CountAsync();
            
            var latestUsers = await _context.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToListAsync();
            return View(latestUsers);
        }

        // Skill Management
        public async Task<IActionResult> Skills()
        {
            return View(await _context.Skills.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction("Skills");
        }

        // Exam Management
        public async Task<IActionResult> Exams()
        {
            return View(await _context.Exams.Include(e => e.Skill).ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateExam()
        {
            ViewBag.Skills = _context.Skills.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam(Exam exam)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction("Exams");
        }

        [HttpGet]
        public async Task<IActionResult> ManageQuestions(int id)
        {
            var exam = await _context.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == id);
            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageQuestions", new { id = question.ExamId });
        }
    }
}
