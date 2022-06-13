using System.Data;
using System.Data.SqlClient;

namespace JiraArchive;
public class JiraIssueSearcher
{
     private const int exactMatchScore = 500;
    private const int fuzzyMatchScore = 10;

    private JiraIssueRepository _issueRepo;
    private JiraIssueCommentRepository _commentRepo;

    public JiraIssueSearcher(JiraIssueRepository issueRepo, JiraIssueCommentRepository commentRepo)
    {
        this._issueRepo = issueRepo;
        this._commentRepo = commentRepo;
    }

    public List<SearchResult> Find(string rawTerms)
    {
        // Do nothing if there are no search terms
        if (string.IsNullOrEmpty(rawTerms.Trim())) { return new List<SearchResult>(); }

        List<SearchResult> results = new List<SearchResult>();

        // Split the search terms
        List<string> processedSearchTerms = rawTerms.Trim().Split(new char[] { ' ', ',', ';'}).ToList();

        if (processedSearchTerms.Count > 0)
        {
            List<JiraIssue> firstStageResults = _issueRepo.Find(processedSearchTerms);

            // Process each search result
            foreach(JiraIssue issue in firstStageResults)
            {
                int score = 0;

                foreach(string term in processedSearchTerms)
                {
                    string term_lower = term.ToLower();

                    if (!string.IsNullOrEmpty(term))
                    {

                        // Exact match scores
                        if (issue.Key.ToLower() == term_lower) { score += exactMatchScore; }
                        if (issue.Reporter.ToLower() == term_lower) { score += exactMatchScore; }
                        if (issue.Summary.ToLower() == term_lower) { score += exactMatchScore; }

                        // Fuzzy match scores
                        if (issue.Summary.ToLower().Contains(term_lower)) { score += fuzzyMatchScore; }
                        if (issue.Description.ToLower().Contains(term_lower)) { score += fuzzyMatchScore; }

                        // Add scores for any comments
                        foreach(JiraIssueComment comment in issue.Comments)
                        {
                            // Exact comment scores
                            if (comment.Author.ToLower() == term_lower) { score += exactMatchScore; }

                            // Fuzzy comment scores
                            if (comment.Content.ToLower().Contains(term_lower)) { score += fuzzyMatchScore; }
                        }
                    }
                }

                results.Add(new SearchResult() {
                    Issue = issue,
                    Score = score
                });
            }

            return results.OrderByDescending(x => x.Score).ThenByDescending(x => x.Issue.Created).ToList();
        } else {
            return new List<SearchResult>();
        }
    }
}
