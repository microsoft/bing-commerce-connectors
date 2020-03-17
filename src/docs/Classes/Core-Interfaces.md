# Core Interfaces

# Table Of Contents

* ### [IDataReader](#idatareader-type).
* ### [IDataCheckpoint](#idatacheckpoint-type).
* ### [IDataPusher](#idatapusher-type).

# Interfaces

## IDataReader `type`

### Summary

An interface that the connector can use to poll the data from the data source.

### Methods

#### ReadNext(checkpoint) `method`

##### Summary

Read the next batch of data from the data source, after the given checkpoint.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| checkpoint | IDataCheckpoint | The checkpoint to read the data since if valid. |

##### Returns

Lists the new data since the given checkpoint.

## IDataCheckpoint `type`

### Summary

An interface that describes the a marker to start polling the data after (e.g. a timestamp, change id, ..etc).

### Methods

#### Accept(newCheckpoint) `method`

##### Summary

Accept the changes, and move the checkpoint to the new given checkpoint value.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| newCheckpoint | string | The new checkpoint value to move the current checkpoint to. |

#### GetValue() `method`

##### Summary

Gets the value of the current checkpoint as a string.

##### Returns

The vlaue of the checkpoint

#### IsValid() `method`

##### Summary

This would be false for the first connector run, which means that the connector should poll all data.

##### Returns

true if the current checkpoint hold a valid checkpoint, false if .

## IDataPusher `type`

### Summary

An interface to describe the strategy that's used to push the data.

### Methods 

#### Push(data,checkpoint) `method`

##### Summary

Use the given data source to push data to the Bing for Commerce endpoint.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| data | IDataReader | The data source to use to poll the data from. |

## IPushSerializer `type`

### Summary

Interface to describe how to serialize the push batch before sending to the Bing for Commerce ingestion API.

### Methods

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

Serializes a single record object.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string.

### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. For example, in JSonArray, this would be 2 (for the angle brackets).|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. For example, in JSonArray, this would be 1 (for the coomma).|