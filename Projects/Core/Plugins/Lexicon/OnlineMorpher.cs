using System;

using SimpleHelpers;

namespace OnWeb.Plugins.Lexicon
{
    class OnlineMorpher
    {
        private DateTime? _timeoutUntil = null;

        public class WordCases
        {
            public string И { get; set; }
            public string Р { get; set; }
            public string Д { get; set; }
            public string В { get; set; }
            public string Т { get; set; }
            public string П { get; set; }
        }

        class PropisResult
        {
            public WordCases n { get; set; } = new WordCases();
            public WordCases unit { get; set; } = new WordCases();
        }

        public WordCases GetNumeralResult(string word, eNumeralType numeralType)
        {
            if (_timeoutUntil.HasValue)
            {
                if (_timeoutUntil.Value > DateTime.Now) return null;
                _timeoutUntil = null;
            }

            try
            {
                int count = numeralType == eNumeralType.SingleType ? 1 : (numeralType == eNumeralType.TwoThreeFour ? 2 : 5);

                var client = new System.Net.WebClient();
                var defaultWebProxy = System.Net.WebRequest.DefaultWebProxy;
                defaultWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                client.Proxy = defaultWebProxy;

                var url = $"https://ws3.morpher.ru/russian/spell?n={count}&unit={word}&format=json";

                var answerBytes = client.DownloadData(url);
                var answerEncoding = FileEncoding.DetectFileEncoding(answerBytes, 0, answerBytes.Length);
                var answerDecoded = answerEncoding.GetString(answerBytes);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PropisResult>(answerDecoded);

                return result.unit;
            }
            catch (Exception ex)
            {
                _timeoutUntil = DateTime.Now.AddMinutes(15);

                Debug.WriteLine($"GetNumeralResult: '{word}', {numeralType}: {ex.Message}");
                return null; 
            }
        }
    }
}
