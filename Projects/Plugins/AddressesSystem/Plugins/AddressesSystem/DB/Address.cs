namespace OnWeb.Core.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Addresses;

#pragma warning disable CS1591 // todo ������ �����������.
    [Table("Address")]
    public partial class Address : Items.ItemBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(30)]
        public string KodAddress { get; set; } = "";

        [StringLength(200)]
        public string NameAddress { get; set; } = "";

        [StringLength(100)]
        public string NameAddressShort { get; set; } = "";

        [StringLength(200)]
        public string NameAddressFull { get; set; } = "";

        public Guid? KodFias { get; set; } = null;

        [StringLength(8)]
        public string ZipCode { get; set; }

        [NotMapped]
        //[StringLength(50)]
        public string Okato { get; set; }

        public DateTime DateChange { get; set; }

        [StringLength(32)]
        public string KodRegion { get; set; } = "";

        [StringLength(32)]
        public string KodDistrict { get; set; } = "";

        [StringLength(32)]
        public string KodCity { get; set; } = "";

        [StringLength(32)]
        public string KodStreet { get; set; } = "";

        /// <summary>
        /// ��� ������ ����� ����� ���� ����� - ���� ����� ��� ���������� �����. ������� ����� ����� ������������� � ��� ����, � ���������� (����� + ����� ����) � <see cref="KodBuilding"/>.
        /// </summary>
        [StringLength(32)]
        public string KodBuildingCommon { get; set; } = "";

        /// <summary>
        /// ��. <see cref="KodBuildingCommon"/>.
        /// </summary>
        [StringLength(32)]
        public string KodBuilding { get; set; } = "";

        public AddressType AddressType { get; set; } = AddressType.Region;

        public decimal CoordinateX { get; set; }

        public decimal CoordinateY { get; set; }

        /// <summary>
        /// ���������� �� ����� �������.
        /// </summary>
        public bool IsRegion { get; set; }

        /// <summary>
        /// �������� �� ����� ���������������� ������� �������.
        /// </summary>
        public bool IsRegionCenter { get; set; }

        /// <summary>
        /// ���������� �� ����� ���������������� �����.
        /// </summary>
        public bool IsDistrict { get; set; }

        /// <summary>
        /// �������� �� ����� ���������������� ������� ������.
        /// </summary>
        public bool IsDistrictCenter { get; set; }

        /// <summary>
        /// ���������� �� ����� �����.
        /// </summary>
        public bool IsCity { get; set; }

        #region ItemBase
        /// <summary>
        /// </summary>
        public override int ID
        {
            get { return 0; }
            set { ; }
        }

        /// <summary>
        /// ��. <see cref="NameAddressFull"/>.
        /// </summary>
        public override string Caption
        {
            get { return NameAddressFull; }
            set { NameAddressFull = value; }
        }

        /// <summary>
        /// ����� ���������� ��������� �� ������ <see cref="DateChange"/>. 
        /// </summary>
        public override DateTime DateChangeBase
        {
            get { return DateChange; }
            set { DateChange = value; }
        }
        #endregion

        //#region �������������� ��������

        //[ForeignKey("KodCity")]
        //public Address City { get; set; }
        //#endregion

        public override string ToString()
        {
            return KodAddress;
        }

    }
}

