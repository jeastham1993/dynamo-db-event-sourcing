using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoDb.EventSourcing.Models;
using Newtonsoft.Json;
using ProtoBuf;

namespace DynamoDb.EventSourcing.Extensions
{
    /// <summary>
    /// Extensions classes for the <see cref="StoredEvent"/> class.
    /// </summary>
    internal static class StoredEventExtension
    {
	    internal static Dictionary<string, AttributeValue> AsAttributeMap(this StoredEvent storedEvent)
	    {
		    if (storedEvent == null)
		    {
			    throw new ArgumentNullException(nameof(storedEvent));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5)
		    {
			    { "PK", new AttributeValue($"EVENT#{storedEvent.AggregateId.ToUpper().Trim().Replace(" ", string.Empty)}") },
			    { "SK", new AttributeValue($"{storedEvent.DateRaised:yyyyMMddHHmmssFFF}#{storedEvent.EventType.ToUpper()}") },
			    { "Type", new AttributeValue(storedEvent.EventType) },
			    { "StoredAs", new AttributeValue("Binary") },
			    {
				    "Data", new AttributeValue(storedEvent.AsBinaryString())
			    },
		    };

		    return attributeMap;
	    }

	    internal static Dictionary<string, AttributeValue> AsData(
		    this StoredEvent evt)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(evt));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }

	    internal static string AsBinaryString(
		    this StoredEvent evt)
	    {
		    var msTestString = new MemoryStream();
		    Serializer.Serialize(msTestString, evt);

		    return Convert.ToBase64String(msTestString.ToArray());
	    }
    }
}