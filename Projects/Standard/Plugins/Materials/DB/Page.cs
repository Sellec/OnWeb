namespace OnWeb.Plugins.Materials.DB
{
    using Core.Items;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Page")]
    public partial class Page : ItemBase<ModuleMaterials>
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

        public int id { get; set; }

        public int? category { get; set; }

        [Required]
        public string subs_id { get; set; }

        [Required]
        public string subs_order { get; set; }

        public short status { get; set; }

        [Required]
        [StringLength(20)]
        public string language { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string urlname { get; set; }

        [Required]
        public string body { get; set; }

        public short parent { get; set; }

        public int order { get; set; }

        [Required]
        public string photo { get; set; }

        public int count_views { get; set; }

        public int comments_count { get; set; }

        public int pages_gallery { get; set; }

        public int news_id { get; set; }

        [Required]
        [StringLength(255)]
        public string seo_title { get; set; }

        [Required]
        public string seo_descr { get; set; }

        [Required]
        public string seo_kw { get; set; }

        [Required]
        [StringLength(255)]
        public string ajax_name { get; set; }
    }
}
