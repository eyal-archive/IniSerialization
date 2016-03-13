namespace IniSerialization.Ini
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Text;
	using System.Text.RegularExpressions;

	public sealed class IniKey : IEquatable<IniKey>
	{
		private const RegexOptions REGEX_OPTIONS = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

		private static readonly Regex _keyRegex = new Regex(@"^\s*(\w+)\s*=\s*(.*?)\s*$", REGEX_OPTIONS);

		private static readonly Regex _nameRegex = new Regex(@"^\w+$", REGEX_OPTIONS);

		private readonly string _keyString;

		public IniKey(string name, string value)
		{
			Contract.Requires(IsValidName(name));
			Contract.Requires(value != null);

			Name = name;

			Value = value;

			StringBuilder keyBuilder = new StringBuilder();

			_keyString = keyBuilder.Append(Name).Append("=").Append(Value).ToString();
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		[Pure]
		public static bool IsValidName(string name)
		{
			Contract.Requires(name != null);

			return _nameRegex.IsMatch(name);
		}

		public static IniKey Parse(string key)
		{
			Contract.Requires(key != null);

			Match match = _keyRegex.Match(key);

			if (match.Success)
			{
				string name = match.Groups[1].Value;

				Contract.Assume(IsValidName(name));

				string value = match.Groups[2].Value;

				return new IniKey(name, value);
			}

			return null;
		}

		public override bool Equals(object obj)
		{
			IniKey key = obj as IniKey;

			return Equals(key);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			return _keyString;
		}

		public bool Equals(IniKey other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(other, this))
			{
				return true;
			}

			if (other != null)
			{
				return other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);
			}

			return false;
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void __ObjectInvariant()
		{
			Contract.Invariant(Name != null);
			Contract.Invariant(IsValidName(Name));
			Contract.Invariant(Value != null);
			Contract.Invariant(_keyRegex != null);
			Contract.Invariant(_nameRegex != null);
		}
	}
}