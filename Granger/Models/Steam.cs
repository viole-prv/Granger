using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Granger
{
    public class Steam
    {
        public static readonly byte COMPETITIVE = 6;

        public static readonly Bitmap[] Profile = new[]
        {
            Properties.Resources.Recruit_Rank_1,
            Properties.Resources.Recruit_Rank_2,
            Properties.Resources.Recruit_Rank_3,
            Properties.Resources.Recruit_Rank_4,
            Properties.Resources.Corporal_Rank_5,
            Properties.Resources.Corporal_Rank_6,
            Properties.Resources.Corporal_Rank_7,
            Properties.Resources.Corporal_Rank_8,
            Properties.Resources.Sergeant_Rank_9,
            Properties.Resources.Sergeant_Rank_10,
            Properties.Resources.Sergeant_Rank_11,
            Properties.Resources.Sergeant_Rank_12,
            Properties.Resources.Master_Sergeant_Rank_13,
            Properties.Resources.Master_Sergeant_Rank_14,
            Properties.Resources.Master_Sergeant_Rank_15,
            Properties.Resources.Master_Sergeant_Rank_16,
            Properties.Resources.Sergeant_Major_Rank_17,
            Properties.Resources.Sergeant_Major_Rank_18,
            Properties.Resources.Sergeant_Major_Rank_19,
            Properties.Resources.Sergeant_Major_Rank_20,
            Properties.Resources.Lieutenant_Rank_21,
            Properties.Resources.Lieutenant_Rank_22,
            Properties.Resources.Lieutenant_Rank_23,
            Properties.Resources.Lieutenant_Rank_24,
            Properties.Resources.Captain_Rank_26,
            Properties.Resources.Captain_Rank_27,
            Properties.Resources.Captain_Rank_28,
            Properties.Resources.Major_Rank_30,
            Properties.Resources.Major_Rank_31,
            Properties.Resources.Major_Rank_32,
            Properties.Resources.Colonel_Rank_33,
            Properties.Resources.Colonel_Rank_34,
            Properties.Resources.Colonel_Rank_35,
            Properties.Resources.Brigadier_General_Rank_36,
            Properties.Resources.Major_General_Rank_37,
            Properties.Resources.Lieutenant_General_Rank_38,
            Properties.Resources.General_Rank_39,
            Properties.Resources.Global_General_Rank_40
        };

        public class IAsset
        {
            [JsonProperty("appid")]
            public uint AppID { get; set; }

            [JsonProperty("contextid")]
            public string? ContextID { get; set; }

            [JsonProperty("assetid")]
            public string? ID { get; set; }

            [JsonProperty("classid")]
            public string? ClassID { get; set; }

            [JsonProperty("instanceid")]
            public string? InstanceID { get; set; }

            [JsonProperty("amount")]
            public string? Amount { get; set; }

            [JsonIgnore]
            public decimal? Price { get; set; }
        }

        public class IDescription
        {
            [JsonProperty("classid")]
            public string? ClassID { get; set; }

            [JsonProperty("instanceid")]
            public string? InstanceID { get; set; }

            [JsonProperty("icon_url")]
            public string? Icon { get; set; }

            [JsonProperty("tradable")]
            public byte Tradable { get; set; }

            public class IOwner
            {
                [JsonProperty("value")]
                public string? Value { get; set; }
            }

            [JsonProperty("owner_descriptions")]
            public List<IOwner>? Owner { get; set; }

            [JsonProperty("market_name")]
            public string? MarketName { get; set; }

            [JsonProperty("market_hash_name")]
            public string? MarketHashName { get; set; }
        }

        public enum EResult : byte
        {
            Invalid = 0,
            OK = 1,
            Fail = 2,
            NoConnection = 3,
            InvalidPassword = 5,
            LoggedInElsewhere = 6,
            InvalidProtocolVer = 7,
            InvalidParam = 8,
            FileNotFound = 9,
            Busy = 10,
            InvalidState = 11,
            InvalidName = 12,
            InvalidEmail = 13,
            DuplicateName = 14,
            AccessDenied = 15,
            Timeout = 16,
            Banned = 17,
            AccountNotFound = 18,
            InvalidSteamID = 19,
            ServiceUnavailable = 20,
            NotLoggedOn = 21,
            Pending = 22,
            EncryptionFailure = 23,
            InsufficientPrivilege = 24,
            LimitExceeded = 25,
            Revoked = 26,
            Expired = 27,
            AlreadyRedeemed = 28,
            DuplicateRequest = 29,
            AlreadyOwned = 30,
            IPNotFound = 31,
            PersistFailed = 32,
            LockingFailed = 33,
            LogonSessionReplaced = 34,
            ConnectFailed = 35,
            HandshakeFailed = 36,
            IOFailure = 37,
            RemoteDisconnect = 38,
            ShoppingCartNotFound = 39,
            Blocked = 40,
            Ignored = 41,
            NoMatch = 42,
            AccountDisabled = 43,
            ServiceReadOnly = 44,
            AccountNotFeatured = 45,
            AdministratorOK = 46,
            ContentVersion = 47,
            TryAnotherCM = 48,
            PasswordRequiredToKickSession = 49,
            AlreadyLoggedInElsewhere = 50,
            Suspended = 51,
            Cancelled = 52,
            DataCorruption = 53,
            DiskFull = 54,
            RemoteCallFailed = 55,
            PasswordUnset = 56,
            ExternalAccountUnlinked = 57,
            PSNTicketInvalid = 58,
            ExternalAccountAlreadyLinked = 59,
            RemoteFileConflict = 60,
            IllegalPassword = 61,
            SameAsPreviousValue = 62,
            AccountLogonDenied = 63,
            CannotUseOldPassword = 64,
            InvalidLoginAuthCode = 65,
            AccountLogonDeniedNoMail = 66,
            HardwareNotCapableOfIPT = 67,
            IPTInitError = 68,
            ParentalControlRestricted = 69,
            FacebookQueryError = 70,
            ExpiredLoginAuthCode = 71,
            IPLoginRestrictionFailed = 72,
            AccountLockedDown = 73,
            AccountLogonDeniedVerifiedEmailRequired = 74,
            NoMatchingURL = 75,
            BadResponse = 76,
            RequirePasswordReEntry = 77,
            ValueOutOfRange = 78,
            UnexpectedError = 79,
            Disabled = 80,
            InvalidCEGSubmission = 81,
            RestrictedDevice = 82,
            RegionLocked = 83,
            RateLimitExceeded = 84,
            AccountLoginDeniedNeedTwoFactor = 85,
            ItemDeleted = 86,
            AccountLoginDeniedThrottle = 87,
            TwoFactorCodeMismatch = 88,
            TwoFactorActivationCodeMismatch = 89,
            AccountAssociatedToMultiplePartners = 90,
            NotModified = 91,
            NoMobileDevice = 92,
            TimeNotSynced = 93,
            SMSCodeFailed = 94,
            AccountLimitExceeded = 95,
            AccountActivityLimitExceeded = 96,
            PhoneActivityLimitExceeded = 97,
            RefundToWallet = 98,
            EmailSendFailure = 99,
            NotSettled = 100,
            NeedCaptcha = 101,
            GSLTDenied = 102,
            GSOwnerDenied = 103,
            InvalidItemType = 104,
            IPBanned = 105,
            GSLTExpired = 106,
            InsufficientFunds = 107,
            TooManyPending = 108,
            NoSiteLicensesFound = 109,
            WGNetworkSendExceeded = 110,
            AccountNotFriends = 111,
            LimitedUserAccount = 112,
            CantRemoveItem = 113,
            AccountHasBeenDeleted = 114,
            AccountHasAnExistingUserCancelledLicense = 115,
            DeniedDueToCommunityCooldown = 116
        }

        public enum ECurrency : byte
        {
            USD = 1,
            RUB = 5,
            TRY = 17
        }
    }
}
