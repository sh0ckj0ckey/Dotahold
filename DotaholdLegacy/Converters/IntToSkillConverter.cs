using System;
using Windows.UI.Xaml.Data;

namespace Dotahold.Converters
{
    internal class IntToSkillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string skill = value.ToString();
                    switch (skill)
                    {
                        case "1":
                            return "Normal";

                        case "2":
                            return "High";

                        case "3":
                            return "Very High";

                        default:
                            return skill;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}