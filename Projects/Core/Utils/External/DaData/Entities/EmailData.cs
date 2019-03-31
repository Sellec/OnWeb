﻿namespace DaData.Entities
{
    class EmailData : IDadataEntity
    {
        public string source { get; set; }
        public string email { get; set; }
        public string qc { get; set; }

        public StructureType structure_type
        {
            get { return StructureType.EMAIL; }
        }

        public override string ToString()
        {
            return string.Format("[EmailData: source={0}, email={1}, qc={2}]", source, email, qc);
        }
    }

}
