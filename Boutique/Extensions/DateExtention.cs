namespace Boutique.Extensions
{
    public static class DateExtention
    {
        //datetime usage: @item.Date_Of_Birth.ToShortDateString();
        public static string ToLongDateString(this DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            return date.Value.ToString("yyyy-MM-ddThh:mm:ss");
        }
        
        public static string ToShortDateString(this DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            return date.Value.ToString("MM/dd/yyyy"); 
        }
    }
    
}
