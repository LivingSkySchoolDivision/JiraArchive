using System.Data;
using System.Data.SqlClient;

namespace JiraArchive;
public class JiraIssueRepository
{
    private readonly string _connString = string.Empty;
    private JiraIssueCommentRepository _commentRepo;

    public JiraIssueRepository(string ConnectionString)
    {
        this._connString = ConnectionString;
        this._commentRepo = new JiraIssueCommentRepository(ConnectionString);
    }


    // This just finds issues that contain any of the terms
    // The separate "search" system will handle ranking and collating
    public List<JiraIssue> Find(List<string> terms)
    {
        Dictionary<string, JiraIssue> results = new Dictionary<string, JiraIssue>();
        using (SqlConnection connection = new SqlConnection(_connString))
        {
            foreach(string term in terms)
            {
                string modifiedTerm = "%" + term + "%";
                if(!string.IsNullOrEmpty(term))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = connection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText =    "SELECT " +
                                                    "    i.ID, " +
                                                    "    concat(p.pkey, '-', i.issuenum) as issuekey, " +
                                                    "    i.REPORTER, " +
                                                    "    i.ASSIGNEE, " +
                                                    "    i.SUMMARY, " +
                                                    "    i.DESCRIPTION, " +
                                                    "    i.CREATED, " +
                                                    "    i.UPDATED, " +
                                                    "    i.RESOLUTIONDATE " +
                                                    "FROM " +
                                                    "    jiraissue as i " +
                                                    "    LEFT OUTER JOIN project as p ON i.PROJECT = p.Id " +
                                                    "WHERE " +
                                                    "    ( " +
                                                    "        (concat(p.pkey, '-', i.issuenum) LIKE @TERM) " +
                                                    "        OR i.REPORTER LIKE @TERM " +
                                                    "        OR i.SUMMARY LIKE @TERM " +
                                                    "        OR i.DESCRIPTION LIKE @TERM " +
                                                    "    ); ";
                        sqlCommand.Parameters.AddWithValue("TERM",modifiedTerm);
                        sqlCommand.Connection.Open();
                        SqlDataReader dataReader = sqlCommand.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                JiraIssue x = dataReaderToObject(dataReader);
                                if (x != null)
                                {
                                    if (!results.ContainsKey(x.Key))
                                    {
                                        results.Add(x.Key, x);
                                    }
                                }
                            }
                        }
                        sqlCommand.Connection.Close();
                    }
                }
            }
        }

        return _commentRepo.LoadCommentsForIssues(results.Values.ToList());
    }

    private JiraIssue dataReaderToObject(SqlDataReader dataReader)
    {
        return new JiraIssue()
        {
            Id = dataReader["ID"].ToString().Trim().ToInt(),
            Key = dataReader["issuekey"].ToString().Trim(),
            Reporter = dataReader["REPORTER"].ToString().Trim(),
            Assignee = dataReader["ASSIGNEE"].ToString().Trim(),
            Summary = dataReader["SUMMARY"].ToString().Trim(),
            Description = dataReader["DESCRIPTION"].ToString().Trim(),
            Created = dataReader["CREATED"].ToString().Trim().ToDateTime(),
            Updated = dataReader["UPDATED"].ToString().Trim().ToDateTime(),
            Resolved = dataReader["RESOLUTIONDATE"].ToString().Trim().ToDateTime(),
            Comments = _commentRepo.GetForIssue(dataReader["ID"].ToString().Trim().ToInt())
        };
    }

    public List<JiraIssue> Get(List<int> issueIDs)
    {
        List<JiraIssue> returnMe = new List<JiraIssue>();

        using (SqlConnection connection = new SqlConnection(_connString))
        {
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText =    "SELECT " +
                                            "    i.ID, " +
                                            "    concat(p.pkey, '-', i.issuenum) as issuekey, " +
                                            "    i.REPORTER, " +
                                            "    i.ASSIGNEE, " +
                                            "    i.SUMMARY, " +
                                            "    i.DESCRIPTION, " +
                                            "    i.CREATED, " +
                                            "    i.UPDATED, " +
                                            "    i.RESOLUTIONDATE " +
                                            "FROM " +
                                            "    jiraissue as i " +
                                            "    LEFT OUTER JOIN project as p ON i.PROJECT = p.Id " +
                                            "WHERE " +
                                            "    ( " +
                                            "        i.ID IN (" + issueIDs.ToCommaSeparatedString() +") " +
                                            "    ); ";
                sqlCommand.Connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        JiraIssue x = dataReaderToObject(dataReader);
                        if (x != null)
                        {
                            returnMe.Add(x);
                        }
                    }
                }
                sqlCommand.Connection.Close();
            }
        }

        return _commentRepo.LoadCommentsForIssues(returnMe);
    }

}
