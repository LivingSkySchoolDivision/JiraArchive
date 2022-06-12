namespace JiraArchive;
public class JiraIssueRepository
{
    private readonly string _connString = string.Empty;

    public JiraIssueRepository(string ConnectionString)
    {
        this._connString = ConnectionString;
    }

}
