using Newtonsoft.Json;
using System.Collections.Generic;

namespace Granger
{
    public class Steam
    {
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

            [JsonProperty("type")]
            public string? Type { get; set; }

            [JsonProperty("market_name")]
            public string? MarketName { get; set; }

            [JsonProperty("market_hash_name")]
            public string? MarketHashName { get; set; }      
            
            public class ITag
            {
                [JsonProperty("category")]
                public string? Category { get; set; }
            }

            [JsonProperty("tags")]
            public List<ITag>? Tag { get; set; }       
        }

        public class IRender
        {
            [JsonProperty("success", Required = Required.DisallowNull)]
            public bool Success { get; private set; }

            public class IResult
            {
                [JsonProperty("sell_price_text", Required = Required.DisallowNull)]
                public string? Price { get; set; }
                
                [JsonProperty("asset_description", Required = Required.DisallowNull)]
                public IDescription? Description { get; set; }
            }

            [JsonProperty("results", Required = Required.DisallowNull)]
            public List<IResult>? Result { get; set; }
        }

        public enum ECurrency : byte
        {
            USD = 1,
            GBP = 2,
            EUR = 3,
            CHF = 4,
            RUB = 5,
            PLN = 6,
            BRL = 7,
            JPY = 8,
            NOK = 9,
            IDR = 10,
            MYR = 11,
            PHP = 12,
            SGD = 13,
            THB = 14,
            VND = 15,
            KRW = 16,
            TRY = 17,
            UAH = 18,
            MXN = 19,
            CAD = 20,
            AUD = 21,
            NZD = 22,
            CNY = 23,
            INR = 24,
            CLP = 25,
            PEN = 26,
            COP = 27,
            ZAR = 28,
            HKD = 29,
            TWD = 30,
            SAR = 31,
            AED = 32,
            SEK = 33,
            ARS = 34,
            ILS = 35,
            BYN = 36,
            KZT = 37,
            KWD = 38,
            QAR = 39,
            CRC = 40,
            UYU = 41,
            BGN = 42,
            HRK = 43,
            CZK = 44,
            DKK = 45,
            HUF = 46,
            RON = 47
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
    }
}
