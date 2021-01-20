using System.Text.RegularExpressions;
using System;
using System.Net;
using CMS.DataEngine;
using System.Collections.Generic;
namespace CMS.Controls
{
    /// <summary>
    /// Extends the CMSTransformation partial class.
    /// </summary>
    public partial class CMSTransformation
    {
	#region "Get WebPart Functions"
public static WebPartInstance GetWebpartInstanceByWebpartID(string ControlID, Guid DocumentGUID) {
            PageInfo nodePageInfo = PageInfoProvider.GetPageInfo(DocumentGUID);

            foreach (WebPartInstance WebPart in nodePageInfo.TemplateInstance.WebPartZones[0].WebParts)
            {
                if (WebPart.ControlID == ControlID)
                {
                    return WebPart;
                }
            }

            return null;
        }

        public static CMSAbstractWebPart GetWebpartControllerByWebpartID(string WebpartID, Page UIPage)
        {
            List<Control> allControls = new List<Control>();
            GetControlList<Control>(UIPage.Controls, allControls);
            Control specifiedWebpart = null;
            foreach (var childControl in allControls)
            {
                if (childControl.ID == WebpartID) specifiedWebpart = childControl;
            }
            if (specifiedWebpart == null) throw new Exception("Webpart ID Not found");

            // Convert to CMSAbstractWebPart and call the ReloadData to ensure it's ready to be rendered
            return (CMSAbstractWebPart)specifiedWebpart;
        }

        public static Control GetChildControlByID(string ChildControlID, Control parentControl)
        {
            List<Control> allControls = new List<Control>();
            GetControlList<Control>(parentControl.Controls, allControls);
            Control specifiedControl = null;
            foreach (var childControl in allControls)
            {
                if (childControl.ID == ChildControlID) specifiedControl = childControl;
            }
            if (specifiedControl == null) throw new Exception("Webpart ID Not found");

            // Convert to CMSAbstractWebPart and call the ReloadData to ensure it's ready to be rendered
            return specifiedControl;
        }

        /// <summary>
        /// Helper function, gets all controls on page.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlCollection"></param>
        /// <param name="resultCollection"></param>
        private static void GetControlList<T>(ControlCollection controlCollection, List<T> resultCollection) where T : Control
        {
            foreach (Control control in controlCollection)
            {
                //if (control.GetType() == typeof(T))
                if (control is T) // This is cleaner
                    resultCollection.Add((T)control);

                if (control.HasControls())
                    GetControlList(control.Controls, resultCollection);
            }
        }
		#endregion
		
	#region "ICal DateTime and other DateTime"
		/// <summary>
    /// Handles logic of the date system to get the UTC time for iCal export.
    /// </summary>
    /// <param name="EventDate">The Starting Event Date</param>
    /// <param name="EventEndDate">(optional) the Ending Event Date</param>
    /// <param name="Pattern">Pattern to convert the DateTime to string</param>
    /// <param name="convertToUTC">Convert time to UTC</param>
    /// <param name="forStart">If for the starting date field, otherwise renders for the later</param>
    /// <returns>String the proper Date Time string formatted</returns>
    public static string getProperDateTimeForICS(object EventDate, object EventEndDate, string Pattern, bool convertToUTC, bool forStart)
    {

        DateTime eventDate = (DateTime)EventDate;
        DateTime eventEndDate = DateTime.Now;
        bool endDateGiven = true;
        try
        {
            eventEndDate = (DateTime)EventEndDate;
        }
        catch (Exception ex)
        {
            eventEndDate = eventDate;
            endDateGiven = false;
        }

        if (forStart)
        {
            if (!endDateGiven || eventDate.ToString("yyyyMMdd") != eventEndDate.ToString("yyyyMMdd")) {
                return ";VALUE=DATE:" + DateToUTC(eventDate, convertToUTC).ToString("yyyyMMdd");
            } else {
                return ":" + DateToUTC(eventDate, convertToUTC).ToString(Pattern);
            }
        }
        else
        {
            if (!endDateGiven) return ";VALUE=DATE:" + DateToUTC(eventDate, convertToUTC).ToString("yyyyMMdd");
            if(eventDate.ToString("yyyyMMdd") != eventEndDate.ToString("yyyyMMdd")) {
                return ";VALUE=DATE:" + DateToUTC(eventEndDate, convertToUTC).ToString("yyyyMMdd");
            } else {
                return ":" + DateToUTC(eventEndDate, convertToUTC).ToString(Pattern);
            }
        }
    }

    /// <summary>
    /// Converts the date to UTC
    /// </summary>
    /// <param name="time">The DateTime</param>
    /// <param name="convertToUTC">If to convert or not.</param>
    /// <returns></returns>
    public static DateTime DateToUTC(DateTime time, bool convertToUTC)
    {
        try
        {
            return (convertToUTC ? TimeZoneInfo.ConvertTimeToUtc(time, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")) : time);
        }
        catch (Exception ex)
        {
            CMS.EventLog.EventLogProvider.LogException("ECASD-Custom-Class.cs", "FUNCTION", ex);
            return time;
        }
    }
	
	/// <summary>
        /// Cleans up HTML content for text only rendering in an ICAL file
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public string CleanForICS(string Content)
        {
            if (string.IsNullOrEmpty(Content))
            {
                return "";
            }
            Content = Regex.Replace(Content, @"\r\n?|\n", "");
            Content = WebUtility.HtmlDecode(Content);
            Content = Regex.Replace(Content,  "<[^>]*(>|$)", "");
            Content = Content.Replace("&", " and ");
            Content = RemoveObjectTags(Content);
            return Content;
        }
		
		// Formats two dates properly, does not account for times.
		public string FormatDualDatesEnglish(object StartDate, object EndDate, string YearFormat = "yyyy", string MonthFormat = "MMMM", string DayFormat = "dd", bool useAbbr = false)
        {
            DateTime dtStartDate = DateTime.Now;
            DateTime dtEndDate = DateTime.Now;
            try
            {
                if(StartDate == null || StartDate == DBNull.Value) {
                    throw new Exception("Start date is null");
                }
                dtStartDate = (DateTime)StartDate;

                if (EndDate == null || EndDate == DBNull.Value)
                {
                    return DateRange.Generate(dtStartDate, dtStartDate, YearFormat, MonthFormat, DayFormat, useAbbr);
                } else {
                    dtEndDate = (DateTime)EndDate;
                    return DateRange.Generate(dtStartDate, dtEndDate, YearFormat, MonthFormat, DayFormat, useAbbr);
                }
            }
            catch (Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogException("CMSTransformation.cs", "FormatDualDates", ex, additionalMessage:"Invalid Start or End Date object passed.");
                return "Invalid Start or End Date object passed.";
            }
        }

		// Returns the time of two dates, handles null end dates and other similar logic.
        public string FormatDualTimes(object StartDate, object EndDate, string TimeFormat = "h:mm tt")
        {
            DateTime dtStartDate = DateTime.Now;
            DateTime dtEndDate = DateTime.Now;
            try
            {
                if (StartDate == null)
                {
                    throw new Exception("Start date is null");
                }
                dtStartDate = (DateTime)StartDate;

                if (EndDate == null)
                {
                    return dtStartDate.ToString(TimeFormat);
                }
                else
                {
                    dtEndDate = (DateTime)EndDate;
                    return string.Format("{0} - {1}", dtStartDate.ToString(TimeFormat), dtEndDate.ToString(TimeFormat));
                }
            }
            catch (Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogException("CMSTransformation.cs", "FormatDualDates", ex, additionalMessage: "Invalid Start or End Date object passed.");
                return "Invalid Start or End Date object passed.";
            }
        }
	#endregion
	
	#region "User Functions"
	
	/// <summary>
        /// Gets a user's Settings
        /// </summary>
        /// <param name="FieldName">The Settings Field Name</param>
        /// <param name="Defaultvalue">Default Value</param>
        /// <returns></returns>
        public static object GetUserSetting(string FieldName, object Defaultvalue = null)
        {
			return GetUserSetting(MembershipContext.AuthenticatedUser.UserID, FieldName, Defaultvalue);
        }
		/// <summary>
        /// Gets a user's Settings
        /// </summary>
        /// <param name="FieldName">The Settings Field Name</param>
        /// <param name="Defaultvalue">Default Value</param>
        /// <returns></returns>
        public static object GetUserSetting(int UserID, string FieldName, object Defaultvalue = null)
        {
            if (!string.IsNullOrWhiteSpace(FieldName))
            {
                // Cache results by user settings ID, if the user settings are ever updated then this will reset.
                object value = CacheHelper.Cache(cs => GetUserSettingHelper(cs, FieldName, UserID), new CacheSettings(1440, "GetUserSetting|" + UserID + "|" + FieldName));
                return (value == null ? Defaultvalue : value);
            }
            else
            {
                return null;
            }
        }
		 /// <summary>
        /// Helper function that uses cache
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        private static object GetUserSettingHelper(CacheSettings cs, string FieldName, int UserID)
        {
            if (cs.Cached)
            {
                // Sets a cache dependency for the data
                // The data is removed from the cache if the objects represented by the dummy key are modified (all user objects in this case)
                cs.CacheDependency = CacheHelper.GetCacheDependency("cms.usersettings|byid|" + UserID);
            }
            return UserSettingsInfoProvider.GetUserSettingsInfoByUser(UserID).GetValue(FieldName);
        }
	
	#endregion
	
	#region "MultiSite Helps"
	/// <summary>
        /// 
        /// </summary>
        /// <param name="AttachmentGuid"></param>
        /// <param name="NodeSiteID"></param>
        /// <param name="LinkedNodeSiteID"></param>
        /// <param name="maxSideSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="alt"></param>
        /// <returns></returns>
        public static string GetAttachmentUrlWithSitename(object AttachmentGuid, object NodeSiteID, object LinkedNodeSiteID, int maxSideSize = -1, int width = -1, int height = -1, string alt = "")
        {
            int siteID = 0;
            if (LinkedNodeSiteID != null)
            {
                siteID = ValidationHelper.GetInteger(LinkedNodeSiteID, 0);
            }
            else if (NodeSiteID != null)
            {
                siteID = ValidationHelper.GetInteger(NodeSiteID, 0);
            }
            else
            {
                siteID = SiteContext.CurrentSiteID;
            }
            string GuidStr = "";
            if (AttachmentGuid != null && AttachmentGuid.GetType() == Guid.NewGuid().GetType())
            {
                GuidStr = ((Guid)AttachmentGuid).ToString();
            }
            return string.Format("<img alt=\"{0}\" src=\"/getattachment/{1}/image.jpg.aspx?sitename={2}{3}{4}{5}\"/>",
                (string.IsNullOrWhiteSpace(alt) ? "" : alt),
                GuidStr,
                SiteInfoProvider.GetSiteName(siteID),
                (maxSideSize > 0 ? "&maxsidesize=" + maxSideSize : ""),
                (width > 0 ? "&width=" + width : ""),
                (height > 0 ? "&height=" + height : ""));
        }

	#endregion
	
	
	#region "Fractions"
	
	public static string ConvertUnit(decimal pvalue, string measurementType)
        {
            if (measurementType == "\"")
            {
                return Convert(pvalue);
            }
            else
            {
                return decimal.Round(pvalue, 2).ToString();
            }
        }

        public static string Convert(decimal pvalue,
        bool skip_rounding = false, decimal dplaces = (decimal)0.0625)
        {
            decimal value = pvalue;

            if (!skip_rounding)
                value = FractionHelper.DecimalRound(pvalue, dplaces);

            if (value == Math.Round(value, 0)) // whole number check
                return value.ToString("#");

            // get the whole value of the fraction
            decimal mWhole = Math.Truncate(value);

            // get the fractional value
            decimal mFraction = value - mWhole;

            // initialize a numerator and denomintar
            uint mNumerator = 0;
            uint mDenomenator = 1;

            // ensure that there is actual a fraction
            if (mFraction > 0m)
            {
                // convert the value to a string so that 
                // you can count the number of decimal places there are
                string strFraction = mFraction.ToString().Remove(0, 2);

                // store the number of decimal places
                uint intFractLength = (uint)strFraction.Length;

                // set the numerator to have the proper amount of zeros
                mNumerator = (uint)Math.Pow(10, intFractLength);

                // parse the fraction value to an integer that equals 
                // [fraction value] * 10^[number of decimal places]
                uint.TryParse(strFraction, out mDenomenator);

                // get the greatest common divisor for both numbers
                uint gcd = GreatestCommonDivisor(mDenomenator, mNumerator);

                // divide the numerator and the denominator by the greatest common divisor
                mNumerator = mNumerator / gcd;
                mDenomenator = mDenomenator / gcd;
            }

            // create a string builder
            StringBuilder mBuilder = new StringBuilder();

            // add the whole number if it's greater than 0
            if (mWhole > 0m)
            {
                mBuilder.Append(mWhole);
            }

            // add the fraction if it's greater than 0m
            if (mFraction > 0m)
            {
                if (mBuilder.Length > 0)
                {
                    //mBuilder.Append(" ");
                }
                mBuilder.Append("<span class=\"Fractional\"><span class=\"top\">");
                mBuilder.Append(mDenomenator);
                mBuilder.Append("</span><span class\"division\">/</span><span class=\"bottom\">");
                mBuilder.Append(mNumerator);
                mBuilder.Append("</span></span>");
            }

            return mBuilder.ToString();
        }

        // Converts fraction to decimal. 
        // There are two formats a fraction greater than 1 can consist of
        // which this function will work for:
        //  Example: 4-1/2 or 4 1/2
        // Fractions less than 1 are in the format of 1/2, etc..
        public static decimal Convert(string value)
        {
            string[] dparse;
            string[] fparse;
            string whole = "0";
            string dec = "";
            decimal dReturn = 0;

            // check for '-' or ' ' separator between whole number and fraction
            dparse = value.Contains('-') ? value.Split('-') : value.Split(' ');
            int pcount = dparse.Count();

            // fraction greater than one.
            if (pcount == 2)
            {
                whole = dparse[0];
                dec = dparse[1];
            }
            // whole number or fraction less than 1.
            else if (pcount == 1)
                dec = dparse[0];

            // split out fractional part of value passed in.
            fparse = dec.Split('/');

            // check for fraction.
            if (fparse.Count() == 2)
            {
                try
                {
                    decimal d0 = System.Convert.ToDecimal(fparse[0]); // convert numerator
                    decimal d1 = System.Convert.ToDecimal(fparse[1]); // convert denominator
                    dReturn = d0 / d1; // divide the fraction (converts to decimal)
                    decimal dWhole = System.Convert.ToDecimal(whole); // convert 
                    // whole number part to decimal.

                    dReturn = dWhole + dReturn; // add whole number 
                    // and fractional part and we're done.
                }
                catch (Exception e)
                {
                    dReturn = 0;
                }
            }
            else
            // there is no fractional part of the input.
            {
                try
                {
                    dReturn = System.Convert.ToDecimal(whole + dec);
                }
                catch (Exception e)
                {
                    // bad input so return 0.
                    dReturn = 0;
                }
            }

            return dReturn;
        }

        private static uint GreatestCommonDivisor(uint valA, uint valB)
        {
            // return 0 if both values are 0 (no GSD)
            if (valA == 0 &&
              valB == 0)
            {
                return 0;
            }
            // return value b if only a == 0
            else if (valA == 0 &&
                  valB != 0)
            {
                return valB;
            }
            // return value a if only b == 0
            else if (valA != 0 && valB == 0)
            {
                return valA;
            }
            // actually find the GSD
            else
            {
                uint first = valA;
                uint second = valB;

                while (first != second)
                {
                    if (first > second)
                    {
                        first = first - second;
                    }
                    else
                    {
                        second = second - first;
                    }
                }

                return first;
            }

        }

        // Converts a value to feet and inches.
        // Examples:
        //  12.1667 converts to 12' 2"
        //  4 converts to 4'
        //  0.1667 converts to 2"
        public static bool ReformatForFeetAndInches
            (ref string line_type, bool zero_for_blank = true)
        {
            if (string.IsNullOrEmpty(line_type))
            {
                if (zero_for_blank)
                    line_type = "0'";
                return true;
            }

            decimal d = System.Convert.ToDecimal(line_type);
            decimal d1 = Math.Floor(d);
            decimal d2 = d - d1;
            d2 = Math.Round(d2 * 12, 2);

            string s1;
            string s2;

            s1 = d1 == 0 ? "" : d1.ToString() + "'";
            s2 = d2 == 0 ? "" : FractionHelper.Convert(d2) + @"""";

            line_type = string.Format(@"{0} {1}", s1, s2).Trim();

            if (string.IsNullOrEmpty(line_type))
            {
                if (zero_for_blank)
                    line_type = "0'";
                return true;
            }

            return true;
        }

        // Rounds a number to the nearest decimal.
        // For instance, carpenters do not want to see a number like 4/5.  
        // That means nothing to them
        // and you'll have an angry carpenter on your hands 
        // if you ask them cut a 2x4 to 36 and 4/5 inches.
        // So, we would want to convert to the nearest 1/16 of an inch.
        // Example: DecimalRound(0.8, 0.0625) Rounds 4/5 to 13/16 or 0.8125.
        private static decimal DecimalRound(decimal val, decimal places)
        {
            string sPlaces = FractionHelper.Convert(places, true);
            string[] s = sPlaces.Split('/');

            if (s.Count() == 2)
            {
                int nPlaces = System.Convert.ToInt32(s[1]);
                decimal d = Math.Round(val * nPlaces);
                return d / nPlaces;
            }

            return val;
        }
	
	#endregion
	
	#region "Other Odd Functions"
	
	/// <summary>
        /// Detects if it is a division point or not.  Assuming Index is a 0 start index.
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Count"></param>
        /// <param name="splitBy"></param>
        /// <returns></returns>
        public static bool IsDivisionPoint(int Index, int Count, int splitBy)
        {
            // Increment Index by 1 since assuming 0 point index.
            Index = Index + 1;

            // based on the Split BY and the Count, gets the 'number' that the splits will occur on.  Ex count 8 and split is by 3, 8/3 = 2.6 => rounded to 3.
            // Every 3rd element will be a split.  The first element is not a split, so 1, 2, 3rd is a split, 4, 5, 6th is a split, 7 8 

            if ((Count <= splitBy && Index == splitBy) || Index == Count) return false;
            Double splitVal = Convert.ToDouble(Count) / Convert.ToDouble(splitBy);
            int SplitCounter = Convert.ToInt32(Math.Ceiling(splitVal));
            if (SplitCounter != 0)
            {
                return ((Index % SplitCounter) == 0);
            }
            else
            {
                return false;
            }
        }
	
	#endregion
	
	#region "String Functions"
	
	
	/// <summary>
        /// Trims text values to the specified length.
        /// </summary>
        /// <param name="txtValue">Original text to be trimmed</param>
        /// <param name="leftChars">Number of characters to be returned</param>
        public string RegexReplace(string Content, string RegexPattern, string Replace = "")
        {
            Regex rgx = new Regex(RegexPattern);
			return rgx.Replace(Content, Replace);
        }
        /// <summary>
        /// Trims text values to the specified length.
        /// </summary>
        /// <param name="txtValue">Original text to be trimmed</param>
        /// <param name="leftChars">Number of characters to be returned</param>
        public string RegexReplace(object Content, string RegexPattern, string Replace = "")
        {
            try { 
                if (Content != null) { 
                    Regex rgx = new Regex(RegexPattern);
                    return rgx.Replace((string)Content, Replace);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
		/// <summary>
        /// Any object tags in the HTML will render as {^ blahblah ^}, this will strip these out.  Note this will 'kill' any inline widget objects as well as other objects such as inline youtube videos
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public string RemoveObjectTags(string Content)
        {
            if (Content.IndexOf("{^") > -1)
            {
                string ContentPre = Content.Substring(0, Content.IndexOf("{^"));
                string ContentPost = Content.Substring(Content.IndexOf("^}") + 2);
                return RemoveObjectTags(ContentPre+ContentPost);
            }
            else
            {
                return Content;
            }
        }
		
		public string RemoveHTMLComments(string input)
        {
            string output = string.Empty;
            string[] temp = System.Text.RegularExpressions.Regex.Split(input, "<!--");
            foreach (string s in temp)
            {
                string str = string.Empty;
                if (!s.Contains("-->"))
                {
                    str = s;
                }
                else
                {
                    str = s.Substring(s.IndexOf("-->") + 3);
                }
                if (str.Trim() != string.Empty)
                {
                    output = output + str.Trim();
                }
            }
            return output;
        }
		
	#endregion
	
	static class DateRange
        { 
            public static string Generate(DateTime StartDate, DateTime EndDate, string YearFormat = "yyyy", string MonthFormat = "MM", string DayFormat = "dd", bool useAbbr = false)
            {
                int startYear = StartDate.Year;
                int startMonth = StartDate.Month;
                int startDay = StartDate.Day;
                int endYear = EndDate.Year;
                int endMonth = EndDate.Month;
                int endDay = EndDate.Day;

                bool yearsSame = startYear == endYear;
                bool monthsSame = startMonth == endMonth;
                bool wholeMonths = (startDay == 1 && IsLastDay(endDay, endMonth));

                string StartDay = StartDate.ToString(" "+DayFormat).Trim();
                string StartDayName = StartDate.ToString(" dddd").Trim();
                string StartMonth = StartDate.ToString(" " + MonthFormat).Trim();
                string StartYear = StartDate.ToString(" " + YearFormat).Trim();
                string EndDay = EndDate.ToString(" " + DayFormat).Trim();
                string EndMonth = EndDate.ToString(" " + MonthFormat).Trim();
                string EndYear = EndDate.ToString(" " + YearFormat).Trim();


                if (monthsSame && yearsSame && startDay == endDay)
                {
                    //return string.Format((useAbbr ? "{1}/{0}/{2}":"{0} {1}, {2}"), StartDay, StartMonth, StartYear);
                    //return string.Format((useAbbr ? "{0}/{1}/{2}" : "{0} {1}, {2}"), StartMonth, StartDay, StartYear;
                    return string.Format((useAbbr ? "{0}/{1}/{2}" : "{3} {0} {1}, {2}"), StartMonth, StartDay, StartYear, StartDayName);
                }

                if (monthsSame)
                {
                    if (yearsSame)
                    {
                        return wholeMonths
                                   ? string.Format((useAbbr ? "{0}/{1}":"{0} {1}"), StartMonth, EndYear)
                                   : string.Format((useAbbr ? "{0}/{1}/{3} - {0}/{2}/{3}":"{0} {1} - {2}, {3}"), StartMonth, StartDay, EndDay, StartYear);
                    }
                    return wholeMonths
                               ? string.Format((useAbbr ? "{0}/{1} - {2}/{3}":"{0}, {1} - {2}, {3}"),
                                               StartMonth, StartYear,
                                               EndMonth, EndYear)
                               : string.Format((useAbbr ? "{0}/{1}/{2} - {3}/{4}/{5}":"{0} {1}, {2} - {3} {4}, {5}"),
                                               StartMonth, StartDay, StartYear,
                                               EndMonth, EndDate, EndYear);
                }

                if (yearsSame)
                {
                    return wholeMonths
                               ? string.Format((useAbbr ? "{0}/{2} - {1}/{2}":"{0} - {1}, {2}"), StartMonth, EndMonth, EndYear)
                               : string.Format((useAbbr ? "{0}/{1}/{4} - {2}/{3}/{4}":"{0} {1} - {2} {3}, {4}"),
                                               StartMonth, StartDay,
                                               EndMonth, EndDay,
                                               EndYear);
                }
                return wholeMonths
                           ? string.Format((useAbbr ? "{0}/{1} - {2}/{3}":"{0}, {1} - {2}, {3}"),
                                           StartMonth, StartYear,
                                           EndMonth, EndYear)
                           : string.Format((useAbbr ? "{0}/{1}/{2} - {3}/{4}/{5}":"{0} {1}, {2} - {3} {4}, {5}"),
                                           StartMonth, StartDay, StartYear,
                                           EndMonth, EndDay, EndYear);
            }


            public static bool IsLastDay(int day, int month)
            {
                switch (month)
                {
                    case 2:
                        // Not leap-year aware
                        return (day == 28 || day == 29);
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        return (day == 31);
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        return (day == 30);
                    default:
                        return false;
                }
            }
        }
		
	}
}