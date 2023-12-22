//using Codeshell.Abp.Marking;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Domain.Repositories;
//using Volo.Abp.SettingManagement;

//namespace Codeshell.Abp.Attachments.Settings
//{
//    [TargetDatabase("Attachments", 1)]
//    public class SettingsDataSeedContributor : IDataSeedContributor, ITransientDependency
//    {

//        private readonly IRepository<Setting, Guid> _settingsRepository;

//        public SettingsDataSeedContributor(IRepository<Setting, Guid> settingsRepository)
//        {
//            _settingsRepository = settingsRepository;
//        }

//        public async Task SeedAsync(DataSeedContext context)
//        {
//            Dictionary<string, string> settingKeys = new Dictionary<string, string>();
//            //settingKeys.Add("Key", "Value");

//            foreach (var settingKey in settingKeys)
//            {
//                if (!await _settingsRepository.AnyAsync(s => s.Name == settingKey.Key))
//                {
//                    Setting entity = new Setting(Guid.NewGuid(), settingKey.Key, settingKey.Value, providerName: "G");
//                    await _settingsRepository.InsertAsync(entity);
//                }
//            }
//        }
//    }
//}
