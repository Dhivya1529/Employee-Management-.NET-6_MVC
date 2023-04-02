using Microsoft.AspNetCore.Mvc;
using FirstProject.Models;
using FirstProject.Models.Domain;
using FirstProject.Data;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext appDbContext;

        public EmployeeController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var employee = await appDbContext.Employees.ToListAsync();
            //parsing employee details from database to the view
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DOB = addEmployeeRequest.DOB
            };
            //Stores Employee Details to the Database
            await appDbContext.Employees.AddAsync(employee);
            await appDbContext.SaveChangesAsync();

            //Redirect to Add Action
            return RedirectToAction("Add");
        }
        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await appDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DOB = employee.DOB
                };

                return await Task.Run(()=>View("View",viewModel));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel viewModel)
        {
            var employee = await appDbContext.Employees.FindAsync(viewModel.Id);
            if (employee!=null)
            {
                employee.Name = viewModel.Name;
                employee.Salary = viewModel.Salary;
                employee.Email = viewModel.Email;
                employee.Salary = viewModel.Salary;
                employee.Department = viewModel.Department;
                employee.DOB = viewModel.DOB;

            }
            await appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel viewModel)
        {
            var employee = await appDbContext.Employees.FindAsync(viewModel.Id);
            if (employee!=null) 
            {
                appDbContext.Employees.Remove(employee);
                await appDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
