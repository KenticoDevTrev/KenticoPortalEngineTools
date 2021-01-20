using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using HBS_GiftCards;

[assembly: RegisterObjectType(typeof(GiftCardInfo), GiftCardInfo.OBJECT_TYPE)]

namespace HBS_GiftCards
{
    /// <summary>
    /// GiftCardInfo data container class.
    /// </summary>
	[Serializable]
    public partial class GiftCardInfo : AbstractInfo<GiftCardInfo>
    {
        #region "Type information"

        /// <summary>
        /// Object type
        /// </summary>
        public const string OBJECT_TYPE = "hbs_giftcards.giftcard";


        /// <summary>
        /// Type information.
        /// </summary>
        public static ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(GiftCardInfoProvider), OBJECT_TYPE, "HBS_GiftCards.GiftCard", "GiftCardID", "GiftCardLastModified", "GiftCardGuid", "GiftCardCode", "GiftCardDisplayName", null, "SiteID", null, null)
        {
            ModuleName = "HBS_GiftCards",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("SiteID", "cms.site", ObjectDependencyEnum.NotRequired),
                new ObjectDependency("CustomerID", "ecommerce.customer", ObjectDependencyEnum.NotRequired),
            },
            LogEvents = true,
            CheckDependenciesOnDelete = true,
            ImportExportSettings =
            {
                IsExportable = true,
                ObjectTreeLocations = new List<ObjectTreeLocation>()
                {
                    // Adds the custom class into a new category in the Global objects section of the export tree
                    new ObjectTreeLocation(GLOBAL, "Ecommerce", "GiftCard"),
                },
            },
            ContinuousIntegrationSettings =
            {
                Enabled = true
            },
            SynchronizationSettings =
            {
                LogSynchronization = SynchronizationTypeEnum.LogSynchronization,
                ObjectTreeLocations = new List<ObjectTreeLocation>()
                {
                    // Adds the custom class into a new category in the Global objects section of the staging tree
                    new ObjectTreeLocation(GLOBAL, "Ecommerce", "GiftCard")
                },
            },
        };

        #endregion


        #region "Properties"

        /// <summary>
        /// Gift card ID
        /// </summary>
        [DatabaseField]
        public virtual int GiftCardID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("GiftCardID"), 0);
            }
            set
            {
                SetValue("GiftCardID", value);
            }
        }


        /// <summary>
        /// Gift Card display name.  Does not have to be anything custom per gift card.
        /// </summary>
        [DatabaseField]
        public virtual string GiftCardDisplayName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GiftCardDisplayName"), "Gift Card");
            }
            set
            {
                SetValue("GiftCardDisplayName", value);
            }
        }


        /// <summary>
        /// This is the unique code for the Gift Card, customer must enter this in order to utilize it.
        /// </summary>
        [DatabaseField]
        public virtual string GiftCardCode
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GiftCardCode"), String.Empty);
            }
            set
            {
                SetValue("GiftCardCode", value);
            }
        }


        /// <summary>
        /// What site this card belongs to, leave blank to make this usable across all stores in your site.
        /// </summary>
        [DatabaseField]
        public virtual int SiteID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("SiteID"), 0);
            }
            set
            {
                SetValue("SiteID", value, 0);
            }
        }


        /// <summary>
        /// The Amount remaining on the gift card.  If "recharged" can add to this amount remaining.  Any purchases that use this will decrease the gift card amount.
        /// </summary>
        [DatabaseField]
        public virtual decimal AmountRemaining
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("AmountRemaining"), 0);
            }
            set
            {
                SetValue("AmountRemaining", value);
            }
        }


        /// <summary>
        /// If set, the gift card will expire at this time. 
        /// </summary>
        [DatabaseField]
        public virtual DateTime ExpirationDate
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("ExpirationDate"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("ExpirationDate", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Optional, can assign a gift card to a customer so that only that customer can use this card.  This can be used then as in store credit as well then.
        /// </summary>
        [DatabaseField]
        public virtual int CustomerID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("CustomerID"), 0);
            }
            set
            {
                SetValue("CustomerID", value, 0);
            }
        }


        /// <summary>
        /// If the Gift Card is enabled.
        /// </summary>
        [DatabaseField]
        public virtual bool Enabled
        {
            get
            {
                return ValidationHelper.GetBoolean(GetValue("Enabled"), true);
            }
            set
            {
                SetValue("Enabled", value);
            }
        }


        /// <summary>
        /// Gift card guid
        /// </summary>
        [DatabaseField]
        public virtual Guid GiftCardGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("GiftCardGuid"), Guid.Empty);
            }
            set
            {
                SetValue("GiftCardGuid", value);
            }
        }


        /// <summary>
        /// Gift card last modified
        /// </summary>
        [DatabaseField]
        public virtual DateTime GiftCardLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("GiftCardLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("GiftCardLastModified", value);
            }
        }

        #endregion


        #region "Type based properties and methods"

        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            GiftCardInfoProvider.DeleteGiftCardInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            GiftCardInfoProvider.SetGiftCardInfo(this);
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected GiftCardInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Constructor - Creates an empty GiftCardInfo object.
        /// </summary>
        public GiftCardInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Constructor - Creates a new GiftCardInfo object from the given DataRow.
        /// </summary>
        /// <param name="dr">DataRow with the object data</param>
        public GiftCardInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }

        #endregion
    }
}