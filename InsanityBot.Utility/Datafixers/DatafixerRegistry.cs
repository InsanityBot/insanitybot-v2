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
			this.RawRegistry = new List<DatafixerRegistryEntry>();
			this.SortedRegistry = new Dictionary<Type, List<SortedDatafixerRegistryEntry>>();
			this.IsSorted = false;

			switch (RegistryMode)
			{
				case 0:
					this.GetRequiredDatafixersByTypeMethodHandler += this.GetRequiredDatafixers_Mode0;
					this.GetRequiredDatafixersByStringMethodHandler += this.GetRequiredDatafixers_Mode0;
					this.SortRawRegistryMethodHandler += this.SortRawRegistry_Mode0;
					this.AddRegistryItemMethodHandler += this.AddRegistryItem_Mode0;
					this.RemoveRegistryItemMethodHandler += this.RemoveRegistryItem_Mode0;
					break;
				case 1:
					this.GetRequiredDatafixersByTypeMethodHandler += this.GetRequiredDatafixers_Mode1;
					this.GetRequiredDatafixersByStringMethodHandler += this.GetRequiredDatafixers_Mode1;
					this.SortRawRegistryMethodHandler += this.SortRawRegistry_Mode1;
					this.AddRegistryItemMethodHandler += this.AddRegistryItem_Mode1;
					this.RemoveRegistryItemMethodHandler += this.RemoveRegistryItem_Mode1;
					break;
			}
		}
		#endregion

		// DatafixerRegistry mode 0: sort on demand, keep sorted data   
		#region Registry Mode 0 
		private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(Type type)
		{
			if (!this.IsSorted)
			{
				this.SortRawRegistryMethodHandler();
			}

			if (!this.SortedRegistry.ContainsKey(type))
			{
				return null;
			}

			return this.SortedRegistry[type].ToUnsorted(type);
		}

		private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode0(String typename)
		{
			if (!this.IsSorted)
			{
				this.SortRawRegistryMethodHandler();
			}

			if (!this.SortedRegistry.ContainsKey(Type.GetType(typename)))
			{
				return null;
			}

			return this.SortedRegistry[Type.GetType(typename)].ToUnsorted(Type.GetType(typename));
		}

		private void SortRawRegistry_Mode0()
		{
			foreach (DatafixerRegistryEntry v in this.RawRegistry)
			{
				if (!this.SortedRegistry.ContainsKey(v.DatafixerTarget))
				{
					this.SortedRegistry.Add(v.DatafixerTarget, new List<SortedDatafixerRegistryEntry>());
				}

				this.SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
			}
			this.RawRegistry.Clear();
			this.IsSorted = true;
		}

		private void AddRegistryItem_Mode0(DatafixerRegistryEntry item)
		{
			this.RawRegistry.Add(item);
			if (this.IsSorted)
			{
				this.IsSorted = false;
			}
		}

		private void RemoveRegistryItem_Mode0(DatafixerRegistryEntry item)
		{
			if (this.RawRegistry.Contains(item))
			{
				this.RawRegistry.Remove(item);
			}
			else if (this.SortedRegistry[item.DatafixerTarget].Contains(item.ToSortedDatafixerRegistryEntry()))
			{
				this.SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());
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
			if (!this.IsSorted)
			{
				this.SortRawRegistryMethodHandler();
			}

			if (!this.SortedRegistry.ContainsKey(type))
			{
				return null;
			}

			return this.SortedRegistry[type].ToUnsorted(type);
		}

		private IEnumerable<DatafixerRegistryEntry> GetRequiredDatafixers_Mode1(String typename)
		{
			if (!this.IsSorted)
			{
				this.SortRawRegistryMethodHandler();
			}

			if (!this.SortedRegistry.ContainsKey(Type.GetType(typename)))
			{
				return null;
			}

			return this.SortedRegistry[Type.GetType(typename)].ToUnsorted(Type.GetType(typename));
		}

		private void SortRawRegistry_Mode1()
		{
			foreach (DatafixerRegistryEntry v in this.RawRegistry)
			{
				if (!this.SortedRegistry.ContainsKey(v.DatafixerTarget))
				{
					this.SortedRegistry.Add(v.DatafixerTarget, new List<SortedDatafixerRegistryEntry>());
				}

				this.SortedRegistry[v.DatafixerTarget].Add(v.ToSortedDatafixerRegistryEntry());
			}
			this.IsSorted = true;
		}

		private void AddRegistryItem_Mode1(DatafixerRegistryEntry item)
		{
			this.RawRegistry.Add(item);
			this.SortedRegistry[item.DatafixerTarget].Add(item.ToSortedDatafixerRegistryEntry());
		}

		private void RemoveRegistryItem_Mode1(DatafixerRegistryEntry item)
		{
			this.RawRegistry.Remove(item);
			this.SortedRegistry[item.DatafixerTarget].Remove(item.ToSortedDatafixerRegistryEntry());
		}
		#endregion

		// interact with the private registry
		#region API
		public IEnumerable<DatafixerRegistryEntry> GetDatafixers(Type type)
		{
			return this.GetRequiredDatafixersByTypeMethodHandler(type);
		}

		public IEnumerable<DatafixerRegistryEntry> GetDatafixers(String typename)
		{
			return this.GetRequiredDatafixersByStringMethodHandler(typename);
		}

		public void SortDatafixers()
		{
			this.SortRawRegistryMethodHandler();
		}

		public void AddDatafixer(DatafixerRegistryEntry datafixer)
		{
			this.AddRegistryItemMethodHandler(datafixer);
		}

		public void RemoveDatafixer(DatafixerRegistryEntry datafixer)
		{
			this.RemoveRegistryItemMethodHandler(datafixer);
		}

		internal Dictionary<Type, List<SortedDatafixerRegistryEntry>> GetAllDatafixers()
		{
			this.SortRawRegistryMethodHandler();
			return this.SortedRegistry;
		}
		#endregion
	}
}
