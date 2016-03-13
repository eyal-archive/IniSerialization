namespace IniSerialization.Ini
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;

	public sealed class IniSectionCollection : IEnumerable<IniSection>
	{
		private readonly IList<IniSection> _sections;

		private string _trackedSectionName;

		public IniSectionCollection()
		{
			_sections = new List<IniSection>();

			_trackedSectionName = null;
		}

		public static IniSectionCollection Parse(string data)
		{
			Contract.Requires(!string.IsNullOrWhiteSpace(data));

			int lineIndex = 0;

			int length = data.IndexOf("\r\n", StringComparison.InvariantCultureIgnoreCase);

			if (length == -1)
			{
				length = data.Length;
			}

			IniSectionCollection sections = new IniSectionCollection();

			while (length > -1)
			{
				int lineLength = length - lineIndex;

				Contract.Assume(lineLength > 0);
				Contract.Assume(lineIndex <= data.Length - lineLength);

				string line = data.Substring(lineIndex, lineLength);

				string cleanLine = line.Trim(' ', '\t', '\r', '\n', '\v', 'b', '\f');

				IniSection section = sections.GetCurrentSection();

				IniKey key = IniKey.Parse(cleanLine);

				if (key != null)
				{
					if (section == null)
					{
						sections.Add(new IniSection());

						section = sections.GetCurrentSection();
					}
					else
					{
						IniSection oldSection = sections.GetSectionByName(section.Name);

						if (oldSection != null && oldSection.Equals(section))
						{
							IniKeyCollection keys = oldSection.Keys;

							if (keys.Contains(key))
							{
								keys.Remove(key);
							}

							section = oldSection;
						}
					}

					Contract.Assume(section != null);

					section.Keys.Add(key);
				}
				else
				{
					section = IniSection.Parse(cleanLine);

					if (section != null)
					{
						sections.Add(section);
					}
				}

				lineIndex = length;

				if (lineIndex == data.Length)
				{
					break;
				}

				length = data.IndexOf("\r\n", length + 1, StringComparison.InvariantCultureIgnoreCase);

				if (length == -1)
				{
					length = data.Length;
				}
			}

			return sections;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(IniSection section)
		{
			Contract.Requires(section != null);

			if (!_sections.Contains(section))
			{
				_sections.Add(section);
			}

			_trackedSectionName = section.Name;
		}

		public IniSection GetCurrentSection()
		{
			return GetSectionByName(_trackedSectionName);
		}

		public IEnumerator<IniSection> GetEnumerator()
		{
			return _sections.GetEnumerator();
		}

		public IniSection GetSectionByName(string sectionName)
		{
			return sectionName != null ? _sections.FirstOrDefault(s => s.Name == sectionName) : null;
		}

		public string[] ToStringArray()
		{
			List<string> data = new List<string>();

			foreach (var section in _sections)
			{
				Contract.Assume(section != null);

				data.AddRange(section.ToStringArray());
			}

			return data.ToArray();
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void __ObjectInvariant()
		{
			Contract.Invariant(_sections != null);
		}
	}
}