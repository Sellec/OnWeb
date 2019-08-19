namespace OnWeb.Core.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    public partial class Sessions
    {
        [Key]
        [StringLength(24)]
        public string SessionId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Created { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Expires { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime LockDate { get; set; }

        public int LockId { get; set; }

        public bool Locked { get; set; }

        public byte[] ItemContent { get; set; }

        public int IdUser { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; } = false;

        public object SyncRoot = new object();

        public DateTime DateLastChanged = DateTime.Now;
        public DateTime DateLastSaved = DateTime.Now;
    }
}
