namespace OnWeb.Core.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Описывает параметры модуля.
    /// </summary>
    [Table("ModuleConfig")]
    public partial class ModuleConfig
    {
        /// <summary>
        /// Идентификатор модуля.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdModule { get; set; }

        /// <summary>
        /// Уникальное значение, позволяющее идентифицировать query-тип модуля. Используется полное имя query-типа.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// Сериализованные в json параметры конфигурации модуля. См. <see cref="Configuration.ModuleConfiguration{TModule}"/>.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Дата последнего изменения записи в базе.
        /// </summary>
        public DateTime DateChange { get; set; }

        /// <summary>
        /// Идентификатор пользователя, менявшего параметры в последний раз.
        /// </summary>
        public int IdUserChange { get; set; }

    }
}
