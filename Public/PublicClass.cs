using rehome.Models;
using System.Security.Cryptography;
using System.Text;

namespace rehome.Public
{
    public static class PublicClass
    {
        static readonly SHA256CryptoServiceProvider hashProvider = new SHA256CryptoServiceProvider();


        public static string GetSHA256HashedString(string value)
        => string.Join("", hashProvider.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => $"{x:x2}"));


        // 年の選択肢を取得するためのメソッドです。
        public static IEnumerable<YearViewModel> GetYearOptions()
        {
            // 直近の 10 年+来年を選択肢として取得する。
            return Enumerable
                .Range(DateTime.Now.Year - 9, 11)
                .Select(t => new YearViewModel() { Value = t });
        }



        // 月の選択肢を取得するためのメソッドです。
        //public static IEnumerable<MonthViewModel> GetMonthOptions()
        //{
        //    // 1 月から 12 月までを選択肢として取得する。
        //    return Enumerable
        //        .Range(1, 12)
        //        .Select(t => new MonthViewModel() { Value = t });
        //}
    }
}
