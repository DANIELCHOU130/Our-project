using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace YourApiProject.Controllers // ← 請改成你的專案 namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDbController : ControllerBase
    {
        private readonly string _connectionString = "Server=134.208.97.162,1433;Initial Catalog=ESGGAMEDB;User ID=LAB;Password=NewStrongP@ssword2024;TrustServerCertificate=True";

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return Ok("✅ 成功連接到 SQL Server！");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ 連接失敗：{ex.Message}");
            }
        }
    }
}
