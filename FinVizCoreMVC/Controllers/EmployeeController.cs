using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using static FinVizCoreMVC.Controllers.EmployeeController;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace FinVizCoreMVC.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult EmployeeData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var JwtTokens = new JwtToken();
            
            var loginModel = new LoginRequest
            {
                Username = username,
                Password = password
            };


           
            using (var clients = new HttpClient())
            {
                var url = "https://localhost:7024/api/";
                client.BaseAddress = new Uri(url);

                var byteContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(loginModel)));
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var postTask = client.PostAsync("Auth/login", byteContent);

                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    // var responseString = await result.Content.ReadAsStringAsync();
                    //var data = JsonConvert.DeserializeObject<dynamic>(responseString);
                    //JwtTokens = data.token;
                    //string token = data.token;
                    //HttpContext.Session.SetString("JWToken", token);
                    // HttpContext.Session.SetString("TokenExpiry", DateTime.UtcNow.AddMinutes(1).ToString());

                      var data = await result.Content.ReadAsStringAsync();
                   
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(data);
                    string token = response.token;
                    DateTime expiresAt = response.expiresAt;
                    return Json(expiresAt);

                }
                else
                {
                    var error = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {result.StatusCode}, {error}");
                }
            }
            return Json(JwtTokens, new JsonSerializerOptions());
            //return Json(JwtTokens, new JsonSerializerOptions());

        }



        [HttpGet]
        public async Task<JsonResult> ShowData(int id)
        {
            //var token = HttpContext.Session.GetString("JWToken");
            //var expiryStr = HttpContext.Session.GetString("TokenExpiry");

            //if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expiryStr))
            //{
            //    return RedirectToAction("SignIn", "Employee");
            //}

            //var expiry = DateTime.Parse(expiryStr);
            //if (DateTime.UtcNow > expiry)
            //{
            //    HttpContext.Session.Clear();
            //    /// return RedirectToAction("SignIn", "Employee");
            //    token = "false";
            //}
            List<Employee> Employees = new List<Employee>();
            using (var client = new HttpClient())
            {
                var url = "https://localhost:7024/api/";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage previousdata = await client.GetAsync("User/showdata?id=" + id);

                if (previousdata.IsSuccessStatusCode)
                {
                    var data = await previousdata.Content.ReadAsStringAsync();
                    Employees = JsonConvert.DeserializeObject<List<Employee>>(data);
                }

            }

            return Json(Employees);

        }

        //public IActionResult ShowData(int id)
        //{
        //    var token = HttpContext.Session.GetString("JWToken");
        //    var expiryStr = HttpContext.Session.GetString("TokenExpiry");

        //    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expiryStr))
        //    {
        //        return RedirectToAction("SignIn", "Employee");
        //    }

        //    var expiry = DateTime.Parse(expiryStr);
        //    if (DateTime.UtcNow > expiry)
        //    {
        //        HttpContext.Session.Clear();
        //       /// return RedirectToAction("SignIn", "Employee");
        //        token = "false";
        //    }
        //    //else
        //    //{
        //    //    IEnumerable<Employee> Employees = null;
        //    //    using (var client = new HttpClient())
        //    //    {
        //    //        var url = "https://localhost:7024/api/";
        //    //        client.BaseAddress = new Uri(url);
        //    //        client.DefaultRequestHeaders.Clear();
        //    //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    //        //    Task<HttpResponseMessage> reposnseBusinessUnits =
        //    //        //        client.GetAsync("User/showdata?id=" + id);

        //    //        //    var resultbusinessunits = reposnseBusinessUnits.Result;
        //    //        //    if (resultbusinessunits.IsSuccessStatusCode)
        //    //        //    {
        //    //        //         var readBusiness = await resultbusinessunits.Content.ReadAsAsync<IEnumerable<Employee>>();
        //    //        //         Employees = readBusiness;


        //    //        //    }
        //    //        //}

        //    //        //return Json(Employees, new JsonSerializerOptions());
        //    //        var result = client.GetAsync("User/showdata?id=" + id);

        //    //        // Console.WriteLine("Status Code: " + resultbusinessunits.StatusCode);
        //    //        var responseText = result.Content.ReadAsStringAsync();
        //    //        //  Console.WriteLine("Response Body: " + responseText);

        //    //        if (resultbusinessunits.IsSuccessStatusCode)
        //    //        {
        //    //            Employees = resultbusinessunits.Content.ReadFromJsonAsync<IEnumerable<Employee>>();
        //    //        }
        //    //    }
        //    //}
        //    //return Json(Employees);
        //    return Json(token);

        //}
        [HttpGet]
        public async Task<JsonResult> ShowPreviousEmpData(int id)
        {

            List<EmployeeExperiences> Employees = new List<EmployeeExperiences>();
            using (var client = new HttpClient())
            {
                var url = "https://localhost:7024/api/";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage previousdata = await client.GetAsync("User/showpreviousempdata?id=" + id);

                   if (previousdata.IsSuccessStatusCode)
                {
                    var data = await previousdata.Content.ReadAsStringAsync();
                    Employees = JsonConvert.DeserializeObject<List<EmployeeExperiences>>(data);
                }

            }

            return Json(Employees);

        }

        [HttpGet]
        public async Task<JsonResult> ShowEmpSalaryData(int id)
        {

            List<EmployeeSalary> Employeesalary = new List<EmployeeSalary>();
            using (var client = new HttpClient())
            {
                var url = "https://localhost:7024/api/";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage previousdata = await client.GetAsync("User/ShowEmpSalaryData?id=" + id);


                if (previousdata.IsSuccessStatusCode)
                {
                    var data = await previousdata.Content.ReadAsStringAsync();
                    Employeesalary = JsonConvert.DeserializeObject<List<EmployeeSalary>>(data);
                }

            }

            return Json(Employeesalary);

        }

        [HttpGet]
        public async Task<JsonResult> GetEmpDetails(int id)
        {

            List<EmployeeHoleSalary> Employeesalary = new List<EmployeeHoleSalary>();
            using (var client = new HttpClient())
            {
                var url = "https://localhost:7024/api/";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage previousdata = await client.GetAsync("User/getempdetails?id=" + id);


                if (previousdata.IsSuccessStatusCode)
                {
                    var data = await previousdata.Content.ReadAsStringAsync();
                    Employeesalary = JsonConvert.DeserializeObject<List<EmployeeHoleSalary>>(data);
                }

            }

            return Json(Employeesalary);

        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class JwtToken
        {
            public string token { get; set; }
            public int expiresAt { get; set; }
        }
        public class Employee
        {
            public int EmployeeId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Designation { get; set; }
            public DateTime DateOfBirth { get; set; }
        }
        public class EmployeeExperiences
        {
            public int EmployeeId { get; set; }
            public int ExperienceId { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Designation { get; set; }
            public string CompanyName { get; set; }
            public string State { get; set; }
            public string ReasonForLeaving { get; set; }


        }
        public class EmployeeSalary
        {
            public int EmployeeId { get; set; }
            public int SalaryId { get; set; }
            public decimal BasicSalary { get; set; }
            public decimal HRA { get; set; }
            public decimal OtherAllowances { get; set; }
            public decimal Deductions { get; set; }



        }
        public class EmployeeHoleSalary
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string curentDesignation { get; set; }
            public string curentstate { get; set; }
            public DateTime DateOfBirth { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Designation { get; set; }
            public string CompanyName { get; set; }
            public string State { get; set; }
            public string ReasonForLeaving { get; set; }
            public decimal BasicSalary { get; set; }
            public decimal HRA { get; set; }
            public decimal OtherAllowances { get; set; }
            public decimal Deductions { get; set; }
        }
 
 
    
    }
}

