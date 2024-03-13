#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using I2.Loc;

namespace HCGames.Utilities
{
    public static class Language
    {
        public static string GetString(string str)
        {
            var translation = LocalizationManager.GetTranslation(str);
            return string.IsNullOrEmpty(translation) ? str : translation;
        }
        
        public static string GetEnglishString(string str)
        {
            return LocalizationManager.GetTranslation(str, overrideLanguage: "English");
        }

        public static string GetRefreshesInString(double totalSeconds)
        {
            return string.Format(ScriptLocalization.Toast.RefreshesIn,GetRemainingTimeString(totalSeconds));
        }

        public static string GetShortRefreshesInString(double totalSeconds)
        {
            return string.Format(ScriptLocalization.Toast.RefreshesIn,GetShortRemainingTimeString(totalSeconds));
        }
        
        public static string GetRemainingTimeString(double totalSeconds){
            var remainingTime = TimeSpan.FromSeconds(totalSeconds);
            var days = remainingTime.Days;
            var hours = remainingTime.Hours;
            var minutes = remainingTime.Minutes;
            var seconds = remainingTime.Seconds;
            if (days > 0){
                return string.Format(ScriptLocalization.TimeFormat.DHMS,TimeToString(days),TimeToString(hours),TimeToString(minutes),TimeToString(seconds));
            }
            if (hours>0){
                return string.Format(ScriptLocalization.TimeFormat.HMS,TimeToString(hours),TimeToString(minutes),TimeToString(seconds));
            }
            return string.Format(ScriptLocalization.TimeFormat.MS,TimeToString(minutes),TimeToString(seconds));
        }

        public static string GetShortRemainingTimeString(double totalSeconds){
            var remainingTime = TimeSpan.FromSeconds(totalSeconds);
            var days = remainingTime.Days;
            var hours = remainingTime.Hours;
            var minutes = remainingTime.Minutes;
            var seconds = remainingTime.Seconds;
            if (days > 0){
                return string.Format(ScriptLocalization.TimeFormat.DH,TimeToString(days),TimeToString(hours));
            }
            if (hours>0){
                return string.Format(ScriptLocalization.TimeFormat.HM,TimeToString(hours),TimeToString(minutes));
            }
            return string.Format(ScriptLocalization.TimeFormat.MS,TimeToString(minutes),TimeToString(seconds));
        }
        
        private static string TimeToString(int time){
            if (time > 9){
                return ""+time;
            }
            return "0" + time;
        }
        
        public static string FormatNumber(this double numberToFormat, int decimalPlaces = 2)
        {
            // Check if the number is smaller than the smallest base value and format it accordingly.
            if (numberToFormat < 1000)
            {
                return System.Math.Round(numberToFormat, numberToFormat >= 10 ? 0 : decimalPlaces, MidpointRounding.ToEven).ToString(CultureInfo.InvariantCulture);
            }

            // Existing logic for larger numbers.
            var numberString = numberToFormat.ToString(CultureInfo.InvariantCulture);
            for (var i = 0; i < (int)NumberSuffix.Count; i++)
            {
                var currentValue = 1 * System.Math.Pow(10, i * 3);
                var suffixValue = Enum.GetName(typeof(NumberSuffix), i);
                if (i == 0) { suffixValue = string.Empty; }

                if (numberToFormat >= currentValue)
                {
                    numberString = $"{System.Math.Round(numberToFormat / currentValue, decimalPlaces, MidpointRounding.ToEven)}{suffixValue}";
                }
            }

            return numberString;
        }

        /// <summary> Suffixes for numbers based on how many digits they have left of the decimal point. </summary>
        /// <remarks> The order of the suffixes matters! </remarks>
        [ SuppressMessage("ReSharper", "UnusedMember.Local") ]
        private enum NumberSuffix
        {
            /// <summary> A placeholder if the value is under 1 thousand </summary>
            P,

            /// <summary> Thousand </summary>
            K,

            /// <summary> Million </summary>
            M,

            /// <summary> Billion </summary>
            B,

            /// <summary> Trillion </summary>
            T,

            /// <summary> Quadrillion </summary>
            Q,
            
            Count
        }

    }
}