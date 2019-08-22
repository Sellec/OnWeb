namespace System.ComponentModel.DataAnnotations
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class PhoneBuilder
    {
        public static PhoneBuilder ParseString(string potentialPhoneNumber)
        {
            return new PhoneBuilder(potentialPhoneNumber);
        }

        private PhoneBuilder(string potentialPhoneNumber)
        {
            Source = potentialPhoneNumber;

            if (string.IsNullOrEmpty(potentialPhoneNumber)) Error = "Переданное значение не является номером телефона.";
            else
            {
                var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();

                try
                {
                    var phoneParsed = phoneUtil.Parse(potentialPhoneNumber, "RU");
                    if (phoneUtil.IsValidNumber(phoneParsed))
                    {
                        ParsedPhoneNumber = phoneUtil.Format(phoneParsed, PhoneNumbers.PhoneNumberFormat.E164);
                        IsCorrect = true;
                    }
                    else throw new Exception("Некорректный номер телефона.");
                }
                catch (PhoneNumbers.NumberParseException ex)
                {
                    switch (ex.ErrorType)
                    {
                        case PhoneNumbers.ErrorType.INVALID_COUNTRY_CODE:
                            Error = "Неизвестный код страны после знака '+'.";
                            break;

                        case PhoneNumbers.ErrorType.NOT_A_NUMBER:
                            Error = "Переданное значение не является номером телефона.";
                            break;

                        case PhoneNumbers.ErrorType.TOO_LONG:
                            Error = "Номер телефона не может быть длиннее 250 символов.";
                            break;

                        case PhoneNumbers.ErrorType.TOO_SHORT_NSN:
                            Error = "Слишком короткое для номера телефона";
                            break;

                        case PhoneNumbers.ErrorType.TOO_SHORT_AFTER_IDD:
                            Error = "Часть номера после префикса слишком короткая";
                            break;

                        default:
                            Error = ex.Message;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
        }

        public bool IsCorrect { get; private set; } = false;

        public string Source { get; private set; }

        public string ParsedPhoneNumber { get; private set; } = string.Empty;

        public string Error { get; private set; } = string.Empty;

        public override string ToString()
        {
            return ParsedPhoneNumber;
        }
    }
}
