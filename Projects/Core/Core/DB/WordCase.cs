namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Plugins.Lexicon;

    /// <summary>
    /// Хранит информацию о падежах слова.
    /// </summary>
    [Table("WordCase")]
    public partial class WordCase
    {
        #region Формы слова
        #region Единственное число
        /// <summary>
        /// Именительный падеж единственное число - кто, что?
        /// </summary>
        [Key]
        [Required]
        public string NominativeSingle { get; set; }

        /// <summary>
        /// Родительный падеж единственное число - кого, чего?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveSingle { get; set; } = "";

        /// <summary>
        /// Дательный падеж единственное число - кому, чему?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeSingle { get; set; } = "";

        /// <summary>
        /// Винительный падеж единственное число - кого, что?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeSingle { get; set; } = "";

        /// <summary>
        /// Творительный падеж единственное число - кем, чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalSingle { get; set; } = "";

        /// <summary>
        /// Предложный падеж единственное число - о ком, о чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalSingle { get; set; } = "";
        #endregion

        #region Число 2/3/4
        /// <summary>
        /// Именительный падеж число 2/3/4 - кто, что?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string NominativeTwo { get; set; } = "";

        /// <summary>
        /// Родительный падеж число 2/3/4 - кого, чего?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveTwo { get; set; } = "";

        /// <summary>
        /// Дательный падеж число 2/3/4 - кому, чему?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeTwo { get; set; } = "";

        /// <summary>
        /// Винительный падеж число 2/3/4 - кого, что?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeTwo { get; set; } = "";

        /// <summary>
        /// Творительный падеж число 2/3/4 - кем, чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalTwo { get; set; } = "";

        /// <summary>
        /// Предложный падеж число 2/3/4 - о ком, о чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalTwo { get; set; } = "";
        #endregion

        #region Множественное число
        /// <summary>
        /// Именительный падеж множественное число - кто, что?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string NominativeMultiple { get; set; } = "";

        /// <summary>
        /// Родительный падеж множественное число - кого, чего?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveMultiple { get; set; } = "";

        /// <summary>
        /// Дательный падеж множественное число - кому, чему?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeMultiple { get; set; } = "";

        /// <summary>
        /// Винительный падеж множественное число - кого, что?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeMultiple { get; set; } = "";

        /// <summary>
        /// Творительный падеж множественное число - кем, чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalMultiple { get; set; } = "";

        /// <summary>
        /// Предложный падеж множественное число - о ком, о чем?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalMultiple { get; set; } = "";
        #endregion
        #endregion

        /// <summary>
        /// Показывает, обработана ли сервисом единственная форма числа.
        /// </summary>
        public bool IsNewSingle { get; set; }

        /// <summary>
        /// Показывает, обработана ли сервисом форма числа 2/3/4.
        /// </summary>
        public bool IsNewTwo { get; set; }

        /// <summary>
        /// Показывает, обработана ли сервисом множественная форма числа.
        /// </summary>
        public bool IsNewMultiple { get; set; }

        /// <summary>
        /// Возвращает структуру <see cref="WordCase"/>, заполненную словом <paramref name="word"/> во всех формах без склонения. Может использоваться в качестве заглушки.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static WordCase FromWord(string word)
        {
            return new WordCase()
            {
                NominativeSingle = word,
                NominativeTwo = word,
                NominativeMultiple = word,
                DativeSingle = word,
                DativeTwo = word,
                DativeMultiple = word,
                GenitiveSingle = word,
                GenitiveTwo = word,
                GenitiveMultiple = word,
                AccusativeSingle = word,
                AccusativeTwo = word,
                AccusativeMultiple = word,
                InstrumentalSingle = word,
                InstrumentalTwo = word,
                InstrumentalMultiple = word,
                PrepositionalSingle = word,
                PrepositionalTwo = word,
                PrepositionalMultiple = word
            };
        }

        private string GetWordCase(
            eNumeralType numeralType,
            Func<string> singleCaseCallback,
            Func<string> twoCaseCallback,
            Func<string> multipleCaseCallback
            )
        {
            var word = string.Empty;
            if (numeralType == eNumeralType.SingleType) word = singleCaseCallback();
            else if (numeralType == eNumeralType.TwoThreeFour) word = twoCaseCallback();
            else if (numeralType == eNumeralType.Multiple) word = multipleCaseCallback();

            if (string.IsNullOrEmpty(word)) word = singleCaseCallback();
            if (string.IsNullOrEmpty(word)) word = NominativeSingle;

            return word;
        }

        /// <summary>
        /// Возвращает именительный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="NominativeSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="NominativeTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="NominativeMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="NominativeTwo"/> или <see cref="NominativeMultiple"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetNominative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => NominativeSingle, () => NominativeTwo, () => NominativeMultiple);
        }

        /// <summary>
        /// Возвращает родительный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="GenitiveSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="GenitiveTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="GenitiveMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="GenitiveTwo"/> или <see cref="GenitiveMultiple"/> пусто, то возвращает <see cref="GenitiveSingle"/>.
        /// Если же <see cref="GenitiveSingle"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetGenitive(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => GenitiveSingle, () => GenitiveTwo, () => GenitiveMultiple);
        }

        /// <summary>
        /// Возвращает дательный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="DativeSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="DativeTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="DativeMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="DativeTwo"/> или <see cref="DativeMultiple"/> пусто, то возвращает <see cref="DativeSingle"/>.
        /// Если же <see cref="DativeSingle"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetDative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => DativeSingle, () => DativeTwo, () => DativeMultiple);
        }

        /// <summary>
        /// Возвращает винительный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="AccusativeSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="AccusativeTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="AccusativeMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="AccusativeTwo"/> или <see cref="AccusativeMultiple"/> пусто, то возвращает <see cref="AccusativeSingle"/>.
        /// Если же <see cref="AccusativeSingle"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetAccusative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => AccusativeSingle, () => AccusativeTwo, () => AccusativeMultiple);
        }

        /// <summary>
        /// Возвращает творительный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="InstrumentalSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="InstrumentalTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="InstrumentalMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="InstrumentalTwo"/> или <see cref="InstrumentalMultiple"/> пусто, то возвращает <see cref="InstrumentalSingle"/>.
        /// Если же <see cref="InstrumentalSingle"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetInstrumental(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => InstrumentalSingle, () => InstrumentalTwo, () => InstrumentalMultiple);
        }

        /// <summary>
        /// Возвращает предложный падеж в количественном варианте <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// Возвращает <see cref="PrepositionalSingle"/> для <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="PrepositionalTwo"/> для <see cref="eNumeralType.TwoThreeFour"/> либо 
        /// <see cref="PrepositionalMultiple"/> для <see cref="eNumeralType.Multiple"/>. 
        /// Если <paramref name="returnSingleFormIfEmpty"/> равен true и <see cref="PrepositionalTwo"/> или <see cref="PrepositionalMultiple"/> пусто, то возвращает <see cref="PrepositionalSingle"/>.
        /// Если же <see cref="PrepositionalSingle"/> пусто, то возвращает <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetPrepositional(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => PrepositionalSingle, () => PrepositionalTwo, () => PrepositionalMultiple);
        }


    }
}
