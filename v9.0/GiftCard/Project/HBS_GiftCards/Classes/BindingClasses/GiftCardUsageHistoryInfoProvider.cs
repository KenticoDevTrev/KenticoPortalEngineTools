using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace HBS_GiftCards
{
    /// <summary>
    /// Class providing GiftCardUsageHistoryInfo management.
    /// </summary>
    public partial class GiftCardUsageHistoryInfoProvider : AbstractInfoProvider<GiftCardUsageHistoryInfo, GiftCardUsageHistoryInfoProvider>
    {
        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public GiftCardUsageHistoryInfoProvider()
            : base(GiftCardUsageHistoryInfo.TYPEINFO)
        {
        }

        #endregion


        #region "Public methods - Basic"

        /// <summary>
        /// Returns a query for all the GiftCardUsageHistoryInfo objects.
        /// </summary>
        public static ObjectQuery<GiftCardUsageHistoryInfo> GetGiftCardUsageHistory()
        {
            return ProviderObject.GetGiftCardUsageHistoryInternal();
        }


        /// <summary>
        /// Returns GiftCardUsageHistoryInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardUsageHistoryInfo ID</param>
        public static GiftCardUsageHistoryInfo GetGiftCardUsageHistoryInfo(int id)
        {
            return ProviderObject.GetGiftCardUsageHistoryInfoInternal(id);
        }


        /// <summary>
        /// Returns GiftCardUsageHistoryInfo with specified GUID.
        /// </summary>
        /// <param name="guid">GiftCardUsageHistoryInfo GUID</param>                
        public static GiftCardUsageHistoryInfo GetGiftCardUsageHistoryInfo(Guid guid)
        {
            return ProviderObject.GetGiftCardUsageHistoryInfoInternal(guid);
        }


        /// <summary>
        /// Sets (updates or inserts) specified GiftCardUsageHistoryInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardUsageHistoryInfo to be set</param>
        public static void SetGiftCardUsageHistoryInfo(GiftCardUsageHistoryInfo infoObj)
        {
            ProviderObject.SetGiftCardUsageHistoryInfoInternal(infoObj);
        }


        /// <summary>
        /// Deletes specified GiftCardUsageHistoryInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardUsageHistoryInfo to be deleted</param>
        public static void DeleteGiftCardUsageHistoryInfo(GiftCardUsageHistoryInfo infoObj)
        {
            ProviderObject.DeleteGiftCardUsageHistoryInfoInternal(infoObj);
        }


        /// <summary>
        /// Deletes GiftCardUsageHistoryInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardUsageHistoryInfo ID</param>
        public static void DeleteGiftCardUsageHistoryInfo(int id)
        {
            GiftCardUsageHistoryInfo infoObj = GetGiftCardUsageHistoryInfo(id);
            DeleteGiftCardUsageHistoryInfo(infoObj);
        }

        #endregion


        #region "Internal methods - Basic"

        /// <summary>
        /// Returns a query for all the GiftCardUsageHistoryInfo objects.
        /// </summary>
        protected virtual ObjectQuery<GiftCardUsageHistoryInfo> GetGiftCardUsageHistoryInternal()
        {
            return GetObjectQuery();
        }


        /// <summary>
        /// Returns GiftCardUsageHistoryInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardUsageHistoryInfo ID</param>        
        protected virtual GiftCardUsageHistoryInfo GetGiftCardUsageHistoryInfoInternal(int id)
        {
            return GetInfoById(id);
        }


        /// <summary>
        /// Returns GiftCardUsageHistoryInfo with specified GUID.
        /// </summary>
        /// <param name="guid">GiftCardUsageHistoryInfo GUID</param>
        protected virtual GiftCardUsageHistoryInfo GetGiftCardUsageHistoryInfoInternal(Guid guid)
        {
            return GetInfoByGuid(guid);
        }


        /// <summary>
        /// Sets (updates or inserts) specified GiftCardUsageHistoryInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardUsageHistoryInfo to be set</param>        
        protected virtual void SetGiftCardUsageHistoryInfoInternal(GiftCardUsageHistoryInfo infoObj)
        {
            SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified GiftCardUsageHistoryInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardUsageHistoryInfo to be deleted</param>        
        protected virtual void DeleteGiftCardUsageHistoryInfoInternal(GiftCardUsageHistoryInfo infoObj)
        {
            DeleteInfo(infoObj);
        }

        #endregion
    }
}