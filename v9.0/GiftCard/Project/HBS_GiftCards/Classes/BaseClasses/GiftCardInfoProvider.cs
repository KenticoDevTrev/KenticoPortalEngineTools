using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace HBS_GiftCards
{
    /// <summary>
    /// Class providing GiftCardInfo management.
    /// </summary>
    public partial class GiftCardInfoProvider : AbstractInfoProvider<GiftCardInfo, GiftCardInfoProvider>
    {
        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public GiftCardInfoProvider()
            : base(GiftCardInfo.TYPEINFO)
        {
        }

        #endregion


        #region "Public methods - Basic"

        /// <summary>
        /// Returns a query for all the GiftCardInfo objects.
        /// </summary>
        public static ObjectQuery<GiftCardInfo> GetGiftCards()
        {
            return ProviderObject.GetGiftCardsInternal();
        }


        /// <summary>
        /// Returns GiftCardInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardInfo ID</param>
        public static GiftCardInfo GetGiftCardInfo(int id)
        {
            return ProviderObject.GetGiftCardInfoInternal(id);
        }


        /// <summary>
        /// Returns GiftCardInfo with specified name.
        /// </summary>
        /// <param name="name">GiftCardInfo name</param>
        public static GiftCardInfo GetGiftCardInfo(string name)
        {
            return ProviderObject.GetGiftCardInfoInternal(name);
        }


        /// <summary>
        /// Returns GiftCardInfo with specified name.
        /// </summary>
        /// <param name="name">GiftCardInfo name</param>
        /// <param name="siteName">Site name</param>
        public static GiftCardInfo GetGiftCardInfo(string name, string siteName)
        {
            return ProviderObject.GetGiftCardInfoInternal(name, siteName);
        }


        /// <summary>
        /// Returns GiftCardInfo with specified GUID.
        /// </summary>
        /// <param name="guid">GiftCardInfo GUID</param>                
        public static GiftCardInfo GetGiftCardInfo(Guid guid)
        {
            return ProviderObject.GetGiftCardInfoInternal(guid);
        }


        /// <summary>
        /// Sets (updates or inserts) specified GiftCardInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardInfo to be set</param>
        public static void SetGiftCardInfo(GiftCardInfo infoObj)
        {
            ProviderObject.SetGiftCardInfoInternal(infoObj);
        }


        /// <summary>
        /// Deletes specified GiftCardInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardInfo to be deleted</param>
        public static void DeleteGiftCardInfo(GiftCardInfo infoObj)
        {
            ProviderObject.DeleteGiftCardInfoInternal(infoObj);
        }


        /// <summary>
        /// Deletes GiftCardInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardInfo ID</param>
        public static void DeleteGiftCardInfo(int id)
        {
            GiftCardInfo infoObj = GetGiftCardInfo(id);
            DeleteGiftCardInfo(infoObj);
        }

        #endregion


        #region "Public methods - Advanced"


        /// <summary>
        /// Returns a query for all the GiftCardInfo objects of a specified site.
        /// </summary>
        /// <param name="siteId">Site ID</param>
        public static ObjectQuery<GiftCardInfo> GetGiftCards(int siteId)
        {
            return ProviderObject.GetGiftCardsInternal(siteId);
        }

        #endregion


        #region "Internal methods - Basic"

        /// <summary>
        /// Returns a query for all the GiftCardInfo objects.
        /// </summary>
        protected virtual ObjectQuery<GiftCardInfo> GetGiftCardsInternal()
        {
            return GetObjectQuery();
        }


        /// <summary>
        /// Returns GiftCardInfo with specified ID.
        /// </summary>
        /// <param name="id">GiftCardInfo ID</param>        
        protected virtual GiftCardInfo GetGiftCardInfoInternal(int id)
        {
            return GetInfoById(id);
        }


        /// <summary>
        /// Returns GiftCardInfo with specified name.
        /// </summary>
        /// <param name="name">GiftCardInfo name</param>        
        protected virtual GiftCardInfo GetGiftCardInfoInternal(string name)
        {
            return GetInfoByCodeName(name);
        }


        /// <summary>
        /// Returns GiftCardInfo with specified name.
        /// </summary>
        /// <param name="name">GiftCardInfo name</param>                
        /// <param name="siteName">Site name</param>         
        protected virtual GiftCardInfo GetGiftCardInfoInternal(string name, string siteName)
        {
            return GetInfoByCodeName(name, SiteInfoProvider.GetSiteID(siteName));
        }


        /// <summary>
        /// Returns GiftCardInfo with specified GUID.
        /// </summary>
        /// <param name="guid">GiftCardInfo GUID</param>
        protected virtual GiftCardInfo GetGiftCardInfoInternal(Guid guid)
        {
            return GetInfoByGuid(guid);
        }


        /// <summary>
        /// Sets (updates or inserts) specified GiftCardInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardInfo to be set</param>        
        protected virtual void SetGiftCardInfoInternal(GiftCardInfo infoObj)
        {
            SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified GiftCardInfo.
        /// </summary>
        /// <param name="infoObj">GiftCardInfo to be deleted</param>        
        protected virtual void DeleteGiftCardInfoInternal(GiftCardInfo infoObj)
        {
            DeleteInfo(infoObj);
        }

        #endregion

        #region "Internal methods - Advanced"


        /// <summary>
        /// Returns a query for all the GiftCardInfo objects of a specified site.
        /// </summary>
        /// <param name="siteId">Site ID</param>
        protected virtual ObjectQuery<GiftCardInfo> GetGiftCardsInternal(int siteId)
        {
            return GetObjectQuery().OnSite(siteId);
        }

        #endregion
    }
}