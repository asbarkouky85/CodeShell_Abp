﻿using Codeshell.Abp.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeshell.Abp.CliDispatch;
using Codeshell.Abp.CliDispatch.Parsing;

namespace Codeshell.Abp.CliDispatch.Parsing
{
    public static class CliDispatcher
    {
        public static CliDispatchConfiguration GetCliDispatchConfiguration(string[] args, bool required = false)
        {
            var config = new CliRequestBuilder<CliDispatchConfiguration>();
            config.Property(e => e.SettingsPath, "settings-folder", "sd", isRequired: required);
            config.Property(e => e.Environment, "environment", "env");

            var parser = new CliArgumentParser<CliDispatchConfiguration>();
            parser.Builder = config;
            return parser.Parse(args);
        }

        public static IConfigurationRoot UseSettingFolderFromArguments(string[] args)
        {
            var config = GetCliDispatchConfiguration(args, true);
            return Utils.LoadConfigurationFrom(config.SettingsPath, config.Environment);
        }
    }
}
