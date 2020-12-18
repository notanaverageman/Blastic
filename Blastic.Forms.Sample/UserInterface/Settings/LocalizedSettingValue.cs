using System;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Settings
{
	public class LocalizedSettingValue<T> : IEquatable<LocalizedSettingValue<T>>
	{
		public T Value { get; }
		public IReadOnlyReactiveProperty<string> Name { get; }

		public IReactiveProperty<string> DisplayName { get; }
		public string NameValue => Name.Value;

		public LocalizedSettingValue(T value, IReadOnlyReactiveProperty<string> name)
		{
			Value = value;
			Name = name;

			DisplayName = new ReactiveProperty<string>(nameof(NameValue));

			Name.Subscribe(_ =>
			{
				DisplayName.Value = "";
				DisplayName.Value = nameof(NameValue);
			});
		}

		public bool Equals(LocalizedSettingValue<T>? other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Value?.Equals(other.Value) == true && Name.Equals(other.Name);
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((LocalizedSettingValue<T>)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Value, Name);
		}

		public static bool operator ==(LocalizedSettingValue<T>? left, LocalizedSettingValue<T>? right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LocalizedSettingValue<T>? left, LocalizedSettingValue<T>? right)
		{
			return !Equals(left, right);
		}
	}
}