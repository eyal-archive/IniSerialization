namespace Ace.Ini
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;

	public sealed class IniSection : IEquatable<IniSection>
	{
		private const RegexOptions REGEX_OPTIONS = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

		private static readonly Regex _enclosedNameRegex = new Regex(@"^\[(\w+)\]$", REGEX_OPTIONS);

		private static readonly Regex _sectionRegex = new Regex(@"^\w+$", REGEX_OPTIONS);

		private IniKeyCollection _keys;

		public IniSection()
		{
			Name = string.Empty;

			EnclosedName = string.Empty;
		}

		public IniSection(string sectionName)
		{
			Contract.Requires(sectionName != null);
			Contract.Requires(IsValidSectionName(sectionName));

			Name = sectionName;

			StringBuilder formattedSectionName = new StringBuilder();

			EnclosedName = formattedSectionName.Append("[").Append(sectionName).Append("]").ToString();
		}

		public string EnclosedName { get; private set; }

		public IniKeyCollection Keys
		{
			get
			{
				Contract.Ensures(Contract.Result<IniKeyCollection>() != null);

				return _keys ?? (_keys = new IniKeyCollection());
			}
		}

		public string Name { get; private set; }

		[Pure]
		public static bool IsValidSectionName(string sectionName)
		{
			Contract.Requires(sectionName != null);

			return _sectionRegex.IsMatch(sectionName);
		}

		public static IniSection Parse(string sectionName)
		{
			IniSection section = null;

			if (sectionName == string.Empty)
			{
				section = new IniSection();
			}
			else if (!string.IsNullOrWhiteSpace(sectionName))
			{
				sectionName = sectionName.Trim();

				Match match = _enclosedNameRegex.Match(sectionName);

				if (match.Success)
				{
					section = new IniSection
					{
						Name = match.Groups[1].Value, 
						EnclosedName = sectionName
					};
				}
			}

			return section;
		}

		public override bool Equals(object obj)
		{
			IniSection section = obj as IniSection;

			return Equals(section);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(IniSection other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(other, this))
			{
				return true;
			}

			return other != null && other.ToString().Equals(ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public string[] ToStringArray()
		{
			List<string> section = new List<string>();

			if (Name != string.Empty)
			{
				section.Add(EnclosedName);
			}

			section.AddRange(Keys.Select(k => k.ToString()));

			return section.ToArray();
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void __ObjectInvariant()
		{
			Contract.Invariant(Name != null);
			Contract.Invariant(EnclosedName != null);
			Contract.Invariant(_sectionRegex != null);
		}
	}
}