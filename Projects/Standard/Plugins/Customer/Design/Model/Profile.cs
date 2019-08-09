namespace OnWeb.Modules.Customer.Design.Model
{
    using Core.DB;

    public class Profile
    {
        public User User { get; set; }

        public Customer.Model.ProfileEdit Edit { get; set; }
    }
}