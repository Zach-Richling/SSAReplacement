namespace SSAReplacement.Web.Extensions;

public static class DateTimeExtensions
{
    public static string ToTimeAgo(this DateTime dateTime)
    {
        var diff = DateTime.UtcNow - dateTime;

        var sec = (int)diff.TotalSeconds;
        if (sec < 10)
            return "just now";

        if (sec < 60)
            return sec + " seconds ago";

        var min = (int)(sec / 60);
        if (min < 60)
            return min + (min == 1 ? " minute ago" : " minutes ago");

        var hr = (int)(min / 60);
        if (hr < 24)
            return hr + (hr == 1 ? " hour ago" : " hours ago");

        var day = (int)(hr / 24);
        return day + (day == 1 ? " day ago" : " days ago");
    }
}
