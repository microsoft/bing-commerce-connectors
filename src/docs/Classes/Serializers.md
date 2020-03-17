# Serializers

# Table Of Contents

*  ### [CSVSerializer](#csvserializer-type)
*  ### [TSVSerializer](#tsvserializer-type)
*  ### [JsonArraySerializer](#jsonarrayserializer-type)
*  ### [NDJsonSerializer](#ndjsonserializer-type)

# Classes  

## CSVSerializer `type`

### Summary

A utility to serialize the batch of records in Comma-Separated Values format.

### Methods

#### ctor()

Initializes a new instance of the CSVSerializer class.

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects in Comma-Separated Values format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

 Serializes a single record object in Comma-Separated Values format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string.


### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for CSV Serialization.|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for CSV Serialization|


## TSVSerializer `type`

### Summary

A utility to serialize the batch of records in Tab-Separated Values format.

### Methods

#### ctor()

Initializes a new instance of the TSVSerializer class.

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects in Tab-Separated Values format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

 Serializes a single record object in Tab-Separated Values format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string in Tsv format.

### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for Tsv Serialization.|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for Tsv Serialization|

## JsonArraySerializer `type`

### Summary

A utility to serialize the batch of records in JSon Array format.

### Methods

#### ctor()

Initializes a new instance of the JsonArraySerializer class.

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects in Json Array format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

Serializes a single record object in Json Array format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string.

### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. That is the sqauare brackets for the Json Array.|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the comma for Json Array Serialization.|



## NDJsonSerializer `type`

### Summary

A utility to serialize the batch of records in New-line Delimited Json format.

### Methods

#### ctor()

Initializes a new instance of the NDJsonSerializer class.

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects in New-line Delimited Json format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

Serializes a single record object in New-line Delimited Json format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string.

### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for NDJson Serialization.|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for NDJson Serialization.|

## FormatSerializer `type`

### Summary

A utility to serialize the batch of records in the one of select format.

### Methods

#### ctor(Format serializerFormat)

##### Summary

Initializes a new instance of the FormatSerializer class.

##### Parameters

| Name | Type | Description |
| ----| ---- | ----------- |
| serializerFormat | [Format](./Core-Config.md#format-enum) | The selected format to intialize the object with. |

#### Serialize(IEnumerable<IDictionary<string, object>> records) `method`

##### Summary

Serializes a batch of record objects in New-line Delimited Json format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)<[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)>> | The records to be serialized. |

##### Returns

The batch of objects serialized as string.

#### Serialize(IDictionary<string, object> record) `method`

##### Summary

Serializes a single record object in New-line Delimited Json format.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| record | [IDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<[string](https://docs.microsoft.com/en-us/dotnet/api/system.string), [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types)> | The record object to be serialized. |

##### Returns

The object serialized as string.

### Properties

|Name|Type|Summary|
|----|---|-------|
|OverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added to any number of serialized records in order to complete the batch. That is zero for NDJson Serialization.|
|RecordOverheadSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets the number of characters added as an overhaed to one serialized record in order to be able to concatenate it with other serialized records. That is the new line characters for NDJson Serialization.|