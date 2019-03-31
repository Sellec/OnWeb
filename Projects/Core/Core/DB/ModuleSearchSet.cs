namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("ModuleSearchSet")]
    public partial class ModuleSearchSet : Items.ItemBase
    {
        [NotMapped]
        private int _idModule = 0;

        [Key]
        public int IdSearchSet { get; set; }

        [StringLength(500)]
        public string NameSearchSet { get; set; }

        [StringLength(100)]
        public string NameSearchSetShort { get; set; }

        [StringLength(255)]
        public string NameBlock { get; set; }

        [StringLength(100)]
        public string NameEntity { get; set; }

        public int IdModule
        {
            get { return _idModule; }
            set
            {
                _idModule = value;
                Owner = DeprecatedSingletonInstances.ModulesManager.GetModule(value);
            }
        }

        public int CountParameters { get; set; }

        public int CountItems { get; set; }

        public bool IsPreset { get; set; }

        public string description { get; set; }

        public string description_old { get; set; }

        [StringLength(500)]
        public string seo_title { get; set; }

        [StringLength(500)]
        public string seo_descr { get; set; }

        public int DateCreate { get; set; }

        public int IdUserCreate { get; set; }

        public int DateChange { get; set; }

        public int IdUserChange { get; set; }

        public int CountUsed { get; set; }

        [StringLength(200)]
        public string urlname { get; set; }

        [StringLength(100)]
        public string UniqueKey { get; set; }

        #region Items.ItemBase
        public override int ID
        {
            get { return IdSearchSet; }
            set { IdSearchSet = value; }
        }

        public override string Caption
        {
            get { return NameSearchSet; }
            set { NameSearchSet = value; }
        }
        #endregion
    }
}
