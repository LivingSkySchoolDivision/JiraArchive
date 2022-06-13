using System.Text;

namespace JiraArchive;

public static class ExtensionMethods
{
    /// <summary>
    /// Converts a list of integers to a comma seperated string, for displaying somewhere to the user
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToCommaSeparatedString(this List<int> list)
    {
        StringBuilder returnMe = new StringBuilder();

        foreach (int item in list)
        {
            returnMe.Append(item);
            returnMe.Append(", ");
        }

        if (returnMe.Length > 2)
        {
            returnMe.Remove(returnMe.Length - 2, 2);
        }

        return returnMe.ToString();
    }
}