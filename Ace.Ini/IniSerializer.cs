namespace Ace.Ini
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;

	public class IniSerializer
	{
		public T Deserilize<T>(string data) where T : class
		{
			Contract.Requires(!string.IsNullOrWhiteSpace(data));

			IniSectionCollection sections = IniSectionCollection.Parse(data);

			Stack<ParentInfo> parents = new Stack<ParentInfo>();

			T root = Activator.CreateInstance<T>();

			IniSection noSection = sections.GetSectionByName(string.Empty);

			parents.Push(new ParentInfo(root, noSection));

			while (parents.Count > 0)
			{
				var parent = parents.Pop();

				Contract.Assume(parent != null);

				object instance = parent.Instance;

				Type type = instance.GetType();

				foreach (var property in type.GetProperties())
				{
					Type propertyType = property.PropertyType;

					Type declaringType = property.DeclaringType;

					if (propertyType == typeof(string))
					{
						IniSection section = parent.Section;

						if (section != null)
						{
							string keyName = property.Name;
							string keyValue = string.Empty;

							IniKeyAttribute keyAttribute = property.GetAttribute<IniKeyAttribute>();

							if (keyAttribute != null && keyAttribute.Key != null)
							{
								IniKey actualKey = keyAttribute.Key;

								keyName = actualKey.Name;
								keyValue = actualKey.Value;
							}

							IniKey key = section.Keys.GetKeyByName(keyName);

							if (key != null)
							{
								keyValue = string.IsNullOrEmpty(key.Value) ? keyValue : key.Value;

								property.SetValue(instance, keyValue, null);
							}
						}
					}
					else if (declaringType == typeof(T))
					{
						string sectionName = property.Name;

						IniSectionAttribute sectionAttribute = property.GetAttribute<IniSectionAttribute>();

						if (sectionAttribute != null && sectionAttribute.SectionName != null)
						{
							sectionName = sectionAttribute.SectionName;
						}

						IniSection section = sections.GetSectionByName(sectionName);

						if (section != null)
						{
							object target = Activator.CreateInstance(propertyType);

							property.SetValue(instance, target, null);

							parents.Push(new ParentInfo(target, section));
						}
					}
					else
					{
						throw new InvalidOperationException(string.Format("Type '{0}' can contain only properties that their return type is a string.", declaringType));
					}
				}
			}

			return root;
		}

		private sealed class ParentInfo
		{
			public ParentInfo(object instance, IniSection section)
			{
				Contract.Requires(instance != null);

				Instance = instance;

				Section = section;
			}

			public object Instance { get; private set; }

			public IniSection Section { get; private set; }

			[ContractInvariantMethod]
			[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
			private void __ObjectInvariant()
			{
				Contract.Invariant(Instance != null);
			}
		}
	}
}