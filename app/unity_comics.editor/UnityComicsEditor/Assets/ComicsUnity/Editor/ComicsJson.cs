using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ComicsUnity
{
	public static class ComicsJson
	{
		public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			DefaultValueHandling = DefaultValueHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto
		};

		public static string ToJson(this object obj) =>
			JsonConvert.SerializeObject(obj, SerializerSettings);

		public static T FromJson<T>(this string data)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(data, SerializerSettings);
			}
			catch
			{
				return default;
			}
		}

		public static string GetEnumName(this Enum value)
		{
			var name = value.ToString();
			var field = value.GetType().GetField(name);
			if (field != null)
			{
				var attr = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
				if (attr != null)
					name = attr.Name;
			}
			return name;
		}
	}
}
