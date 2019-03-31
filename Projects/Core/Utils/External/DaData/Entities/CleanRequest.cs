using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaData.Entities
{
    /// <summary>
    /// Clean request.
    /// </summary>
    class CleanRequest
    {
        public IEnumerable<StructureType> structure { get; set; }

        public IEnumerable<IEnumerable<string>> data { get; set; }

        public CleanRequest(IEnumerable<StructureType> structure, IEnumerable<IEnumerable<string>> data)
        {
            this.structure = structure;
            this.data = data;
        }

        public override string ToString()
        {
            return string.Format("[CleanQuery: structure={0}, data={1}]", string.Join(", ", structure), string.Join("\n", data));
        }
    }

}
