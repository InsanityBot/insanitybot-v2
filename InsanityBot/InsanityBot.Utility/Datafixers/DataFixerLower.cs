using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Datafixers
{
    public class DataFixerLower //code style: capital F intended
    {
        private readonly DatafixerRegistry Registry;

        public DataFixerLower()
        {
            Registry = new DatafixerRegistry();
        }

        // this just calls the Load method on all of them in case they need additional data
        // itll only succeed if all datafixers are valid, make sure to handle potential issues properly
        public DatafixerLoadingResult LoadAllDatafixers() 
        {
            foreach(var v in Registry.GetRegistryEntries())
            {
                if ((!v.Datafixer.IsClass && !v.Datafixer.IsValueType) || v.Datafixer.IsAbstract)
                    return DatafixerLoadingResult.CouldNotInstantiate; // invalid datafixer
                if ((!v.Datafixable.IsClass && !v.Datafixer.IsValueType) || v.Datafixable.IsAbstract)
                    return DatafixerLoadingResult.CouldNotInstantiate; // invalid datafixable

                // Load is static and should only use static data per the spec
                DatafixerLoadingResult result = (DatafixerLoadingResult)v.Datafixer.GetMethod("Load").Invoke(null, null);

                if (result != DatafixerLoadingResult.Success)
                    return result;
            }

            return DatafixerLoadingResult.Success;
        }

        public DatafixerLoadingResult LoadDatafixers(Type Datafixable)
        {
            foreach (var v in Registry.GetDatafixers(Datafixable))
            {
                if ((!v.Datafixer.IsClass && !v.Datafixer.IsValueType) || v.Datafixer.IsAbstract)
                    return DatafixerLoadingResult.CouldNotInstantiate; // invalid datafixer
                if ((!v.Datafixable.IsClass && !v.Datafixer.IsValueType) || v.Datafixable.IsAbstract)
                    return DatafixerLoadingResult.CouldNotInstantiate; // invalid datafixable

                // Load is static
                DatafixerLoadingResult result = (DatafixerLoadingResult)v.Datafixer.GetMethod("Load").Invoke(null, null);

                if (result != DatafixerLoadingResult.Success)
                    return result;
            }

            return DatafixerLoadingResult.Success;
        }

        public DatafixerUpgradeResult ApplyDatafixers(ref IDatafixable Data, Type DatafixableType)
        {
            if (DatafixableType == null)
                throw new ArgumentException($"{nameof(DatafixableType)} cannot be null or omitted", nameof(DatafixableType));

            List<IDatafixer> datafixers = (from v in Registry.GetDatafixers(DatafixableType)
                                           select (IDatafixer)Activator.CreateInstance(v.Datafixer))
                                           .OrderBy(datafixer => datafixer.GetType()
                                               .GetField("DatafixerId", BindingFlags.Public | BindingFlags.Static)
                                               .GetValue(null))
                                           .ToList();

            Data.DataVersion = (String)datafixers.Last().GetType()
                .GetField("NewDataVersion", BindingFlags.Public | BindingFlags.Static)
                .GetValue(null);

            Boolean hasFailed = false;

            foreach(IDatafixer datafixer in datafixers)
            {
                switch (datafixer.UpgradeData(ref Data))
                {
                    case DatafixerUpgradeResult.Success:
                        continue;
                    default:
                        if ((Boolean)datafixer.GetType()
                                .GetField("BreakingChange", BindingFlags.Public | BindingFlags.Static)
                                    .GetValue(null))
                            return DatafixerUpgradeResult.Failure;
                        else
                            hasFailed = true;
                        continue;
                }
            }

            return hasFailed switch
            {
                true => DatafixerUpgradeResult.PartialSuccess,
                false => DatafixerUpgradeResult.Success
            };
        }

        public DatafixerUpgradeResult ApplyDatafixers(ref IDatafixable Data, params IDatafixer[] Datafixers)
        {
            if (Datafixers == null)
                throw new ArgumentException($"{nameof(Datafixers)} cannot be null or omitted", nameof(Datafixers));

            Data.DataVersion = (String)Datafixers.Last().GetType()
                .GetField("NewDataVersion", BindingFlags.Public | BindingFlags.Static)
                .GetValue(null);

            Boolean hasFailed = false;

            foreach (IDatafixer datafixer in Datafixers)
            {
                switch (datafixer.UpgradeData(ref Data))
                {
                    case DatafixerUpgradeResult.Success:
                        continue;
                    default:
                        if ((Boolean)datafixer.GetType()
                                .GetField("BreakingChange", BindingFlags.Public | BindingFlags.Static)
                                    .GetValue(null))
                            return DatafixerUpgradeResult.Failure;
                        else
                            hasFailed = true;
                        continue;
                }
            }

            return hasFailed switch
            {
                true => DatafixerUpgradeResult.PartialSuccess,
                false => DatafixerUpgradeResult.Success
            };
        }
    }
}
