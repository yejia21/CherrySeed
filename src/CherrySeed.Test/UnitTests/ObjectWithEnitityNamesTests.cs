﻿using System;
using System.Collections.Generic;
using CherrySeed.Configuration;
using CherrySeed.EntityDataProvider;
using CherrySeed.Repositories;
using CherrySeed.Test.Asserts;
using CherrySeed.Test.Mocks;
using CherrySeed.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CherrySeed.Test.UnitTests
{
    [TestClass]
    public class ObjectWithEntityNamesTests
    {
        [TestMethod]
        public void ObjectWithEntityNames()
        {
            var entityData = new List<EntityData>
            {
                new EntityData
                {
                    EntityName = "Main",
                    Objects = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            { "Id", "1" },
                            { "MyString", "MyString 1" },
                            { "MyBool", "true" },
                            { "MyDateTime", "2016/05/03" },
                            { "MyDouble", "123,456" },
                            { "MyDecimal", "123,12" },
                            { "MyEnum1", "EnumValue1" },
                            { "MyEnum2", "EnumValue2" },
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "2" },
                            { "MyString", "MyString 2" },
                            { "MyBool", "false" },
                            { "MyDateTime", "2016/04/05" },
                            { "MyDouble", "123,998" },
                            { "MyDecimal", "1,43" },
                            { "MyEnum1", "0" },
                            { "MyEnum2", "1" },
                        }
                    }
                },
                new EntityData
                {
                    EntityName = "MySub",
                    Objects = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            { "Id", "1" },
                            { "MyString", "MyString 1" },
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "2" },
                            { "MyString", "MyString 2" },
                        }
                    }
                },
            };

            var assertRepository = new AssertRepository((obj, count, entities) =>
            {
                AssertHelper.AssertIf(typeof(Main), 0, count, obj, () =>
                {
                    AssertMain.AssertProperties(obj, new DateTime(2016, 5, 3), "MyString 1", true, (decimal)123.12, 123.456, MyEnum.EnumValue1, MyEnum.EnumValue2);
                });

                AssertHelper.AssertIf(typeof(Main), 1, count, obj, () =>
                {
                    AssertMain.AssertProperties(obj, new DateTime(2016, 4, 5), "MyString 2", false, (decimal)1.43,
                            123.998, MyEnum.EnumValue1, MyEnum.EnumValue2);
                });

                AssertHelper.AssertIf(typeof(Sub), 0, count, obj, () =>
                {
                    AssertSub.AssertProperties(obj, "MyString 1");
                });

                AssertHelper.AssertIf(typeof(Sub), 1, count, obj, () =>
                {
                    AssertSub.AssertProperties(obj, "MyString 2");
                });
            }, type =>
            {
                
            });
            
            InitAndExecute(entityData, assertRepository, cfg =>
            {
                cfg.ForEntity<Sub>()
                    .HasEntityName("MySub")
                    .WithPrimaryKey(e => e.Id)
                    .WithPrimaryKeyIdGenerationInApplicationAsInteger();

                cfg.ForEntity<Main>();
            });
        }

        private void InitAndExecute(List<EntityData> data, IRepository repository, 
            Action<ISeederConfigurationBuilder> entitySettings)
        {
            var config = new CherrySeedConfiguration(cfg =>
            {
                cfg.WithDataProvider(new DictionaryDataProvider(data));
                cfg.WithRepository(repository);

                entitySettings(cfg);
            });

            var cherrySeeder = config.CreateSeeder();
            cherrySeeder.Seed();
        }
    }
}