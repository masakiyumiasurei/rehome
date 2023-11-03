using System.ComponentModel.DataAnnotations;

namespace rehome.Models
{
    public class SaturdayAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"土曜日を指定してください";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            // 入力値が空の場合は検証をスキップ 
            if (value == null) { return ValidationResult.Success; }

            dynamic D = value;
            // valueが土曜日か判別
            if(D.DayOfWeek== DayOfWeek.Saturday)
            //if ((string.Format("{0:ddd}", value) == "土")|| (string.Format("{0:ddd}", value) == "Sat"))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(GetErrorMessage());
        }
    }

    //public class KubunBikouAttribute : ValidationAttribute
    //{
    //    public string GetErrorMessage() => $"区分が「その他」の場合は区分詳細を入力してください。";

    //    protected override ValidationResult IsValid(object value,
    //        ValidationContext validationContext)
    //    {
    //        // 入力値が空の場合は検証をスキップ 
    //        if (value == null) { return ValidationResult.Success; }

    //        return new ValidationResult(GetErrorMessage());
    //    }
    //}
}
