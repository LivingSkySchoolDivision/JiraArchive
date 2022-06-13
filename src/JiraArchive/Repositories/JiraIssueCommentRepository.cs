using System.Data;
using System.Data.SqlClient;

namespace JiraArchive;
public class JiraIssueCommentRepository
{

    private readonly string _connString = string.Empty;

    public JiraIssueCommentRepository(string ConnectionString)
    {
        this._connString = ConnectionString;

    }


    public List<JiraIssueComment> GetForIssue(int issueId)
    {
        /*
        if (_cache.ContainsKey(issueId))
        {
            return _cache[issueId];
        } else {
            return new List<JiraIssueComment>();
        }
        */
        return new List<JiraIssueComment>();
    }

    private JiraIssueComment dataReaderToObject(SqlDataReader dataReader)
    {
        return new JiraIssueComment()
        {
            Id = dataReader["ID"].ToString().Trim().ToInt(),
            IssueId = dataReader["issueid"].ToString().Trim().ToInt(),
            Author = dataReader["AUTHOR"].ToString().Trim(),
            ActionType = dataReader["actiontype"].ToString().Trim(),
            Content = dataReader["actionbody"].ToString().Trim(),
            Created = dataReader["CREATED"].ToString().Trim().ToDateTime(),
            Updated = dataReader["UPDATED"].ToString().Trim().ToDateTime()
        };
    }


    public List<int> FindInCommentsAndGetIssueIds(List<string> terms)
    {
        List<int> issueIDs = new List<int>();

        using (SqlConnection connection = new SqlConnection(_connString))
        {
            foreach(string term in terms)
            {
                if (!string.IsNullOrEmpty(term))
                {
                    string adjustedTerm = "%" + term + "%";
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = connection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText =    "SELECT " +
                                                    "    ID, " +
                                                    "    issueid, " +
                                                    "    AUTHOR, " +
                                                    "    actiontype," +
                                                    "    actionbody, " +
                                                    "    CREATED, " +
                                                    "    UPDATED " +
                                                    "FROM " +
                                                    "    jiraaction " +
                                                    "WHERE " +
                                                    "    actionbody LIKE @TERM ";
                        sqlCommand.Parameters.AddWithValue("TERM", adjustedTerm);
                        sqlCommand.Connection.Open();
                        SqlDataReader dataReader = sqlCommand.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                int issueID = dataReader["issueid"].ToString().Trim().ToInt();
                                if (!issueIDs.Contains(issueID))
                                {
                                    issueIDs.Add(issueID);
                                }
                            }
                        }
                        sqlCommand.Connection.Close();
                    }
                }
            }
        }
        return issueIDs;
    }

    public List<JiraIssue> LoadCommentsForIssues(List<JiraIssue> Issues)
    {
        Dictionary<int, List<JiraIssueComment>> allComments = GetForIssues(Issues.Select(x => x.Id).ToList());

        foreach(JiraIssue issue in Issues)
        {
            if (allComments.ContainsKey(issue.Id))
            {
                issue.Comments = allComments[issue.Id];
            }
        }

        return Issues;
    }

    public Dictionary<int,List<JiraIssueComment>> GetForIssues(List<int> IssueIDs)
    {
        Dictionary<int,List<JiraIssueComment>> returnMe = new Dictionary<int,List<JiraIssueComment>>();

        using (SqlConnection connection = new SqlConnection(_connString))
        {
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT ID,issueid,AUTHOR,actiontype,actionbody,CREATED,UPDATED FROM jiraaction WHERE issueid IN (" + IssueIDs.ToCommaSeparatedString() + ") ORDER BY CREATED";
                sqlCommand.Connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        JiraIssueComment x = dataReaderToObject(dataReader);
                        if (x != null)
                        {
                            if (!returnMe.ContainsKey(x.IssueId))
                            {
                                returnMe.Add(x.IssueId, new List<JiraIssueComment>());
                            }
                            returnMe[x.IssueId].Add(x);
                        }
                    }
                }
                sqlCommand.Connection.Close();
            }
        }

        return returnMe;
    }
}
