using Dapper;
using DapperDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SuperHeroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "select * from SuperHeroes";
            IEnumerable<SuperHero> heroes = await SelectAllHeros(connection, sql);
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "select * from SuperHeroes where id = @Id";
            var hero = await connection.QuerySingleAsync<SuperHero>(sql, new { Id = heroId });
            return Ok(hero);
        }

        [HttpPost("NewHero")]
        public async Task<ActionResult<int>> CreateHero(SuperHero superHero)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "insert into SuperHeroes (Name, FirstName, LastName, Place) values (@Name, @FirstName, @LastName, @Place)";
            var result = await connection.ExecuteAsync(sql, superHero);
            return Ok(result);
        }

        [HttpDelete("Delete{heroId}")]
        public async Task<ActionResult<int>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "delete from SuperHeroes where id = @Id ";
            var result = await connection.ExecuteAsync(sql, new {Id = heroId});
            return Ok(result);
        }


        private static async Task<IEnumerable<SuperHero>> SelectAllHeros(SqlConnection connection, string sql)
        {
            return await connection.QueryAsync<SuperHero>(sql);
        }
    }
}
