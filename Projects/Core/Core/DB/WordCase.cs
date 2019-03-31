namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Plugins.Lexicon;

    /// <summary>
    /// ������ ���������� � ������� �����.
    /// </summary>
    [Table("WordCase")]
    public partial class WordCase
    {
        #region ����� �����
        #region ������������ �����
        /// <summary>
        /// ������������ ����� ������������ ����� - ���, ���?
        /// </summary>
        [Key]
        [Required]
        public string NominativeSingle { get; set; }

        /// <summary>
        /// ����������� ����� ������������ ����� - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveSingle { get; set; } = "";

        /// <summary>
        /// ��������� ����� ������������ ����� - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeSingle { get; set; } = "";

        /// <summary>
        /// ����������� ����� ������������ ����� - ����, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeSingle { get; set; } = "";

        /// <summary>
        /// ������������ ����� ������������ ����� - ���, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalSingle { get; set; } = "";

        /// <summary>
        /// ���������� ����� ������������ ����� - � ���, � ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalSingle { get; set; } = "";
        #endregion

        #region ����� 2/3/4
        /// <summary>
        /// ������������ ����� ����� 2/3/4 - ���, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string NominativeTwo { get; set; } = "";

        /// <summary>
        /// ����������� ����� ����� 2/3/4 - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveTwo { get; set; } = "";

        /// <summary>
        /// ��������� ����� ����� 2/3/4 - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeTwo { get; set; } = "";

        /// <summary>
        /// ����������� ����� ����� 2/3/4 - ����, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeTwo { get; set; } = "";

        /// <summary>
        /// ������������ ����� ����� 2/3/4 - ���, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalTwo { get; set; } = "";

        /// <summary>
        /// ���������� ����� ����� 2/3/4 - � ���, � ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalTwo { get; set; } = "";
        #endregion

        #region ������������� �����
        /// <summary>
        /// ������������ ����� ������������� ����� - ���, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string NominativeMultiple { get; set; } = "";

        /// <summary>
        /// ����������� ����� ������������� ����� - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string GenitiveMultiple { get; set; } = "";

        /// <summary>
        /// ��������� ����� ������������� ����� - ����, ����?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string DativeMultiple { get; set; } = "";

        /// <summary>
        /// ����������� ����� ������������� ����� - ����, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string AccusativeMultiple { get; set; } = "";

        /// <summary>
        /// ������������ ����� ������������� ����� - ���, ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string InstrumentalMultiple { get; set; } = "";

        /// <summary>
        /// ���������� ����� ������������� ����� - � ���, � ���?
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string PrepositionalMultiple { get; set; } = "";
        #endregion
        #endregion

        /// <summary>
        /// ����������, ���������� �� �������� ������������ ����� �����.
        /// </summary>
        public bool IsNewSingle { get; set; }

        /// <summary>
        /// ����������, ���������� �� �������� ����� ����� 2/3/4.
        /// </summary>
        public bool IsNewTwo { get; set; }

        /// <summary>
        /// ����������, ���������� �� �������� ������������� ����� �����.
        /// </summary>
        public bool IsNewMultiple { get; set; }

        /// <summary>
        /// ���������� ��������� <see cref="WordCase"/>, ����������� ������ <paramref name="word"/> �� ���� ������ ��� ���������. ����� �������������� � �������� ��������.
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
        /// ���������� ������������ ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="NominativeSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="NominativeTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="NominativeMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="NominativeTwo"/> ��� <see cref="NominativeMultiple"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetNominative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => NominativeSingle, () => NominativeTwo, () => NominativeMultiple);
        }

        /// <summary>
        /// ���������� ����������� ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="GenitiveSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="GenitiveTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="GenitiveMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="GenitiveTwo"/> ��� <see cref="GenitiveMultiple"/> �����, �� ���������� <see cref="GenitiveSingle"/>.
        /// ���� �� <see cref="GenitiveSingle"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetGenitive(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => GenitiveSingle, () => GenitiveTwo, () => GenitiveMultiple);
        }

        /// <summary>
        /// ���������� ��������� ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="DativeSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="DativeTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="DativeMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="DativeTwo"/> ��� <see cref="DativeMultiple"/> �����, �� ���������� <see cref="DativeSingle"/>.
        /// ���� �� <see cref="DativeSingle"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetDative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => DativeSingle, () => DativeTwo, () => DativeMultiple);
        }

        /// <summary>
        /// ���������� ����������� ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="AccusativeSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="AccusativeTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="AccusativeMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="AccusativeTwo"/> ��� <see cref="AccusativeMultiple"/> �����, �� ���������� <see cref="AccusativeSingle"/>.
        /// ���� �� <see cref="AccusativeSingle"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetAccusative(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => AccusativeSingle, () => AccusativeTwo, () => AccusativeMultiple);
        }

        /// <summary>
        /// ���������� ������������ ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="InstrumentalSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="InstrumentalTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="InstrumentalMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="InstrumentalTwo"/> ��� <see cref="InstrumentalMultiple"/> �����, �� ���������� <see cref="InstrumentalSingle"/>.
        /// ���� �� <see cref="InstrumentalSingle"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetInstrumental(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => InstrumentalSingle, () => InstrumentalTwo, () => InstrumentalMultiple);
        }

        /// <summary>
        /// ���������� ���������� ����� � �������������� �������� <paramref name="numeralType"/>.
        /// </summary>
        /// <returns>
        /// ���������� <see cref="PrepositionalSingle"/> ��� <see cref="eNumeralType.SingleType"/>, 
        /// <see cref="PrepositionalTwo"/> ��� <see cref="eNumeralType.TwoThreeFour"/> ���� 
        /// <see cref="PrepositionalMultiple"/> ��� <see cref="eNumeralType.Multiple"/>. 
        /// ���� <paramref name="returnSingleFormIfEmpty"/> ����� true � <see cref="PrepositionalTwo"/> ��� <see cref="PrepositionalMultiple"/> �����, �� ���������� <see cref="PrepositionalSingle"/>.
        /// ���� �� <see cref="PrepositionalSingle"/> �����, �� ���������� <see cref="NominativeSingle"/>.
        /// </returns>
        public string GetPrepositional(eNumeralType numeralType, bool returnSingleFormIfEmpty = true)
        {
            var word = string.Empty;
            return GetWordCase(numeralType, () => PrepositionalSingle, () => PrepositionalTwo, () => PrepositionalMultiple);
        }


    }
}
