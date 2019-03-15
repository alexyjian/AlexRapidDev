namespace ALEXFW.Entity.CommonDictionary
{
    /// <summary>
    ///     列表表头参数
    /// </summary>
    public class ListColumnHeader
    {
        public string Title { get; set; }
        public string Width { get; set; }
        public string OrderSort { get; set; }
        public bool UseSortIndicator { get; set; }
        public string PropertyName { get; set; }
    }
}