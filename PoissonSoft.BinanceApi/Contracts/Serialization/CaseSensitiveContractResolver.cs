using System;
using System.Collections.Generic;
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
                AdjustCollection(objectContract.Properties);

                //var pi = typeof(JsonObjectContract).GetProperty("Properties");
                //var backingField = pi?.DeclaringType?.GetField($"<{pi.Name}>k__BackingField",
                //    BindingFlags.Instance | BindingFlags.NonPublic);
                //if (backingField != null)
                //{
                //    backingField.SetValue(objectContract, new CaseSensitiveJsonPropertyCollection(type, objectContract.Properties));
                //}
            }

            return contract;
        }

        private static void AdjustCollection(JsonPropertyCollection collection)
        {
            var usedNames = new HashSet<string>(collection.Count);
            foreach (var item in collection)
            {
                usedNames.Add(item.PropertyName);
            }

            void tryAddCaseDuplicate(string n)
            {
                if (!usedNames.Contains(n) && collection.GetProperty(n, StringComparison.Ordinal) == null)
                {
                    collection.AddProperty(new JsonProperty
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
    }
}