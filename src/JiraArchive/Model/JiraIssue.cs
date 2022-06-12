namespace JiraArchive;
public class JiraIssue
{
    public int Id { get; set; }
    public string Reporter { get; set; }
    public string Assignee { get; set; }
    public string IssueType { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Resolved { get; set; }


}
