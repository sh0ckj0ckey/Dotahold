using System;
using Windows.UI.Xaml.Data;


namespace Dotahold.Converters
{
    internal class LongToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value == null) return string.Empty;
                string time = value.ToString();
                if (string.IsNullOrEmpty(time) || time == "0") return string.Empty;

                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
                double duration = (System.Convert.ToInt64(ts.TotalSeconds) - System.Convert.ToInt64(time));
                int ago = 0;
                if (duration / 31536000 >= 1)
                {
                    ago = System.Convert.ToInt32(duration / 31536000);

                    if (ago <= 1)
                        time = "1 year ago";
                    else
                        time = ago + " years ago";
                }
                else if (duration / 2592000 >= 1)
                {
                    ago = System.Convert.ToInt32(duration / 2592000);

                    if (ago <= 1)
                        time = "1 month ago";
                    else
                        time = ago + " months ago";
                }
                //else if (duration / 604800 >= 1)
                //{
                //    ago = System.Convert.ToInt32(duration / 604800);

                //    if (ago <= 1)
                //        time = "1 week ago";
                //    else
                //        time = ago + " weeks ago";
                //}
                else if (duration / 86400 >= 1)
                {
                    ago = System.Convert.ToInt32(duration / 86400);

                    if (ago <= 1)
                        time = "1 day ago";
                    else
                        time = ago + " days ago";
                }
                else if (duration / 3600 >= 1)
                {
                    ago = System.Convert.ToInt32(duration / 3600);

                    if (ago <= 1)
                        time = "1 hour ago";
                    else
                        time = ago + " hours ago";
                }
                else if (duration / 60 >= 1)
                {
                    ago = System.Convert.ToInt32(duration / 60);

                    if (ago <= 1)
                        time = "1 minute ago";
                    else
                        time = ago + " minutes ago";
                }
                else
                {
                    ago = System.Convert.ToInt32(duration);

                    if (ago <= 1)
                        time = "1 second ago";
                    else
                        time = ago + " seconds ago";
                }
                return time;
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
