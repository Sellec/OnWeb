namespace OnWeb.Modules.Lexicon
{
    /// <summary>
    /// Обозначает тип числительного слова.
    /// </summary>
    public enum eNumeralType : byte
    {
        /// <summary>
        /// Применяется для формы слова в единственном числе.
        /// </summary>
        SingleType = 0,

        /// <summary>
        /// Применяется для формы слова в количестве 2, 3 или 4.
        /// </summary>
        TwoThreeFour = 1,

        /// <summary>
        /// Применяется для формы слова во всех остальных количественных случаях.
        /// </summary>
        Multiple = 2,
    }


}
