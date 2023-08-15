namespace Boutique.Areas.Admin.Models.ToDo;

public class Filters
{
    public Filters(string filterString)
    {
        FilterString = filterString ?? "all-all-all"; // string.Empty;
        string[] filters = FilterString.Split('-');
        CategoryId = filters[0];
        StatusId = filters[2];
        Due = filters[1];
    }

    public string FilterString { get; }

    public string CategoryId { get; }
    public string StatusId { get; }

    public string Due { get; }

    public bool HasCategory => CategoryId.ToLower() != "all";
    public bool HasStatus => StatusId.ToLower() != "all";
    public bool HasDue => Due.ToLower() != "all";

    public static Dictionary<string, string> DueFilterValues =>
        new()
        {
            {"future", "Future" },
            {"past", "Past" },
            {"today", "Today" },
        };

    public bool IsPast => Due.ToLower() == "past";
    public bool IsFuture => Due.ToLower() == "future";
    public bool IsToday => Due.ToLower() == "today";

    
}