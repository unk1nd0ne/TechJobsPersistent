using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        [HttpGet("/Add")]
        public IActionResult AddJob()
        {
            AddJobViewModel addJobViewModel = new AddJobViewModel(context.Employers.ToList(), context.Skills.ToList());

            return View(addJobViewModel);
        }

        [HttpPost]
        [Route("/Add")]
        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel, string[] selectedSkills)
        {
            if (ModelState.IsValid)
            {
                List<JobSkill> jobSkills = new List<JobSkill>();

                Employer employer = context.Employers.Find(addJobViewModel.EmployerId);
                Job newJob = new Job
                {
                    Name = addJobViewModel.Name,
                    EmployerId = addJobViewModel.EmployerId,
                    Employer = employer
                };

                foreach (string skill in selectedSkills)
                {
                    Skill theSkill = context.Skills
                        .Single(s => s.Name == skill);

                    JobSkill newJobSkill = new JobSkill
                    {
                        JobId = newJob.Id,
                        Job = newJob,
                        SkillId = theSkill.Id,
                        Skill = theSkill
                    };
                    jobSkills.Add(newJobSkill);
                    context.JobSkills.Add(newJobSkill);
                };

                newJob.JobSkills = jobSkills;

                context.Jobs.Add(newJob);
                context.SaveChanges();

                return Redirect("Home");
            }            

            return View("Add", addJobViewModel);
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }
    }
}
