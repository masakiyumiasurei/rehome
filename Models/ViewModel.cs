namespace rehome.Models
{
    // ドロップダウンリストの選択肢を表す ViewModel です。

    public class YearViewModel
    {
        // 選択肢の値をセットします。
        public int Value { get; set; }

        // 選択肢として表示するテキストを取得するためのプロパティ。
        // ここでは値に" 年"を足して、 "2016 年" と表示されるようにしています。
        public string DisplayText
        {
            get
            {
                return $"{this.Value} 年";
            }
        }
    }
}
