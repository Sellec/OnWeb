namespace OnWeb.Plugins.Materials.DB
{
    using Core.Items;
    using Core.Modules.Extensions.ExtensionUrl;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("News")]
    public class News : ItemBase, IItemBaseUrl
    {
        public override int ID
        {
            get => id;
            set => id = value; 
        }

        public override string Caption
        {
            get => name;
            set => name = value; 
        }

        public override DateTime DateChangeBase
        {
            get => date;
            set => date = value;
        }

        public int id { get; set; }

        public int category { get; set; }

        public bool status { get; set; }

        [Required]
        [MaxLength(300)]
        public string name { get; set; }

        public string short_text { get; set; }

        public string text { get; set; }

        public DateTime date { get; set; }

        public int comments_count { get; set; }

        public int user { get; set; }

        public bool Block { get; set; } = false;

        public Uri Url => UrlBase;

        public UrlSourceType UrlSourceType => UrlSourceTypeBase;
    }
}
