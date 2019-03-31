namespace DaData.Entities
{
    /// <summary>
    /// "As is" entity.
    /// </summary>
    class AsIsData : IDadataEntity
    {
        public string source { get; set; }

        public StructureType structure_type
        {
            get { return StructureType.AS_IS; }
        }

        public override string ToString()
        {
            return string.Format("[AsIsData: source={0}]", source);
        }
    }

}
