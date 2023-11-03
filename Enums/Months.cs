using System.ComponentModel.DataAnnotations;
namespace rehome.Enums
{
    // 月の選択肢の元となる Enum。
    public enum Months
    {
        [Display(Name = "1 月")]
        January = 1,
        [Display(Name = "2 月")]
        February = 2,
        [Display(Name = "3 月")]
        March = 3,
        [Display(Name = "4 月")]
        April = 4,
        [Display(Name = "5 月")]
        May = 5,
        [Display(Name = "6 月")]
        June = 6,
        [Display(Name = "7 月")]
        July = 7,
        [Display(Name = "8 月")]
        August = 8,
        [Display(Name = "9 月")]
        September = 9,
        [Display(Name = "10 月")]
        October = 10,
        [Display(Name = "11 月")]
        November = 11,
        [Display(Name = "12 月")]
        December = 12
    }
}