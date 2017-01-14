using UCS.Files.CSV;

namespace UCS.Files.Logic
{
    internal class BuildingClassesData : Data
    {
        public BuildingClassesData(CSVRow row, DataTable dt) : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public bool CanBuy { get; set; }
        public string Name { get; set; }
        public bool ShopCategoryArmy { get; set; }
        public bool ShopCategoryDefense { get; set; }
        public bool ShopCategoryResource { get; set; }
        public string TID { get; set; }
    }
}
