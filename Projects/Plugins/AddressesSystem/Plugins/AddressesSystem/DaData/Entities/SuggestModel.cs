using System.Collections.Generic;

namespace DaData.Entities.Suggestions
{
    class SuggestQuery
    {
        public string query { get; set; }
        public int count { get; set; }
        public SuggestQuery(string query)
        {
            this.query = query;
        }
    }

    class AddressSuggestQuery : SuggestQuery
    {
        public AddressData[] locations { get; set; }
        public AddressData[] locations_boost { get; set; }
        public AddressBound from_bound { get; set; }
        public AddressBound to_bound { get; set; }
        public bool restrict_value { get; set; }
        public AddressSuggestQuery(string query) : base(query) { }
    }

    class BankSuggestQuery : SuggestQuery
    {
        public PartyStatus[] status { get; set; }
        public BankType[] type { get; set; }
        public BankSuggestQuery(string query) : base(query) { }
    }

    class FioSuggestQuery : SuggestQuery
    {
        public FioPart[] parts { get; set; }
        public FioSuggestQuery(string query) : base(query) { }
    }

    class PartySuggestQuery : SuggestQuery
    {
        public AddressData[] locations { get; set; }
        public AddressData[] locations_boost { get; set; }
        public PartyStatus[] status { get; set; }
        public PartyType? type { get; set; }
        public PartySuggestQuery(string query) : base(query) { }
    }

    class GeolocationQuery : SuggestQuery
    {
        public string ip { get; set; }
        public GeolocationQuery(string query) : base(query) { }
    }



    class AddressBound
    {
        public string value { get; set; }
        public AddressBound(string name)
        {
            this.value = name;
        }
    }

    class BankData
    {
        public AddressData address { get; set; }
        public string bic { get; set; }
        public string correspondent_account { get; set; }
        public BankNameData name { get; set; }
        public string okpo { get; set; }
        public BankOpfData opf { get; set; }
        public string phone { get; set; }
        public string registration_number { get; set; }
        public string rkc { get; set; }
        public PartyStateData state { get; set; }
        public string swift { get; set; }
    }

    class BankNameData
    {
        public string payment { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    class BankOpfData
    {
        public BankType type { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    enum BankType
    {
        BANK,
        NKO,
        BANK_BRANCH,
        NKO_BRANCH,
        RKC,
        OTHER
    }

    class EmailData
    {
        public string value { get; set; }
        public string local { get; set; }
        public string domain { get; set; }
    }

    class FioData
    {
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public string gender { get; set; }
    }

    enum FioPart
    {
        SURNAME,
        NAME,
        PATRONYMIC
    }

    class PartyData
    {
        public Suggestion<AddressData> address { get; set; }
        public string branch_count { get; set; }
        public PartyBranchType branch_type { get; set; }
        public string inn { get; set; }
        public string kpp { get; set; }
        public PartyManagementData management { get; set; }
        public PartyNameData name { get; set; }
        public string ogrn { get; set; }
        public string okpo { get; set; }
        public string okved { get; set; }
        public PartyOpfData opf { get; set; }
        public PartyStateData state { get; set; }
        public PartyType type { get; set; }
    }

    enum PartyBranchType
    {
        MAIN,
        BRANCH
    }

    class PartyManagementData
    {
        public string name { get; set; }
        public string post { get; set; }
    }

    class PartyNameData
    {
        public string full_with_opf { get; set; }
        public string short_with_opf { get; set; }
        public string latin { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    class PartyOpfData
    {
        public string code { get; set; }
        public string full { get; set; }
        public string @short { get; set; }
    }

    class PartyStateData
    {
        public string actuality_date { get; set; }
        public string registration_date { get; set; }
        public string liquidation_date { get; set; }
        public PartyStatus status { get; set; }
    }

    enum PartyStatus
    {
        ACTIVE,
        LIQUIDATING,
        LIQUIDATED
    }

    enum PartyType
    {
        LEGAL,
        INDIVIDUAL
    }

    class Suggestion<TData>
    {
        public TData data { get; set; }

        public string value { get; set; }

        public override string ToString()
        {
            return value;
        }
    }

    class SuggestAddressResponse
    {
        public List<Suggestion<AddressData>> suggestions { get; set; }
    }

    class SuggestBankResponse
    {
        public List<Suggestion<BankData>> suggestions { get; set; }
    }

    class SuggestEmailResponse
    {
        public List<Suggestion<EmailData>> suggestions { get; set; }
    }

    class SuggestFioResponse
    {
        public List<Suggestion<FioData>> suggestions { get; set; }
    }

    class SuggestPartyResponse
    {
        public List<Suggestion<PartyData>> suggestions { get; set; }
    }

    class GeolocationResponse
    {
        public Suggestion<AddressData> location { get; set; }
    }


}
