using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Serialization
{
    /// <summary>
    /// ContractResolver для NewtonSoft-сериализации с точным сопоставлением имён полей
    /// В используемом по умолчанию ресолвере имена полей не чувствительны к регистру
    /// </summary>
    public class CaseSensitiveContractResolver: DefaultContractResolver
    {
        /// <inheritdoc />
        public override JsonContract ResolveContract(Type type)
        {
            var contract = base.ResolveContract(type);
            if (contract is JsonObjectContract objectContract)
            {
                var pi = typeof(JsonObjectContract).GetProperty("Properties");
                var backingField = pi?.DeclaringType?.GetField($"<{pi.Name}>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (backingField != null)
                {
                    backingField.SetValue(objectContract, new CaseSensitiveJsonPropertyCollection(type, objectContract.Properties));
                }
            }

            return contract;
        }
    }

    /// <summary>
    /// Реализация PropertyCollection, чувствительной к регистру имён полей
    /// </summary>
    public class CaseSensitiveJsonPropertyCollection: JsonPropertyCollection
    {
        /// <inheritdoc />
        public CaseSensitiveJsonPropertyCollection(Type type) : base(type) { }

        /// <summary>
        /// Конструктор, создающий коллекцию из экземпляра базового типа
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseCollection"></param>
        public CaseSensitiveJsonPropertyCollection(Type type, JsonPropertyCollection baseCollection): base(type)
        {
            var usedNames = new HashSet<string>(baseCollection.Count);
            foreach (var item in baseCollection)
            {
                Add(item);
                usedNames.Add(item.PropertyName);
            }

            void tryAddCaseDuplicate(string n)
            {
                if (!usedNames.Contains(n))
                {
                    Add(new JsonProperty
                    {
                        PropertyName = n,
                        Ignored = true
                    });
                }
            }

            foreach (var name in usedNames)
            {
                tryAddCaseDuplicate(name.ToLowerInvariant());
                tryAddCaseDuplicate(name.ToUpperInvariant());
            }

        }

        /// <summary/>
        public new JsonProperty GetClosestMatchProperty(string propertyName)
        {
            return GetProperty(propertyName, StringComparison.Ordinal);
        }
    }
}