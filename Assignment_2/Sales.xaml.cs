using Assignment_2.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Assignment_2
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Window
    {
        private HttpClient httpClient = new HttpClient();

        List<SaleItem> salesItems;


        public Sales()
        {

            InitializeComponent();

         //   FillProductComboBox();
            salesItems = new List<SaleItem>();
            //cartItems = new List<CartItems>();

        }







        private async  Task FillProductComboBox()
        {
            try
            {
                string apiUrl = "https://localhost:7002/api/Admin/GetAllProducts";
                var response = await httpClient.GetStringAsync(apiUrl);

                Response res = JsonConvert.DeserializeObject<Response>(response);

                List<Products> prods = res.products;
                C_Box_productName.ItemsSource = null;
                C_Box_productName.ItemsSource = prods;
                C_Box_productName.DisplayMemberPath = "product_name";
                C_Box_productName.SelectedValuePath = "product_id";


             //   int prodid = (int)C_Box_productName.SelectedValue;

            }
            catch (HttpRequestException ex)
            {
                
            }
            catch (Exception ex)
            {
                
            }
        }

        private async Task<decimal> GetProductPriceAsync(string productName)
        {
            try
            {
                // Send an HTTP GET request to the API to fetch the product details
                string apiUrl = "https://localhost:7002/api/Admin/"; 
                var response = await httpClient.GetStringAsync(apiUrl + productName);

                // Deserialize the API response into the Product class
                var product = JsonConvert.DeserializeObject<Models.Products>(response);

                if (product != null)
                {
                    return product.product_id;
                }
                else
                {
            
                    return 0;
                }
            }
            catch (HttpRequestException ex)
            {

                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        private async Task UpdateProductInventoryAsync(Products product, int product_id)
        {
            try
            {
                 product = new Products
                {
                    product_name = C_Box_productName.Text,
                    product_id = product_id,
                    amount_kg = product.amount_kg - int.Parse(txt_amount.Text),
                   
                };

                // Send an HTTP PUT request to the API to update the product
                string apiUrl = "https://localhost:7002/api/Admin/UpdateProduct/" + product_id;
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(apiUrl, content);



                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show ("Inventory updated succesfully");
                }
                else
                {
                    string errorMessage = "Inventory update failed: " + response.ReasonPhrase;
                    MessageBox.Show(errorMessage);                 
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("HTTP request error: " + ex.Message);
               
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Operation error: " + ex.Message);
                
            }
            catch (JsonException ex)
            {
                MessageBox.Show("JSON error: " + ex.Message);           
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Admin adminWindow = new();
            adminWindow.Show();
            Close();
        }

        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            string textBoxEntry = txt_amount.Text;

            if (!int.TryParse(C_Box_productName.SelectedValue?.ToString(), out int prodid))
            {
                MessageBox.Show("Please select a valid product.");
                return;
            }

            try
            {
                // Send an HTTP GET request to the API to fetch product details
                string apiUrl = "https://localhost:7002/api/Admin/GetProductById/";
                var response = await httpClient.GetStringAsync(apiUrl + prodid);

                // Deserialize the API response into the Response class
                var responseData = JsonConvert.DeserializeObject<Response>(response);

                if (responseData != null && responseData.statusCode == 200 && responseData.product != null)
                {
                    double productPrice = responseData.product.price_cad_kg;
                    int qty = int.Parse(textBoxEntry);

                    SaleItem si = new SaleItem();
                    si.ProductPrice = productPrice;
                    si.ProductName = responseData.product.product_name;
                    si.ProductQTY = qty;
                    si.ProductID = responseData.product.product_id.ToString();
                    salesItems.Add(si);

                    dta_ViewSales.ItemsSource = null;
                    dta_ViewSales.Items.Clear();
                    dta_ViewSales.ItemsSource = salesItems;

                    double total = salesItems.AsEnumerable().Sum(si => si.ProductPrice * si.ProductQTY);
                    lbl_total.Content = "$" + total.ToString("0.00");
                }
                else
                {
                    MessageBox.Show("Product not found in the API");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("An error occurred while fetching product details: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding to cart: " + ex.Message);
            }
        }



        private async void Btn_Checkout(object sender, RoutedEventArgs e)
        {
            if (salesItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty.");
                return;
            }

            double total = salesItems.AsEnumerable().Sum(si => si.ProductPrice * si.ProductQTY);

            MessageBoxResult r = MessageBox.Show("You have made a purchase worth of $" + total.ToString("0.00"), "Confirmation", MessageBoxButton.YesNo);

            if (r == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var item in salesItems)
                    {
                        // Send an HTTP PATCH request to the API to update the product inventory
                        string apiUrl = "https://localhost:7002/api/Admin/UpdateProduct"; 
                        string productUrl = apiUrl + "/" + item.ProductID;
                        var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                        // Use PATCH request to update the inventory quantity for the product
                        var response = await httpClient.PutAsJsonAsync(productUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Failed to update inventory for {item.ProductName}.");
                        }
                    }

                    // Clear the cart items list after successful checkout
                    salesItems.Clear();
                    dta_ViewSales.ItemsSource = null;
                    dta_ViewSales.Items.Clear();

                    lbl_total.Content = "$0.00";

                    MessageBox.Show("Checkout successful. Thank you for your purchase!");
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("An error occurred during checkout: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during checkout: " + ex.Message);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
          await   FillProductComboBox();
        }
    }
}



