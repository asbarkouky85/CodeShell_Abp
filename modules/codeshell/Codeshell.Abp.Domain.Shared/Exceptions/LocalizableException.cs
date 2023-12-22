using Microsoft.Extensions.Localization;
using System;
using System.Runtime.Serialization;
using Volo.Abp;
using Codeshell.Abp.Extensions;

namespace Codeshell.Abp.Exceptions
{
    public class LocalizableException<T> : LocalizableException
    {
        public LocalizableException(string messageKey, params string[] parameters) : base(typeof(T), messageKey, parameters)
        {
        }
    }

    [Serializable]
    public class LocalizableException : UserFriendlyException, IUserFriendlyException
    {
        public override string Message => _message;
        public object[] MessageParameters { get; private set; }
        public string MessageKey { get; private set; }
        public Type ResourceType { get; protected set; }
        private string _message;

        public LocalizableException(string messageKey, params string[] parameters) : base(messageKey)
        {
            MessageKey = messageKey;
            _message = messageKey;
            MessageParameters = parameters;
        }

        public LocalizableException(Type resourceType, string messageKey, params string[] parameters) : base(messageKey)
        {
            MessageKey = messageKey;
            MessageParameters = parameters;
            ResourceType = resourceType;
            _message = messageKey;
        }

        public void SetMessage(IStringLocalizer localizer)
        {
            _message = localizer.GetString(MessageKey, MessageParameters).ToString();
        }

        protected LocalizableException(SerializationInfo info, StreamingContext cont) : base(info, cont)
        {

        }

        public LocalizableException(string messageKey, Exception inner, params string[] parameters) : base(messageKey, innerException: inner)
        {
            MessageKey = messageKey;
            MessageParameters = parameters;
            _message = messageKey;
        }

        public string GetMessage(IStringLocalizer loc)
        {
            return loc.GetString(MessageKey, MessageParameters);
        }

        public UserFriendlyException ToUserFriendlyException(IStringLocalizer loc)
        {
            return new UserFriendlyException(GetMessage(loc), details: this.GetMessageRecursive());
        }

    }
}
