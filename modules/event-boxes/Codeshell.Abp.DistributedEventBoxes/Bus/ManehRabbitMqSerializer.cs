using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Json;
using Volo.Abp.RabbitMQ;

namespace Codeshell.Abp.DistributedEventBoxes.Bus;

[ExposeServices(typeof(IRabbitMqSerializer))]
public class ManehRabbitMqSerializer : IRabbitMqSerializer, ITransientDependency
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly bool EventInboxOutboxEnabled;
    public ManehRabbitMqSerializer(IJsonSerializer jsonSerializer, IConfiguration context)
    {
        _jsonSerializer = jsonSerializer;
        var eventInboxOutboxSection = context.GetSection("EventInboxOutbox");
        EventInboxOutboxEnabled = eventInboxOutboxSection.GetValue<bool>("Enable", false);
    }

    public byte[] Serialize(object obj)
    {
        var ser = _jsonSerializer.Serialize(obj);
        return Encoding.UTF8.GetBytes(ser);
    }

    public object Deserialize(byte[] value, Type type)
    {
        var res = Encoding.ASCII.GetString(value).Replace("\"", "");
        if (EventInboxOutboxEnabled)
        {
            var fromBase64 = Convert.FromBase64String(res);
            res = Encoding.UTF8.GetString(fromBase64);
        }
        return _jsonSerializer.Deserialize(type, res);
    }

    public T Deserialize<T>(byte[] value)
    {
        return _jsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(value));
    }
}
