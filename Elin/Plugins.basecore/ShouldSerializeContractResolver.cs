using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class ShouldSerializeContractResolver : DefaultContractResolver
{
	public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		JsonProperty property = base.CreateProperty(member, memberSerialization);
		if (!typeof(string).IsAssignableFrom(property.PropertyType) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
		{
			Predicate<object> newShouldSerialize = (object obj) => !(property.ValueProvider.GetValue(obj) is ICollection collection) || collection.Count != 0;
			Predicate<object> oldShouldSerialize = property.ShouldSerialize;
			property.ShouldSerialize = ((oldShouldSerialize != null) ? ((Predicate<object>)((object o) => oldShouldSerialize(o) && newShouldSerialize(o))) : newShouldSerialize);
		}
		return property;
	}
}
