using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal delegate void RawRegistryDeletedEventHandler(DatafixerRegistry sender);
        internal delegate void RegistrySortedEventHandler(DatafixerRegistry sender);
        internal delegate void RegistryItemAddedEventHandler(DatafixerRegistry sender);
        internal delegate void RegistryItemRemovedEventHandler(DatafixerRegistry sender);


        private readonly List<DatafixerRegistryEntry> RawRegistry;
        private readonly Dictionary<Type, List<SortedDatafixerRegistryEntry>> SortedRegistry;
        private Boolean IsSorted;

        private GetRequiredDatafixersByTypeDelegate GetRequiredDatafixersByTypeMethodHandler;
        private GetRequiredDatafixersByStringDelegate GetRequiredDatafixersByStringMethodHandler;
        private SortRawRegistryDelegate SortRawRegistryMethodHandler;
        private AddRegistryItemDelegate AddRegistryItemMethodHandler;
        private RemoveRegistryItemDelegate RemoveRegistryItemMethodHandler;

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

        #region Events
        internal event RawRegistryDeletedEventHandler RawRegistryDeletedEvent;
        internal event RegistrySortedEventHandler RegistrySortedEvent;
        internal event RegistryItemAddedEventHandler RegistryItemAddedEvent;
        internal event RegistryItemRemovedEventHandler RegistryItemRemovedEvent;
        #endregion

        // DatafixerRegistry mode 0: sort on demand, keep sorted data   
        #region Registry Mode 0 
        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(Type type)
        {
            if (!IsSorted)
                SortRawRegistryMethodHandler();
            return (IEnumerable<DatafixerRegistryEntry>)SortedRegistry[type];
        }
        
        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(String typename)
        {
            if (!IsSorted)
                SortRawRegistryMethodHandler();
            return (IEnumerable<DatafixerRegistryEntry>)SortedRegistry[Type.GetType(typename)];
        }

        private void SortRawRegistry_Mode0()
        {
            foreach(var v in RawRegistry)
            {
                SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
                RawRegistry.Remove(v);
            }
            IsSorted = true;
            RegistrySortedEvent(this);
        }

        private void AddRegistryItem_Mode0(DatafixerRegistryEntry item)
        {
            RawRegistry.Add(item);
            if (IsSorted)
                IsSorted = false;
            RegistryItemAddedEvent(this);
        }

        private void RemoveRegistryItem_Mode0(DatafixerRegistryEntry item)
        {
            if (RawRegistry.Contains(item))
                RawRegistry.Remove(item);

            else if (SortedRegistry[item.DatafixerTarget].Contains(item.ToSortedDatafixerRegistryEntry()))
                SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());

            else
                throw new DatafixerNotFoundException("Attempted to remove a datafixer from the registry that was not registered", item);

            RegistryItemRemovedEvent(this);
        }
        #endregion

        // DatafixerRegistry mode 1: sort instantaneously
        #region Registry Mode 1
        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode1(Type type)
        {
            if (!IsSorted)
                SortRawRegistryMethodHandler();
            return (IEnumerable<DatafixerRegistryEntry>)SortedRegistry[type];
        }

        private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode1(String typename)
        {
            if (!IsSorted)
                SortRawRegistryMethodHandler();
            return (IEnumerable<DatafixerRegistryEntry>)SortedRegistry[Type.GetType(typename)];
        }

        private void SortRawRegistry_Mode1()
        {
            foreach(var v in RawRegistry)
                SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
            IsSorted = true;
            RegistrySortedEvent(this);
        }

        private void AddRegistryItem_Mode1(DatafixerRegistryEntry item)
        {
            RawRegistry.Add(item);
            SortedRegistry[item.DatafixerTarget].Add(item.ToSortedDatafixerRegistryEntry());
            RegistryItemAddedEvent(this);
        }

        private void RemoveRegistryItem_Mode1(DatafixerRegistryEntry item)
        {
            RawRegistry.Remove(item);
            SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());
            RegistryItemRemovedEvent(this);
        }
        #endregion
    }
}
