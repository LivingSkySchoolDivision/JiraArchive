namespace JiraArchive;
public class JiraIssueComment
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string Author { get; set; }
    public string ActionType { get; set; }
    public string Content { get; set; } // ActionBody
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
