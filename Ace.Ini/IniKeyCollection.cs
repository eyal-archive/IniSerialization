namespace Ace.Ini
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;

	public sealed class IniKeyCollection : IEnumerable<IniKey>
	{
		private readonly IList<IniKey> _keys;

		public IniKeyCollection()
		{
			_keys = new List<IniKey>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(IniKey key)
		{
			Contract.Requires(key != null);

			if (!_keys.Contains(key))
			{
				_keys.Add(key);
			}
		}

		public IEnumerator<IniKey> GetEnumerator()
		{
			return _keys.GetEnumerator();
		}

		public IniKey GetKeyByName(string name)
		{
			Contract.Requires(name != null);

			return _keys.FirstOrDefault(k => k.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void Remove(IniKey key)
		{
			_keys.Remove(key);
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void __ObjectInvariant()
		{
			Contract.Invariant(_keys != null);
		}
	}
}