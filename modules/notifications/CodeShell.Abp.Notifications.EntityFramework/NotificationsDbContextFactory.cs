using Codeshell.Abp.EntityFramework.DesignTime;

namespace Codeshell.Abp.Notifications
{
    public class NotificationsDbContextFactory : CodeshellDesignTimeDbContextFactory<NotificationsContext>
    {
        protected override string ConnectionStringKey => "Notifications";

        public NotificationsDbContextFactory()
        {

        }


    }
}
