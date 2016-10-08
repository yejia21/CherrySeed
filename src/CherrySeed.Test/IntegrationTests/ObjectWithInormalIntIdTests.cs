﻿using System;
using System.Collections.Generic;
using CherrySeed.Configuration;
using CherrySeed.EntityDataProvider;
using CherrySeed.Repositories;
using CherrySeed.Test.Asserts;
using CherrySeed.Test.Mocks;
using CherrySeed.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CherrySeed.Test.IntegrationTests
{
    [TestClass]
    public class ObjectWithInormalIntIdTests
    {
        [TestMethod]
        public void ObjectWithInormalIntId()
        {
            var entityData = new List<EntityData>
            {
                new EntityData
                {
                    EntityName = "CherrySeed.Test.Models.Project",
                    Objects = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            { "MyProjectId", "P1" },
                            { "CustomerId", "C1" }
                        },
                        new Dictionary<string, string>
                        {
                            { "MyProjectId", "P2" },
                            { "CustomerId", "C2" }
                        }
                    }
                },
                new EntityData
                {
                    EntityName = "CherrySeed.Test.Models.Customer",
                    Objects = new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            { "MyCustomerId", "C1" }
                        },
                        new Dictionary<string, string>
                        {
                            { "MyCustomerId", "C2" }
                        }
                    }
                },
            };

            var assertRepository = new AssertRepository((obj, count, entities) =>
            {
                AssertHelper.AssertIf(typeof(Project), 0, count, obj, () =>
                {
                    AssertProject.AssertProperties(obj, 1);
                });

                AssertHelper.AssertIf(typeof(Project), 1, count, obj, () =>
                {
                    AssertProject.AssertProperties(obj, 2);
                });

            }, type =>
            {
                
            });
            
            InitAndExecute(entityData, assertRepository, cfg =>
            {
                cfg.ForEntity<Customer>()
                    .WithPrimaryKey(e => e.MyCustomerId)
                    .WithPrimaryKeyIdGenerationInApplicationAsInteger();

                cfg.ForEntity<Project>()
                    .WithPrimaryKey(e => e.MyProjectId)
                    .WithReference(e => e.CustomerId, typeof (Customer))
                    .WithPrimaryKeyIdGenerationInApplicationAsInteger();
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