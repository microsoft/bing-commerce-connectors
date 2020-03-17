# Core Classes

# Table Of Contents

* ### [PollingConnector](#pollingconnector-type).
* ### [SimpleBingCommercePusher](#simplebingcommercepusher-type)
* ### [BufferedBingCommercePusher](#bufferedbingcommercepusher-type).
* ### [DataPoint](#datapoint-type).

# Classes

## PollingConnector `type`

### Summary

A Bing for Commerce Connector that polls data from the given data source in a configurable interval.

### Properties

|Name|Type|Summary|
|----|---|-------|
|ScanInterval|[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)|The interval to use when polling the data.|

### Methods

#### #ctor(PollingConnectorConfigs config, IDataAccess dataAccess, IDataCheckpoint checkpoint, IDataPusher pusher) `constructor`

##### Summary

Creates a new polling connector.

##### Parameters

|Parameter|Type|Summary|
|----|---|-------|
|config|[PollingConnectorConfigs](./core-config.md#pollingconnectorconfigs-type)|The polling connector configuration.|
|dataAccess|[IDataReader](./Core-Interfaces.md#idatareader-type)|The data source to poll data from.|
|checkpoint|[IDataCheckpoint](./Core-Interfaces.md#idatacheckpoint-type)|The checkpoint object to start polling data since.|
|pusher|[IDataPusher](./Core-Interfaces.md#idatapusher-type)|The Bing for Commerce data pusher.|

#### RunAsync(CancellationToken cancellationToken = default(CancellationToken)) `method`

##### Summary

Start running the connector. It first does an initial push for data since the provided checkpoint, and then starts the background job cadence.

##### Parameters

|Parameter|Type|Summary|
|----|---|-------|
|cancellationToken|[CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)|(Optional): The task cancelation token.|

## SimpleBingCommercePusher `type`

### Summary

A simple Bing for Commerce pusher, that can support capping the concurrent push requests.

### Properties

|Name|Type|Summary|
|----|---|-------|
|Serializer|[IPushSerializer](core-interfaces.md#ipushserializer-type)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>>|Gets or sets the serializer used to serialize the given records into string.|

### Methods

#### #ctor(BingCommerceConfig config, IDataCheckpoint checkpoint, IPushSerializer<IDictionary<string, object>> serializer = null) `constructor`

##### Summary

Initializes a new instance of the `SimpleBingCommercePusher` class.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| config | [BingCommerceConfig](./Core-Config.md#bingcommerceconfig-type) | The pusher configurations object. |
| checkpoint | [IDataCheckpoint](./Core-Interfaces.md#idatacheckpoint-type) | The checkpoint to poll the data since if it's valid. |
| serializer | [IPushSerializer](core-interfaces.md#ipushserializer-type)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | (Optional): Explicit serialier to be used. |

#### Push(data) `method`

##### Summary

Use the given data source to push the data after the given checkpoint to the Bing for Commerce endpoint.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| data | [IDataReader](./Core-Interfaces.md#idatareader-type) | the data source to use to poll the data from. |

## BufferedBingCommercePusher `type`

### Summary

A bing for commerce pusher that supports buffering updates for a configurable time period before sending all buffered records, or when it reached the configurable maximum records per request.

### Methods

#### #ctor(config) `constructor`

##### Summary

Creates a new bing commerce buffered pusher object.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| config | [BingCommerceConfig](./Core-Config.md#bingcommerceconfig-type) | The pusher configurations object. | The pusher configuration. |

#### Push(records) `method`

##### Summary

Add a collection of records to the buffer before pushing.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [ICollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection)<[DataPoint](#datapoint-type)> | The records to add. |

## DataPoint `type`

### Summary

Describes the data record that the data source returns to the connector.

### Properties

|Name|Type|Summary|
|----|---|-------|
|Checkpoint|[IDataCheckpoint](./Core-Interfaces.md#idatacheckpoint-type)|Gets or sets the current record checkpoint value.|
|OperationType|[DataOperation](#dataoperation-enum)|Gets or sets the intended operaiton type to do with the current record.|
|Record|[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>|Gets or sets a dicionary describing the record to be pushed.|

## DataOperation `enum`

Describes the change type that happend to the data. Currently only support update.

### Values

|Name|Summary|
|----|-------|
|Update|Represents an update that happened to the data, whether it's a new record or an update to an existing one.|
