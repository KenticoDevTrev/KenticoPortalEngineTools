using System;
using System.Collections.Generic;
using System.Web;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.Base;
using CMS.MacroEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS;


/// <summary>
/// Sample MacroMethodContainer class.
/// </summary>
// Makes all methods in the 'CustomMacroMethods' container class available for string objects
[assembly: RegisterExtension(typeof(CustomMacroMethods), typeof(string))]
// Registers methods from the 'CustomMacroMethods' container into the "Util" macro namespace
[assembly: RegisterExtension(typeof(CustomMacroMethods), typeof(UtilNamespace))]
public class CustomMacroMethods : MacroMethodContainer
{

    [MacroMethod(typeof(string), "Generates the SQL WHERE condition to filter Page Repeaters by Categories", 2)]
    [MacroMethodParam(0, "CategoryList", typeof(string), "A delimited list of the Category CodeNames that you want to filter by.")]
    [MacroMethodParam(1, "FilterType", typeof(string), "How you want the filter to behave.  ALL = Document must be in all the categories, ANY = Document must be in at least 1 of the given categories, NOTANY = Document must not be in any of the categories.")]
    [MacroMethodParam(2, "Delimeter", typeof(string), "(optional) The character that the list is delimited by.  Default is comma.")]
    [MacroMethodParam(3, "IgnoreEmpty", typeof(bool), "(optional) Whether or not to ignore the filter if there are no categories provided.  Default is true.")]
    public static object FilterByCategories(EvaluationContext context, params object[] parameters)
    {
        context.HandleSQLInjection = true;
        switch (parameters.Length)
        {
            case 0:
            case 1:
                throw new NotSupportedException();
            case 2:
                return FilterByCategories(ValidationHelper.GetString(parameters[0], ""), ValidationHelper.GetString(parameters[1], "ANY"));
            case 3:
                return FilterByCategories(ValidationHelper.GetString(parameters[0], ""), ValidationHelper.GetString(parameters[1], "ANY"), ValidationHelper.GetString(parameters[2], ","));
            case 4:
                return FilterByCategories(ValidationHelper.GetString(parameters[0], ""), ValidationHelper.GetString(parameters[1], "ANY"), ValidationHelper.GetString(parameters[2], ","), ValidationHelper.GetBoolean(parameters[3], true));
            default:
                // Overload with two parameters
                throw new NotSupportedException();
        }
    }

    public static string FilterByCategories(string CategoryList, string FilterType, string Delimeter = ",", bool IgnoreIfEmpty = true)
    {
        if (IgnoreIfEmpty && CategoryList.Length == 0)
        {
            return " 1=1 ";
        }
        int testInt = 0;
        List<string> Categories = new List<string>();
        var unique_items = new HashSet<string>(CategoryList.Split(Delimeter[0]));
        foreach (string s in unique_items) {
            Categories.Add(CMS.DataEngine.SqlHelper.GetSafeQueryString(s, false));
        }
        Categories.RemoveAll(x => x.Length == 0);
        bool allNumbers = true;
        string WhereStatement = "";
        switch (FilterType.ToUpper())
        {
            case "ALL":
                List<string> WhereStatements = new List<string>();
                foreach (string categoryName in Categories)
                {
                    WhereStatements.Add("DocumentID in (Select DocumentID from CMS_DocumentCategory WHERE CategoryID in (Select CategoryID from CMS_Category where CategoryName = '" + categoryName + "' "+ (int.TryParse(categoryName, out testInt) ? " or CategoryId = "+categoryName : "")+"))");
                }
                WhereStatement = string.Join(" and ", WhereStatements.ToArray());
                break;
            case "NOTANY":
                
                foreach (string categoryName in Categories)
                {
                    allNumbers = (allNumbers && int.TryParse(categoryName, out testInt));
                }
                WhereStatement = "DocumentID not in (Select DocumentID from CMS_DocumentCategory WHERE CategoryID in (Select CategoryID from CMS_Category where CategoryName in ('" + string.Join("','", Categories.ToArray()) + "') " + (allNumbers && Categories.Count > 0 ? " or CategoryId in (" + string.Join(",", Categories.ToArray()) + ")" : "") + "))";
                break;
            case "ANY":
            default:
                foreach (string categoryName in Categories)
                {
                    allNumbers = (allNumbers && int.TryParse(categoryName, out testInt));
                }
                WhereStatement = "DocumentID in (Select DocumentID from CMS_DocumentCategory WHERE CategoryID in (Select CategoryID from CMS_Category where CategoryName in('" + string.Join("','", Categories.ToArray()) + "') " + (allNumbers && Categories.Count > 0 ? " or CategoryId in (" + string.Join(",", Categories.ToArray()) + ")" : "") + "))";
                break;
        }
        return " "+WhereStatement+" ";
    }


}



