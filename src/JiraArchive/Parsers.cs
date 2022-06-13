using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiraArchive;

public static class Parsers
{
    /// <summary>
    /// A potentially easier to use wrapper for the parse int function
    /// </summary>
    /// <param name="thisString"></param>
    /// <returns></returns>
    public static int ToInt(this string thisString)
    {
        int returnMe = 0;

        if (Int32.TryParse(thisString, out returnMe))
        {
            return returnMe;
        }

        return 0;
    }

    public static DateTime ToDateTime(this string thisDate)
    {
        // Check if we were given the "null" date value from a SQL database, and return the C# minvalue instead
        if (thisDate.Trim() == "1900-01-01 00:00:00.000")
        {
            return DateTime.MinValue;
        }

        DateTime returnMe = DateTime.MinValue;

        if (!DateTime.TryParse(thisDate, out returnMe))
        {
            returnMe = DateTime.MinValue;
        }

        // Again, check if we've managed to parse the SQL's minimum date and convert it
        if (returnMe == new DateTime(1900, 1, 1))
        {
            returnMe = DateTime.MinValue;
        }

        return returnMe;
    }


    /// <summary>
    /// Parse a bool from a database or other text source
    /// </summary>
    /// <param name="thisDatabaseValue"></param>
    /// <returns></returns>
    public static bool ToBool(this string thisDatabaseValue)
    {
        if (String.IsNullOrEmpty(thisDatabaseValue))
        {
            return false;
        }
        else
        {
            bool parsedBool = false;
            Boolean.TryParse(thisDatabaseValue, out parsedBool);
            return parsedBool;
        }
    }
}
