using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blastic.ViewManagement.TypeMappers
{
	public class SuffixTypeMapper : ITypeMapper
	{
		private readonly Dictionary<string, Type> _viewTypeCache;
		private readonly string _viewSuffix;
		private readonly string _viewModelSuffix;

		public SuffixTypeMapper(
			IEnumerable<Assembly> viewAssemblies,
			string viewSuffix,
			string viewModelSuffix)
		{
			_viewSuffix = viewSuffix;
			_viewModelSuffix = viewModelSuffix;

			_viewTypeCache = new Dictionary<string, Type>();

			foreach (Type viewType in viewAssemblies.SelectMany(x => x.GetTypes()))
			{
				string typeName = viewType.FullName;

				if (typeName == null)
				{
					continue;
				}

				if (!typeName.EndsWith(_viewSuffix))
				{
					continue;
				}

				typeName = NormalizeTypeName(typeName);
				_viewTypeCache[typeName] = viewType;
			}
		}

		public Type? Map(Type type)
		{
			string typeName = type.FullName;

			if (typeName == null)
			{
				return null;
			}

			typeName = NormalizeTypeName(typeName);

			int indexOfViewModelSuffix = typeName.IndexOf(_viewModelSuffix);

			if (indexOfViewModelSuffix < 0)
			{
				return null;
			}

			string viewTypeName = $"{typeName.Substring(0, indexOfViewModelSuffix)}{_viewSuffix}";

			return _viewTypeCache.TryGetValue(viewTypeName, out Type viewType)
				? viewType
				: null;
		}

		private string NormalizeTypeName(string typeName)
		{
			int indexOfBacktick = typeName.IndexOf('`');

			if (indexOfBacktick < 0)
			{
				return typeName;
			}

			return typeName.Substring(0, indexOfBacktick);
		}
	}
}