using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string connStr;

    public AuthController(IConfiguration config)
    {
        connStr = config.GetConnectionString("Default");
    }

    [HttpPost("login")]
    public IActionResult Login([FromForm] string account, [FromForm] string password)
    {
        using SqlConnection conn = new SqlConnection(connStr);
        conn.Open();
        string query = "SELECT * FROM AccountDATA WHERE account = @acc AND password = @pw";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@acc", account);
        cmd.Parameters.AddWithValue("@pw", password);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Ok("LoginSuccess") : Unauthorized("LoginFailed");
    }

    [HttpPost("register")]
    public IActionResult Register([FromForm] string username, [FromForm] string account, [FromForm] string password)
    {
        using SqlConnection conn = new SqlConnection(connStr);
        conn.Open();

        SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM AccountDATA WHERE account = @acc", conn);
        check.Parameters.AddWithValue("@acc", account);
        if ((int)check.ExecuteScalar() > 0)
            return Conflict("AccountExists");

        SqlCommand reg = new SqlCommand("INSERT INTO AccountDATA (account, password, username) VALUES (@acc, @pw, @name)", conn);
        reg.Parameters.AddWithValue("@acc", account);
        reg.Parameters.AddWithValue("@pw", password);
        reg.Parameters.AddWithValue("@name", username);
        reg.ExecuteNonQuery();

        SqlCommand insertGame = new SqlCommand("INSERT INTO nowgamedata (account, money, esg) VALUES (@acc, 100000, 0)", conn);
        insertGame.Parameters.AddWithValue("@acc", account);
        insertGame.ExecuteNonQuery();

        return Ok("RegisterSuccess");
    }
}
