using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using ProtoBuf;

namespace DynamoDb.EventSourcing.Extensions
{
	internal static class DynamoDbHelper
	{
		public static T CreateFromItem<T>(Dictionary<string, AttributeValue> item)
		{
			var data = item.FirstOrDefault(p => p.Key == "Data");

			return JsonConvert.DeserializeObject<T>(Document.FromAttributeMap(data.Value.M).ToJson());
		}
		
		public static T CreateFromBinaryItem<T>(Dictionary<string, AttributeValue> item)
		{
			try
			{
				var data = item.FirstOrDefault(p => p.Key == "Data");

				var byteArray = Convert.FromBase64String(data.Value.S);

				using var ms = new MemoryStream(byteArray);

				return Serializer.Deserialize<T>(ms);
			}
			catch (Exception)
			{
				// If fails, may be an old event stored in JSON. Attempt to deserialize that way.
				return DynamoDbHelper.CreateFromItem<T>(item);
			}
		}
	}
}