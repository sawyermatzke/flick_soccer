using NUnit.Framework;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Tests;

public sealed class SharedValueTypesSerializationTests
{
    [Test] public void Identifiers_RoundTripSerialize_PreserveWrappedValues() => Assert.That(TestJson.RoundTrip(new RoomId("room_123")).Value, Is.EqualTo("room_123"));
    [Test] public void Versioning_RoundTripSerialize_PreserveNumericValues() => Assert.That(TestJson.RoundTrip(new StateVersion(19)).Value, Is.EqualTo(19));
    [Test] public void MetadataEnums_SerializeWithoutCustomConverters() => Assert.That(TestJson.RoundTrip(GeometryReadiness.TopologyReady), Is.EqualTo(GeometryReadiness.TopologyReady));
}
