using System;

namespace BusinessLogic.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime datebirth)
        {
            var today = DateTime.Today;
            var age = today.Year - datebirth.Year;
            if (datebirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}