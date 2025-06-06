using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Role = TempData["Role"];
            ViewBag.EmployeeList = GetAllEmployees();
            TempData.Keep();
            return View(new EmployeeModel() { DateOfJoin = DateTime.Today });
        }


        [HttpPost]
        public IActionResult Add(EmployeeModel emp)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query;

                // Check if the record exists
                string checkQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeCode = @EmployeeCode";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@EmployeeCode", emp.EmployeeCode);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Update existing record
                        query = @"UPDATE Employees SET 
                            BranchName = @BranchName,
                            Department = @Department,
                            Area = @Area,
                            Email = @Email,
                            Qualification = @Qualification,
                            EmployeeName = @EmployeeName,
                            City = @City,
                            Gender = @Gender,
                            Designation = @Designation,
                            MobileNo1 = @MobileNo1,
                            DateOfBirth = @DateOfBirth,
                            State = @State,
                            UserName = @UserName,
                            Role = @Role,
                            MobileNo2 = @MobileNo2,
                            DateOfJoin = @DateOfJoin,
                            Pincode = @Pincode,
                            Password = @Password,
                            Status = @Status,
                            PresentAddress = @PresentAddress,
                            PermanentAddress = @PermanentAddress
                          WHERE EmployeeCode = @EmployeeCode";
                    }
                    else
                    {
                        // Insert new record
                        query = @"INSERT INTO Employees 
                        (EmployeeCode, BranchName, Department, Area, Email, Qualification, EmployeeName, City, Gender, Designation, 
                         MobileNo1, DateOfBirth, State, UserName, Role, MobileNo2, DateOfJoin, Pincode, Password, Status, 
                         PresentAddress, PermanentAddress)
                        VALUES 
                        (@EmployeeCode, @BranchName, @Department, @Area, @Email, @Qualification, @EmployeeName, @City, @Gender, @Designation, 
                         @MobileNo1, @DateOfBirth, @State, @UserName, @Role, @MobileNo2, @DateOfJoin, @Pincode, @Password, @Status, 
                         @PresentAddress, @PermanentAddress)";
                    }
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", emp.EmployeeCode);
                    cmd.Parameters.AddWithValue("@BranchName", emp.BranchName);
                    cmd.Parameters.AddWithValue("@Department", emp.Department);
                    cmd.Parameters.AddWithValue("@Area", emp.Area);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Qualification", emp.Qualification ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    cmd.Parameters.AddWithValue("@City", emp.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@Designation", emp.Designation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo1", emp.MobileNo1);
                    cmd.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", emp.State ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", emp.UserName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", emp.Role ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo2", emp.MobileNo2);
                    cmd.Parameters.AddWithValue("@DateOfJoin", emp.DateOfJoin);
                    cmd.Parameters.AddWithValue("@Pincode", emp.Pincode);
                    cmd.Parameters.AddWithValue("@Password", emp.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", emp.Status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PresentAddress", emp.PresentAddress);
                    cmd.Parameters.AddWithValue("@PermanentAddress", emp.PermanentAddress);

                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }

            TempData["Message"] = "Employee data saved successfully!"; // Use TempData for message
            return RedirectToAction("Add");
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Employees WHERE EmployeeCode = @EmployeeCode";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Add");
        }

        private List<EmployeeModel> GetAllEmployees()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Employees";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new EmployeeModel
                        {
                            EmployeeCode = Convert.ToInt32(reader["EmployeeCode"]),
                            BranchName = reader["BranchName"].ToString(),
                            Department = reader["Department"].ToString(),
                            Email = reader["Email"].ToString(),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            MobileNo1 = Convert.ToInt64(reader["MobileNo1"]),
                            Role = reader["Role"].ToString(),
                            DateOfJoin = Convert.ToDateTime(reader["DateOfJoin"]),
                            Status = reader["Status"].ToString()
                        });
                    }

                    con.Close();
                }
            }

            return employees;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            EmployeeModel emp = new EmployeeModel();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Employees WHERE EmployeeCode = @EmployeeCode";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EmployeeCode", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    emp.EmployeeCode = Convert.ToInt32(reader["EmployeeCode"]);
                    emp.BranchName = reader["BranchName"].ToString();
                    emp.Department = reader["Department"].ToString();
                    emp.Area = reader["Area"].ToString();
                    emp.Email = reader["Email"].ToString();
                    emp.Qualification = reader["Qualification"].ToString();
                    emp.EmployeeName = reader["EmployeeName"].ToString();
                    emp.City = reader["City"].ToString();
                    emp.Gender = reader["Gender"].ToString();
                    emp.Designation = reader["Designation"].ToString();
                    emp.MobileNo1 = Convert.ToInt64(reader["MobileNo1"]);
                    emp.DateOfBirth = reader["DateOfBirth"] != DBNull.Value
                        ? Convert.ToDateTime(reader["DateOfBirth"])
                        : (DateTime?)null;
                    emp.State = reader["State"].ToString();
                    emp.UserName = reader["UserName"].ToString();
                    emp.Role = reader["Role"].ToString();
                    emp.MobileNo2 = Convert.ToInt64(reader["MobileNo2"]);
                    emp.DateOfJoin = Convert.ToDateTime(reader["DateOfJoin"]);
                    emp.Pincode = Convert.ToInt32(reader["Pincode"]);
                    emp.Password = reader["Password"].ToString();
                    emp.Status = reader["Status"].ToString();
                    emp.PresentAddress = reader["PresentAddress"].ToString();
                    emp.PermanentAddress = reader["PermanentAddress"].ToString();
                }
            }

            ViewBag.EmployeeList = GetAllEmployees();
            return View("Add", emp);
        }

    }
}
