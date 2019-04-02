namespace OnWeb.Plugins.Customer.Design.Model
{
    public class Profile
    {
        public TraceWeb.DB.User User { get; set; }

        public Customer.Model.ProfileEdit Edit { get; set; }
    }
}