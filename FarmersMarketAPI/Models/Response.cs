namespace FarmersMarketAPI.Models
{
    public class Response
    {

        public int statusCode {  get; set; }
        public string message { get; set; }
        public Products product { get; set; } 

        public List<Products> products { get; set; }  

    }
}
