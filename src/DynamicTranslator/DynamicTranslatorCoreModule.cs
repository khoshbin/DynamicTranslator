﻿using System;
using System.Reflection;

using Castle.Facilities.TypedFactory;

using DynamicTranslator.Configuration;
using DynamicTranslator.Configuration.Startup;
using DynamicTranslator.LanguageManagement;

namespace DynamicTranslator
{
    public class DynamicTranslatorCoreModule : DynamicTranslatorModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.IocContainer.AddFacility<TypedFactoryFacility>();

            IocManager.Resolve<DynamicTranslatorConfiguration>().Initialize();

            var existingToLanguage = AppConfigManager.Get("ToLanguage");
            var existingFromLanguage = AppConfigManager.Get("FromLanguage");

            Configurations.ApplicationConfiguration.IsLanguageDetectionEnabled = true;

            Configurations.ApplicationConfiguration.Client.CreateOrConsolidate(client =>
            {
                client.AppVersion = ApplicationVersion.GetCurrentVersion();
                client.Id = Guid.NewGuid().ToString();
                client.MachineName = Environment.MachineName.Normalize();
            });

            Configurations.ApplicationConfiguration.LeftOffset = 500;
            Configurations.ApplicationConfiguration.TopOffset = 15;
            Configurations.ApplicationConfiguration.SearchableCharacterLimit = 200;
            Configurations.ApplicationConfiguration.IsNoSqlDatabaseEnabled = true;
            Configurations.ApplicationConfiguration.MaxNotifications = 4;
            Configurations.ApplicationConfiguration.ToLanguage = new Language(existingToLanguage, LanguageMapping.All[existingToLanguage]);
            Configurations.ApplicationConfiguration.FromLanguage = new Language(existingFromLanguage, LanguageMapping.All[existingFromLanguage]);

            Configurations.GoogleAnalyticsConfiguration.Url = "http://www.google-analytics.com/collect";
            Configurations.GoogleAnalyticsConfiguration.TrackingId = "UA-70082243-2";
        }
    }
}