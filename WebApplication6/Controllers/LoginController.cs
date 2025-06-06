using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication6.Models;


namespace WebApplication6.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UserModel user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Message = "Username and Password are required.";
                return View();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password); 

                    con.Open();
                    var role = cmd.ExecuteScalar();
                    con.Close();

                    if (role != null)
                    {
                        TempData["Username"] = user.Username;
                        TempData["Role"] = role.ToString();
                        TempData.Keep();
                        return RedirectToAction("Add", "Employee");
                    }
                }
            }

            ViewBag.Message = "Invalid login.";
            ViewBag.Role = TempData["Role"];
            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            TempData.Clear();  
            return RedirectToAction("Index", "Login");
        }

        public IActionResult TestDbConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    ViewBag.Message = "Database connection is successful!";
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Database connection failed: " + ex.Message;
            }

            return View();
        }
    }
}
