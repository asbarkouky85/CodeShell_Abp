using AutoMapper;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Senders;
using CodeshellCore.Notifications.Users;

namespace Codeshell.Abp.Notifications
{
    public class CodeshellNotificationsMappingProfile : Profile
    {
        public CodeshellNotificationsMappingProfile()
        {
            CreateMap<NotifyAttachmentData, NotificationAttachment>();
            CreateMap<Notification, NotificationListDto>()
                .ForMember(e => e.Resource, e => e.MapFrom(e => e.EntityType));
            CreateMap<User, UserMessageDeliveryData>()
                .ForMember(e => e.UserId, e => e.MapFrom(d => d.Id))
                .ForMember(e => e.PhoneNumber, e => e.MapFrom(d => d.Mobile))
                .ForMember(e => e.Name, e => e.MapFrom(d => d.Name));
            CreateMap<UserDevice, UserDeviceData>();
        }
    }
}
