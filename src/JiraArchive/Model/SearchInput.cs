namespace JiraArchive;
public class SearchInput
{
    public string RawSearchTerms { get; set; }
    public List<SearchResult> Results { get; set; } = new List<SearchResult>();
}
