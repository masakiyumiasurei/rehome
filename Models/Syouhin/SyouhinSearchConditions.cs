namespace rehome.Models
{
    public class SyouhinSearchConditions
    {
        public int page { get; set; }

        public int? 商品ID { get; set; }
        public string? 商品名 { get; set; }
        public string? メーカー名 { get; set; }
        public string? 品番 { get; set; }

        public bool 削除フラグ { get; set; }
    }
}
