using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly string connStr;

    public GameController(IConfiguration config)
    {
        connStr = config.GetConnectionString("Default");
    }

    [HttpGet("getcard")]
    public IActionResult GetCard(string type)
    {
        using SqlConnection conn = new SqlConnection(connStr);
        conn.Open();
        string query = "SELECT TOP 1 * FROM CardDATA WHERE cardtype = @type ORDER BY NEWID()";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@type", type);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var result = new
            {
                cardname = reader["cardname"].ToString(),
                cardin = reader["cardin"].ToString(),
                cardmoney = (int)reader["cardmoney"],
                cardesg = (int)reader["cardesg"],
                cardknow = reader["cardknow"].ToString(),
                cardtype = reader["cardtype"].ToString()
            };
            return Ok(result);
        }
        return NotFound("No card found");
    }

    [HttpPost("updategamedata")]
    public IActionResult UpdateGameData([FromForm] string account, [FromForm] int moneyDelta, [FromForm] int esgDelta)
    {
        using SqlConnection conn = new SqlConnection(connStr);
        conn.Open();
        string query = "UPDATE nowgamedata SET money = money + @m, esg = esg + @e WHERE account = @acc";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@m", moneyDelta);
        cmd.Parameters.AddWithValue("@e", esgDelta);
        cmd.Parameters.AddWithValue("@acc", account);

        return cmd.ExecuteNonQuery() > 0 ? Ok("更新成功") : NotFound("帳號不存在");
    }

    [HttpGet("nowgamedata")]
    public IActionResult GetGameData(string account)
    {
        using SqlConnection conn = new SqlConnection(connStr);
        conn.Open();
        string query = "SELECT * FROM nowgamedata WHERE account = @acc";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@acc", account);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var result = new
            {
                money = (int)reader["money"],
                esg = (int)reader["esg"]
            };
            return Ok(result);
        }
        return NotFound("查無資料");
    }
}
