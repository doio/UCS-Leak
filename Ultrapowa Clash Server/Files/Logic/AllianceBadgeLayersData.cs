using UCS.Files.CSV;

namespace UCS.Files.Logic
{
    internal class AllianceBadgeLayersData : Data
    {
        public AllianceBadgeLayersData(CSVRow row, DataTable dt) : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public string ExportName { get; set; }
        public string Name { get; set; }
        public int RequiredClanLevel { get; set; }
        public string SWF { get; set; }
        public string Type { get; set; }
    }
}
