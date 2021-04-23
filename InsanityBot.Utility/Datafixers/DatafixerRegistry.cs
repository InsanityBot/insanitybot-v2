using System;
using System.Collections.Generic;

using InsanityBot.Utility.Converters;
using InsanityBot.Utility.Datafixers.Reference;
using InsanityBot.Utility.Exceptions;

namespace InsanityBot.Utility.Datafixers
{
    internal class DatafixerRegistry
    {
        #region Variables, Constructor, nested Types, ...
        private delegate IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixersByTypeDelegate(Type type);
        private delegate IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixersByStringDelegate(String typename);
        private delegate void SortRawRegistryDelegate();
        private delegate void AddRegistryItemDelegate(DatafixerRegistryEntry item);
        private delegate void RemoveRegistryItemDelegate(DatafixerRegistryEntry item);

        private readonly List<DatafixerRegistryEntry> RawRegistry;
        private readonly Dictionary<Type, List<SortedDatafixerRegistryEntry>> SortedRegistry;
        private Boolean IsSorted;

        private readonly GetRequiredDatafixersByTypeDelegate GetRequiredDatafixersByTypeMethodHandler;
        private readonly GetRequiredDatafixersByStringDelegate GetRequiredDatafixersByStringMethodHandler;
        private readonly SortRawRegistryDelegate SortRawRegistryMethodHandler;
        private readonly AddRegistryItemDelegate AddRegistryItemMethodHandler;
        private readonly RemoveRegistryItemDelegate RemoveRegistryItemMethodHandler;

        public DatafixerRegistry(Byte RegistryMode)
        {
            RawRegistry = new List<DatafixerRegistryEntry>();
            SortedRegistry = new Dictionary<Type, List<SortedDatafixerRegistryEntry>>();
            IsSorted = false;

            switch (RegistryMode)
            {
                case 0:
                    GetRequiredDatafixersByTypeMethodHandler += GetRequiredDatafixers_Mode0;
                    GetRequiredDatafixersByStringMethodHandler += GetRequiredDatafixers_Mode0;
                    SortRawRegistryMethodHandler += SortRawRegistry_Mode0;
                    AddRegistryItemMethodHandler += AddRegistryItem_Mode0;
                    RemoveRegistryItemMethodHandler += RemoveRegistryItem_Mode0;
                    break;
                case 1:
                    GetRequiredDatafixersByTypeMethodHandler += GetRequiredDatafixers_Mode1;
                    GetRequiredDatafixersByStringMethodHandler += GetRequiredDatafixers_Mode1;
                    SortRawRegistryMethodHandler += SortRawRegistry_Mode1;
                    AddRegistryItemMethodHandler += AddRegistryItem_Mode1;
                    RemoveRegistryItemMethodHandler += RemoveRegistryItem_Mode1;
                    break;
            }
        }
        #endregion

        // DatafixerRegistry mode 0: sort on demand, keep sorted data   
        #region Registry Mode 0 
        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(Type type)
        {
            if (!IsSorted)
            {
                SortRawRegistryMethodHandler();
            }

            if (!SortedRegistry.ContainsKey(type))
            {
                return null;
            }

            return SortedRegistry[type].ToUnsorted(type);
        }

        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(String typename)
        {
            if (!IsSorted)
            {
                SortRawRegistryMethodHandler();
            }

            if (!SortedRegistry.ContainsKey(Type.GetType(typename)))
            {
                return null;
            }

            return SortedRegistry[Type.GetType(typename)].ToUnsorted(Type.GetType(typename));
        }

        private void SortRawRegistry_Mode0()
        {
            foreach (DatafixerRegistryEntry v in RawRegistry)
            {
                if (!SortedRegistry.ContainsKey(v.DatafixerTarget))
                {
                    SortedRegistry.Add(v.DatafixerTarget, new List<SortedDatafixerRegistryEntry>());
                }

                SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
            }
            RawRegistry.Clear();
            IsSorted = true;
        }

        private void AddRegistryItem_Mode0(DatafixerRegistryEntry item)
        {
            RawRegistry.Add(item);
            if (IsSorted)
            {
                IsSorted = false;
            }
        }

        private void RemoveRegistryItem_Mode0(DatafixerRegistryEntry item)
        {
            if (RawRegistry.Contains(item))
            {
                RawRegistry.Remove(item);
            }
            else if (SortedRegistry[item.DatafixerTarget].Contains(item.ToSortedDatafixerRegistryEntry()))
            {
                SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());
            }
            else
            {
                throw new DatafixerNotFoundException("Attempted to remove a datafixer from the registry that was not registered", item);
            }
        }
        #endregion

        // DatafixerRegistry mode 1: sort instantaneously
        #region Registry Mode 1
        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode1(Type type)
        {
            if (!IsSorted)
            {
                SortRawRegistryMethodHandler();
            }

            if (!SortedRegistry.ContainsKey(type))
            {
                return null;
            }

            return SortedRegistry[type].ToUnsorted(type);
        }

        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode1(String typename)
        {
            if (!IsSorted)
            {
                SortRawRegistryMethodHandler();
            }

            if (!SortedRegistry.ContainsKey(Type.GetType(typename)))
            {
                return null;
            }

            return SortedRegistry[Type.GetType(typename)].ToUnsorted(Type.GetType(typename));
        }

        private void SortRawRegistry_Mode1()
        {
            foreach (DatafixerRegistryEntry v in RawRegistry)
            {
                if (!SortedRegistry.ContainsKey(v.DatafixerTarget))
                {
                    SortedRegistry.Add(v.DatafixerTarget, new List<SortedDatafixerRegistryEntry>());
                }

                SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
            }
            IsSorted = true;
        }

        private void AddRegistryItem_Mode1(DatafixerRegistryEntry item)
        {
            RawRegistry.Add(item);
            SortedRegistry[item.DatafixerTarget].Add(item.ToSortedDatafixerRegistryEntry());
        }

        private void RemoveRegistryItem_Mode1(DatafixerRegistryEntry item)
        {
            RawRegistry.Remove(item);
            SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());
        }
        #endregion

        // interact with the private registry
        #region API
        public IEnumerable<DatafixerRegistryEntry> GetDatafixers(Type type) => GetRequiredDatafixersByTypeMethodHandler(type);

        public IEnumerable<DatafixerRegistryEntry> GetDatafixers(String typename) => GetRequiredDatafixersByStringMethodHandler(typename);

        public void SortDatafixers() => SortRawRegistryMethodHandler();

        public void AddDatafixer(DatafixerRegistryEntry datafixer) => AddRegistryItemMethodHandler(datafixer);

        public void RemoveDatafixer(DatafixerRegistryEntry datafixer) => RemoveRegistryItemMethodHandler(datafixer);

        internal Dictionary<Type, List<SortedDatafixerRegistryEntry>> GetAllDatafixers()
        {
            SortRawRegistryMethodHandler();
            return SortedRegistry;
        }
        #endregion
    }
}
