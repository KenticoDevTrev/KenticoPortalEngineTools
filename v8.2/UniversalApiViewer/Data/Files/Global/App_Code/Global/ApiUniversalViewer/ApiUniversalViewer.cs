using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS;
using CMS.DataEngine;
using System.Data;
using System.Collections;

/// <summary>
/// Summary description for ApiUniversalViewer
/// </summary>
public class ApiUniversalViewerObject
{
    public virtual bool ObjectHasChildren()
    {
        return false;
    }
    public bool Published;
    public string ClassName;
    public int NodeID;
    public Guid NodeGUID;
    public string DocumentID;
    public string DocumentCulture
    {
        get
        {
            return CMS.Localization.LocalizationContext.CurrentCulture.CultureShortName;
        }
    }
    public int NodeParentID;
    public int NodeLevel;
    public int NodeOrder;
    public string NodeIDPath;
    public bool NodeHasChildren
    {
        get
        {
            return ObjectHasChildren();
        }
    }
    public string SiteName
    {
        get
        {
            return CMS.SiteProvider.SiteContext.CurrentSiteName;
        }
    }
    public int NodeClassID
    {
        get
        {
            try
            {
                return (int)CMS.DataEngine.DataClassInfoProvider.GetClasses().Where(x => x.ClassName == ClassName).FirstOrDefault().ClassID;
            }
            catch (Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogException("ApiUniversalViewer.cs", "INVALIDCLASSNAME", ex, additionalMessage: "Could not find a Class ID for the class :" + ClassName + ", cannot bind without it.  Please ensure this class exists.");
                return 0;
            }
        }
    }
    public void InitializeBase(ref int NextAvailableId, int ParentID, string ParentIDPath, int ItemLevel, int ItemOrder, string DocID = "")
    {
        NodeParentID = ParentID;
        NodeID = NextAvailableId;
        if(string.IsNullOrEmpty(DocID)) {
            DocumentID = NodeID.ToString();
        }
        NodeIDPath = ParentIDPath + "/" + NodeID.ToString();
        NextAvailableId++;
        NodeLevel = ItemLevel;
        NodeOrder = ItemOrder;
        NodeGUID = Guid.NewGuid();
        Published = true;
    }

    public void AppendDataRow(ref DataTable dt)
    {
        var dr = dt.NewRow();
        List<ApiUniversalViewerObject> ItemsToAppendAfter = new List<ApiUniversalViewerObject>();
        foreach(var Field in this.GetType().GetFields()) {

            if (ApiUniversalViewerHandler.TypeIsValid(Field.FieldType))
            {
                if (dt.Columns.Contains(Field.Name))
                {
                    dr[Field.Name] = Field.GetValue(this);
                }
            }
            else if (Field.FieldType.FullName.Contains("System.Collections.Generic"))
            {
                // Item is a list, now check if the items are of an API Universal viewer type. If so need to loop through them
                if (ApiUniversalViewerHandler.HasBaseClassOf(ApiUniversalViewerHandler.GetEnuermatedType(Field.FieldType), typeof(ApiUniversalViewerObject)))
                {
                    var test = Field.GetValue(this);
                    ItemsToAppendAfter.AddRange((List<ApiUniversalViewerObject>)test);
                }
            }
        }
        foreach (var Property in this.GetType().GetProperties())
        {
            if (ApiUniversalViewerHandler.TypeIsValid(Property.PropertyType))
            {
                if (dt.Columns.Contains(Property.Name))
                {
                    dr[Property.Name] = Property.GetValue(this, null);
                }
            }
            else if (Property.PropertyType.FullName.Contains("System.Collections.Generic"))
            {
                // Item is a list, now check if the items are of an API Universal viewer type. If so need to loop through them
                if (ApiUniversalViewerHandler.HasBaseClassOf(ApiUniversalViewerHandler.GetEnuermatedType(Property.PropertyType), typeof(ApiUniversalViewerObject)))
                {
                    var test = Property.GetValue(this, null);
                    ItemsToAppendAfter.AddRange((List<ApiUniversalViewerObject>)test);
                }
            }
        }

        dt.Rows.Add(dr);
        // Append child items
        foreach (var ItemToAppendAfter in ItemsToAppendAfter)
        {
            ItemToAppendAfter.AppendDataRow(ref dt);
        }
    }
}

public static class ApiUniversalViewerHandler
{
    // Just in case one class references a class above it, you could theoretically end up in an infinate loop of defining classes.
    private static int MaxLevels = 25;

    public static bool TypeIsValid(Type PropType)
    {
        bool isValid = false;
        switch (Type.GetTypeCode(PropType))
        {
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.String:
                isValid = true;
                break;
            default:
                isValid = false;
                break;
        }
        if (PropType == typeof(Guid))
        {
            isValid = true;
        }
        return isValid;
    }
    public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
    {
        return typeof(T);
    }
    // for the List Types, the first generic argument is the actual type
    public static Type GetEnuermatedType(Type ListType)
    {
        Type[] typeArguments = ListType.GetGenericArguments();
        foreach (Type tParam in typeArguments)
        {
            return tParam;
        }
        return ListType;
    }
    public static void AddColumnPropertiesToDataTable(ref DataTable dt, Type ObjectType)
    {
        // Build a list of unique field names
        List<string> Properties = new List<string>();

        Properties.Add("NodeGUID");
        Properties.AddRange(ObjectProperties(ObjectType));

        foreach (string Property in Properties)
        {
            try
            {
                dt.Columns.Add(Property);
            }
            catch (Exception ex)
            {
                // ignore duplicate columns till can do distinct
            }
        }

    }

    private static List<string> ObjectProperties(Type ApiUniversalViewerObjectType)
    {
        return ObjectProperties(ApiUniversalViewerObjectType, 0);
    }
    /// <summary>
    /// Takes a given Object Type and gives a List of all the valid Object Properties, this will be the 'Columns' of the Data Set.
    /// </summary>
    /// <param name="ApiUniversalViewerObjectType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private static List<string> ObjectProperties(Type ApiUniversalViewerObjectType, int level)
    {
        List<string> Properties = new List<string>();
        foreach (var field in ApiUniversalViewerObjectType.GetFields())
        {
            if (TypeIsValid(field.FieldType))
            {
                Properties.Add(field.Name);
            }
            else if (field.FieldType.FullName.Contains("System.Collections.Generic"))
            {
                // Item is a list, now check if the items are of an API Universal viewer type. If so need to loop through them
                if (HasBaseClassOf(ApiUniversalViewerHandler.GetEnuermatedType(field.FieldType), typeof(ApiUniversalViewerObject)))
                {
                    Properties.AddRange(ObjectProperties(ApiUniversalViewerHandler.GetEnuermatedType(field.FieldType), level++));
                }
            }
        }
        foreach (var prop in ApiUniversalViewerObjectType.GetProperties())
        {
            if (TypeIsValid(prop.PropertyType))
            {
                Properties.Add(prop.Name);
            }
            else if (prop.PropertyType.FullName.Contains("System.Collections.Generic"))
            {
                // Item is a list, now check if the items are of an API Universal viewer type. If so need to loop through them
                if (HasBaseClassOf(ApiUniversalViewerHandler.GetEnuermatedType(prop.PropertyType), typeof(ApiUniversalViewerObject)))
                {
                    Properties.AddRange(ObjectProperties(ApiUniversalViewerHandler.GetEnuermatedType(prop.PropertyType), level++));
                }
            }
        }
        Properties.RemoveAll(x => x.Length == 0);
        return Properties.Distinct().ToList();
    }

    public static bool HasBaseClassOf(Type objectType, Type BaseType)
    {
        if (objectType == BaseType)
        {
            return true;
        }
        else if (objectType == typeof(object) || objectType == null)
        {
            return false;
        }
        else
        {
            return HasBaseClassOf(objectType.BaseType, BaseType);
        }
    }

    public static DataTable FilterDataTable(DataTable dt, string OrderBy, string WhereCondition, int SelectTopN, int SkipN, int SelectNLevel)
    {
        DataTable SortedDt = null;

        // Handle Where, Order by conditions
        if (!string.IsNullOrEmpty(OrderBy) || !string.IsNullOrEmpty(WhereCondition))
        {
            DataView dv = dt.DefaultView;
            dv.Sort = OrderBy;
            dv.RowFilter = WhereCondition;
            SortedDt = dv.ToTable();
        }
        else
        {
            SortedDt = dt;
        }


        if (SelectTopN <= 0 && SkipN <= 0)
        {
            return SortedDt;
        }

        // Handle Select, Skip and Select Level
        DataTable dtn = SortedDt.Clone();
        int i = 0;
        if (SelectTopN < 0)
        {
            SelectTopN = 0;
        }
        if (SelectTopN == 0)
        {
            SelectTopN = dt.Rows.Count;
        }
        if (SkipN < 0)
        {
            SkipN = 0;
        }
        foreach (DataRow row in dt.Rows)
        {
            if (SelectNLevel < 0)
            {
                if (i < (SelectTopN + SkipN) && i >= SkipN)
                {
                    dtn.ImportRow(row);
                }
                else if (i >= (SelectTopN + SkipN))
                {
                    break;
                }
                i++;
            }
            else
            {
                if (SelectNLevel != int.Parse((string)row["NodeLevel"]))
                {
                    dtn.ImportRow(row);
                }
                else
                {
                    if (i < (SelectTopN + SkipN) && i >= SkipN)
                    {
                        dtn.ImportRow(row);
                    }
                    else if (i >= (SelectTopN + SkipN))
                    {
                        break;
                    }
                    i++;
                }
            }
        }

        // Clean up new Data Table, removing any Orphaned children that were excluded by the NodeLevel Select Filter
        if (SelectNLevel >= 0)
        {
            DataView dv = dtn.DefaultView;
            string OrphanFilter = "NodeLevel <= " + SelectNLevel + " or (1=0";
            foreach (DataRow row in dtn.Rows)
            {
                if (int.Parse((string)row["NodeLevel"]) > SelectNLevel)
                {
                    OrphanFilter += " or NodeIDPath like '" + (string)row["NodeIDPath"] + "/%'";
                }
            }
            dv.RowFilter = OrphanFilter + ")";
            return dv.ToTable();
        }

        return dtn;
    }
}

