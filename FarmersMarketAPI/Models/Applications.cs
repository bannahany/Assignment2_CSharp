using Npgsql;
using System.Data;

namespace FarmersMarketAPI.Models
{
    public class Applications
    {
        public Response AddProduct(NpgsqlConnection con, Products product)
        {
            Response response = new Response();

            try
            { 
                string Query = "insert into Products values(@Product_Name, @Product_ID, @amount_kg, @price_CAD_kg)";
                NpgsqlCommand cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@product_name", product.product_name);
                cmd.Parameters.AddWithValue("@product_id", product.product_id);
                cmd.Parameters.AddWithValue("@amount_kg", product.amount_kg);
                cmd.Parameters.AddWithValue("@price_cad_kg", product.price_cad_kg);
                con.Open();

                int i = cmd.ExecuteNonQuery();

                if (i > 0)
                {
                    response.statusCode = 200;
                    response.message = "Product added succesfully";
                    response.product = product;
                    response.products = null;
                }
                else
                {
                    response.statusCode = 100;
                    response.message = "Nothing added";
                    response.product = null;
                    response.products = null;
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public Response GetAllProducts(NpgsqlConnection con)
        {
            Response response = new Response();
            try
            {
                string Query = "Select * from products";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(Query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                List<Products> products = new List<Products>();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Products p = new Products();
                        p.product_name = (string)dt.Rows[i]["product_name"];
                        p.product_id = (int)dt.Rows[i]["product_id"];
                        p.amount_kg = (int)dt.Rows[i]["amount_kg"];
                        p.price_cad_kg = (double)dt.Rows[i]["price_cad_kg"];

                        products.Add(p);


                    }

                }

                if (products.Count > 0)
                {
                    response.statusCode = 200;
                    response.message = "Products are retrieved succesfully";
                    response.product = null;
                    response.products = products;

                }
                else
                {
                    response.statusCode = 400;
                    response.message = "Products are not retrieved";
                    //response.product = new Products();
                    //response.products = ne;
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public Response GetProductById(NpgsqlConnection con, int product_id)
        {
            Response response = new Response();
            try
            {

                string Query = "select * from products where product_id=@product_id";
                NpgsqlCommand cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@product_id", product_id);
                // data adapter
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);   // data table will get the structure/entries of thhe dtabase over here
                Products product = new Products();    // this will create instance for only one student

                if (dt.Rows.Count > 0)  // this means we get at least one match, which is required
                {
                    product.product_name = (string)dt.Rows[0]["product_name"];

                    product.product_id = (int)dt.Rows[0]["product_id"];
                    product.amount_kg = (int)dt.Rows[0]["amount_kg"];
                    product.price_cad_kg = (double)dt.Rows[0]["price_cad_kg"];

                    // create the response from our server a well

                    response.statusCode = 200;
                    response.message = "Product found and retrieved";
                    response.product = product;
                    response.products = null;

                }

                else
                {
                    product = null;
                    response.statusCode = 100;
                    response.message = "Couldn't find product... might not be present in our database";
                    response.product = null;
                    response.products = null;
                }



            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }
        public Response UpdateProduct(NpgsqlConnection con, Products product, int product_id)
        {

            Response response = new Response();
            try
            {
                con.Open();
                string Query = "update products set product_name=@product_name, amount_kg=@amount_kg, price_cad_kg=@price_cad_kg where product_id=@product_id";
                NpgsqlCommand cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@product_name", product.product_name);
                cmd.Parameters.AddWithValue("@amount_kg", product.amount_kg);
                cmd.Parameters.AddWithValue("@price_cad_kg", product.price_cad_kg);
                cmd.Parameters.AddWithValue("@product_id", product_id);


                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    response.statusCode = 200;
                    response.message = "Product information updated succesfully";
                    response.product = product;
                    response.products = null;

                }
                else
                {
                    response.statusCode = 100;   // it fails to update the reote server student
                    response.message = "Product failed to update";
                    response.product = null;
                    response.products = null;

                }


            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public Response DeleteProductById(NpgsqlConnection con, int product_id)
        {
            Response response = new Response();

            try
            {
                con.Open();
                string Query = "Delete from products where product_id='" + product_id + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(Query, con);
                int i = cmd.ExecuteNonQuery();

                if (i > 0)
                {
                    response.statusCode = 200;
                    response.message = "Product deleted from the database";
                    response.product = null;
                    response.products = null;

                }
                else
                {
                    response.statusCode = 100;
                    response.message = "Product couldn't be searched or deleted";
                    response.product = null;
                    response.products = null;
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public List<string> GetAllProductNames(NpgsqlConnection con)
        {
            List<string> productNamesList = new List<string>();
            try
            {
                string query = "SELECT product_name FROM products";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);

                // Data adapter
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt); // Data table will get the structure/entries of the database over here

                if (dt.Rows.Count > 0)
                {
                    // If there are rows, extract the product names and add them to the list
                    foreach (DataRow row in dt.Rows)
                    {
                        string productName = (string)row["product_name"];
                        productNamesList.Add(productName);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return productNamesList;
        }
    }
}
