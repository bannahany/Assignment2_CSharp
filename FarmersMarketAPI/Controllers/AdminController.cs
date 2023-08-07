using FarmersMarketAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace FarmersMarketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
       private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AddProduct")]
        public Response AddProduct(Products product) 
        {
            NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());
            Response response = new Response();

            Applications applications = new Applications();
            response = applications.AddProduct(con, product);
            return response;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public Response GetProduct(int id)
        {
            Response response = new Response();
            NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());
            Applications applications = new Applications();
            response = applications.GetAllProducts(con);
            return response;
        }

        [HttpGet]
        [Route("GetProductById/{product_id}")]
        public Response GetProductById(int product_id)
        {
            Response response = new Response();
            NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());
            Applications app = new Applications();
            response = app.GetProductById(con, product_id);
            return response;
        }

        // Update information using REST API

        [HttpPut]
        [Route("UpdateProduct/{product_id}")]
        public Response UpdateProduct(Products product, int product_id)
        {
            Response response = new Response();
            NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());
            Applications app = new Applications();
            response = app.UpdateProduct(con, product, product_id);
            return response;

        }

        // Delete information using REST API, which is done using student ID

        [HttpDelete]
        [Route("DeleteProductById/{product_id}")]
        public Response DeleteProductById(int product_id)
        {
            Response response = new Response();
            NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());
            Applications app = new Applications();
            response = app.DeleteProductById(con, product_id);
            return response;
        }

        //[HttpGet]
        //[Route("GetAllProductNames")]
        //public Response GetAllProductNames(int product_id)
        //{
        //    Response response = new Response();
        //    NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("productsConnection").ToString());

        //}

    }
}