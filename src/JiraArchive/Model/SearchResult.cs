
namespace JiraArchive;

public class SearchResult
{
    public int Score { get; set; }
    public JiraIssue Issue { get; set; }

    public bool ShowComments { get; set; }
}