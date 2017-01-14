using UCS.Files.CSV;

namespace UCS.Files.Logic
{
    internal class MissionsData : Data
    {
        public MissionsData(CSVRow row, DataTable dt) : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public string Name { get; set; }
    }
}
