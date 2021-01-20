using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using HBS_GiftCards;

[assembly: RegisterObjectType(typeof(GiftCardUsageHistoryInfo), GiftCardUsageHistoryInfo.OBJECT_TYPE)]

namespace HBS_GiftCards
{
    /// <summary>
    /// GiftCardUsageHistoryInfo data container class.
    /// </summary>
	[Serializable]
    public partial class GiftCardUsageHistoryInfo : AbstractInfo<GiftCardUsageHistoryInfo>
    {
        #region "Type information"

        /// <summary>
        /// Object type
        /// </summary>
        public const string OBJECT_TYPE = "hbs_giftcards.giftcardusagehistory";


        /// <summary>
        /// Type information.
        /// </summary>
        public static ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(GiftCardUsageHistoryInfoProvider), OBJECT_TYPE, "HBS_GiftCards.GiftCardUsageHistory", "GiftCardUsageHistoryID", "GiftCardUsageHistoryLastModified", "GiftCardUsageHistoryGuid", null, null, null, null, "GiftCardID", GiftCardInfo.OBJECT_TYPE)
        {
            ModuleName = "HBS_GiftCards",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("GiftCardID", "hbs_giftcards.giftcard", ObjectDependencyEnum.Binding),
                new ObjectDependency("OrderID", "ecommerce.order", ObjectDependencyEnum.NotRequired),
            },
            LogEvents = true,
            CheckDependenciesOnDelete = true,
            ImportExportSettings =
            {
                IsExportable = true,
                ObjectTreeLocations = new List<ObjectTreeLocation>()
                {
                    // Adds the custom class into a new category in the Global objects section of the export tree
                    new ObjectTreeLocation(GLOBAL, "Ecommerce", "GiftCardHistory"),
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
                    new ObjectTreeLocation(GLOBAL, "Ecommerce", "GiftCardHistory")
                },
            },
        };

        #endregion


        #region "Properties"

        /// <summary>
        /// Gift card usage history ID
        /// </summary>
        [DatabaseField]
        public virtual int GiftCardUsageHistoryID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("GiftCardUsageHistoryID"), 0);
            }
            set
            {
                SetValue("GiftCardUsageHistoryID", value);
            }
        }


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
        /// Order ID the gift card charge was applied for.  Not required as you may charge the gift card some other way not associated with an order.
        /// </summary>
        [DatabaseField]
        public virtual int OrderID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("OrderID"), 0);
            }
            set
            {
                SetValue("OrderID", value, 0);
            }
        }


        /// <summary>
        /// The Amount Deducted or Added
        /// </summary>
        [DatabaseField]
        public virtual decimal Amount
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("Amount"), 0);
            }
            set
            {
                SetValue("Amount", value);
            }
        }


        /// <summary>
        /// If the amount listed is a deduction from the gift card.  Put False if you are charging onto the gift card.
        /// </summary>
        [DatabaseField]
        public virtual bool AmountIsDeduction
        {
            get
            {
                return ValidationHelper.GetBoolean(GetValue("AmountIsDeduction"), true);
            }
            set
            {
                SetValue("AmountIsDeduction", value);
            }
        }


        /// <summary>
        /// New balance
        /// </summary>
        [DatabaseField]
        public virtual decimal NewBalance
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("NewBalance"), 0);
            }
            set
            {
                SetValue("NewBalance", value, 0);
            }
        }


        /// <summary>
        /// Descriptive Note of what the history event is, such as "Used on Order" or "Inactivity Fee"
        /// </summary>
        [DatabaseField]
        public virtual string Note
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Note"), String.Empty);
            }
            set
            {
                SetValue("Note", value, String.Empty);
            }
        }


        /// <summary>
        /// Gift card usage history created
        /// </summary>
        [DatabaseField]
        public virtual DateTime GiftCardUsageHistoryCreated
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("GiftCardUsageHistoryCreated"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("GiftCardUsageHistoryCreated", value);
            }
        }


        /// <summary>
        /// Gift card usage history guid
        /// </summary>
        [DatabaseField]
        public virtual Guid GiftCardUsageHistoryGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("GiftCardUsageHistoryGuid"), Guid.Empty);
            }
            set
            {
                SetValue("GiftCardUsageHistoryGuid", value);
            }
        }


        /// <summary>
        /// Gift card usage history last modified
        /// </summary>
        [DatabaseField]
        public virtual DateTime GiftCardUsageHistoryLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("GiftCardUsageHistoryLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("GiftCardUsageHistoryLastModified", value);
            }
        }

        #endregion


        #region "Type based properties and methods"

        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            GiftCardUsageHistoryInfoProvider.DeleteGiftCardUsageHistoryInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            GiftCardUsageHistoryInfoProvider.SetGiftCardUsageHistoryInfo(this);
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected GiftCardUsageHistoryInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Constructor - Creates an empty GiftCardUsageHistoryInfo object.
        /// </summary>
        public GiftCardUsageHistoryInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Constructor - Creates a new GiftCardUsageHistoryInfo object from the given DataRow.
        /// </summary>
        /// <param name="dr">DataRow with the object data</param>
        public GiftCardUsageHistoryInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }

        #endregion
    }
}