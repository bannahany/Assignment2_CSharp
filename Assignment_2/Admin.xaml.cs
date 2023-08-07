using Assignment_2.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Assignment_2
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        HttpClient httpClient = new HttpClient();

        public Admin()
        {
            httpClient.BaseAddress = new Uri("https://localhost:7002/api/Admin/");
            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            InitializeComponent();
        }

        private async void Btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            // Check if any of the text boxes are empty
            if (string.IsNullOrWhiteSpace(txt_product_Name.Text) ||
                string.IsNullOrWhiteSpace(txt_Product_Id.Text) ||
                string.IsNullOrWhiteSpace(txt_Amount_kg.Text) ||
                string.IsNullOrWhiteSpace(txt_Price.Text))
            {
                MessageBox.Show("Please fill in all the fields");
                return;
            }

            Products product = new Products
            {
                product_name = txt_product_Name.Text,
                product_id = int.Parse(txt_Product_Id.Text),
                amount_kg = int.Parse(txt_Amount_kg.Text),
                price_cad_kg = double.Parse(txt_Price.Text)
            };

            try
            {
                var response = await httpClient.PostAsJsonAsync("AddProduct", product);

                // Ensure that the response is successful before reading the content
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response content into ResponseData class
                var responseData = JsonConvert.DeserializeObject<Response>(responseContent);

                // Show only the relevant part of the response in the text box
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Status Code: " + responseData.statusCode + "... " + responseData.message;
            }
            catch (HttpRequestException ex)
            {
                // If the request fails, show the error message in the text box
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }


        }

        private async void btn_Show_DB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetch the list of products from the API
                var response = await httpClient.GetStringAsync("GetAllProducts");

                // Deserialize the response into a Response object
                var apiResponse = JsonConvert.DeserializeObject<Response>(response);

                // Check if the API call was successful
                if (apiResponse.statusCode == 200 && apiResponse.products != null)
                {
                    // Display the list of products in the data grid
                    dta_View.ItemsSource = apiResponse.products;

                    // Hide any previous response messages
                    txt_response.Visibility = Visibility.Hidden;
                }
                else
                {
                    // Show a message when there are no products in the API response
                    txt_response.Visibility = Visibility.Visible;
                    txt_response.Text = "No products found ";
                }
            }
            catch (HttpRequestException ex)
            {
                // If the request fails, show the error message in the response text box
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }
        }


        private async void btn_Search_DB_Click(object sender, RoutedEventArgs e)
        {
            // Check if the product ID is valid
            if (!int.TryParse(txt_Product_Id.Text, out int product_id))
            {
                MessageBox.Show("Please enter a valid product ID");
                return;
            }

            try
            {
                // Search for the product by ID
                var response = await httpClient.GetStringAsync("GetProductById/" + product_id);

                // Deserialize the response into a Response object
                var apiResponse = JsonConvert.DeserializeObject<Response>(response);

                // Show the product data in the relevant text boxes
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Status Code: " + apiResponse.statusCode + "... " + apiResponse.message;

                // Check if the product is found in the API response
                if (apiResponse.product != null)
                {
                    // Display product details in text boxes
                    txt_product_Name.Text = apiResponse.product.product_name;
                    txt_Product_Id.Text = apiResponse.product.product_id.ToString();
                    txt_Amount_kg.Text = apiResponse.product.amount_kg.ToString();
                    txt_Price.Text = apiResponse.product.price_cad_kg.ToString();
                }
                else
                {
                    // Show a message when the product is not found
                    txt_product_Name.Text = "";
                    txt_Amount_kg.Text = "";
                    txt_Price.Text = "";
                    MessageBox.Show("Product not found");
                }
            }
            catch (HttpRequestException ex)
            {
                // If the request fails, show the error message in the response text box
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;

                // Clear the text boxes when there's an error
                txt_product_Name.Text = "";
                txt_Amount_kg.Text = "";
                txt_Price.Text = "";
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;

                // Clear the text boxes when there's an error
                txt_product_Name.Text = "";
                txt_Amount_kg.Text = "";
                txt_Price.Text = "";
            }
        }

        private async void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Check if the product ID is valid
            if (!int.TryParse(txt_Product_Id.Text, out int productId))
            {
                MessageBox.Show("Please enter a valid product ID");
                return;
            }

            try
            {
                // Send a DELETE request to delete the product
                var response = await httpClient.DeleteAsync($"DeleteProductById/{productId}");

                // Deserialize the response into a Response object
                var apiResponse = JsonConvert.DeserializeObject<Response>(await response.Content.ReadAsStringAsync());

                // Check if the API call was successful
                if (apiResponse.statusCode == 200)
                {
                    // Show the success message in the response text box
                    txt_response.Visibility = Visibility.Visible;
                    txt_response.Text = "Product deleted successfully";

                    // Clear the text boxes after successful deletion
                    txt_product_Name.Text = "";
                    txt_Amount_kg.Text = "";
                    txt_Price.Text = "";
                }
                else
                {
                    // Show the error message in the response text box if deletion was unsuccessful
                    txt_response.Visibility = Visibility.Visible;
                    txt_response.Text = "Error: " + apiResponse.message;
                }
            }
            catch (HttpRequestException ex)
            {
                // If the request fails, show the error message in the response text box
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                txt_response.Visibility = Visibility.Visible;
                txt_response.Text = "Error: " + ex.Message;
            }
        }
        private void btn_DSales_Click(object sender, RoutedEventArgs e)
        {
            Sales salesWindow = new();
            salesWindow.Show();
            Close();
        }

        private async void btn_Update_DB_Click(object sender, RoutedEventArgs e)
        {
            // Check if the product ID is valid
            if (!int.TryParse(txt_Product_Id.Text, out int product_id))
            {
                MessageBox.Show("Please enter a valid product ID");
                return;
            }

            // Check if the product name and amount fields are empty
            if (string.IsNullOrEmpty(txt_product_Name.Text) || string.IsNullOrEmpty(txt_Amount_kg.Text))
            {
                MessageBox.Show("Please enter product name and amount.");
                return;
            }

            // Check if the amount entered is a valid integer
            if (!int.TryParse(txt_Amount_kg.Text, out int amount_kg))
            {
                MessageBox.Show("Please enter a valid amount in kg.");
                return;
            }

            // Check if the price entered is a valid decimal
            if (!decimal.TryParse(txt_Price.Text, out decimal price_cad_kg))
            {
                MessageBox.Show("Please enter a valid price in CAD.");
                return;
            }

            try
            {
                // Create a new Products object with updated data
                Products product = new Products
                {
                    product_name = txt_product_Name.Text,
                    product_id = product_id,
                    amount_kg = amount_kg,
                    price_cad_kg = double.Parse(txt_Price.Text)
                };

                // Send an HTTP PUT request to the API to update the product
                string apiUrl = "https://localhost:7002/api/Admin/UpdateProduct/" + product_id;
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Product updated successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to update product.");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("An error occurred while updating product: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


    }
}




