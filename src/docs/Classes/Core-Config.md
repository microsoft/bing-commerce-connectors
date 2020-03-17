# Core Configurations

# Table Of Contents

* ### [PollingConnectorConfigs](#pollingconnectorconfigs-type).
* ### [BingCommerceConfig](#bingcommerceconfig-type).
* ### [Format](#format-enum).
* ### [RequestLogLevel](#requestloglevel-enum)

# Classes and Enums

## PollingConnectorConfigs `type`

### Summary

The configurations needed for the polling connector.

### Properties

|Name|Type|Summary|
|----|---|-------|
|ScanCadence|[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)|Gets or sets the interval to use when polling the data|

## BingCommerceConfig `type`

### Summary

The configuration needed for the Bing for Commerce pusher.

### Properties

|Name|Type|Summary|
|----|---|-------|
|TenantId|[string](https://docs.microsoft.com/en-us/dotnet/api/system.string)|Gets or sets the tenant id to push the data to.|
|IndexId|[string](https://docs.microsoft.com/en-us/dotnet/api/system.string)|Gets or sets the index to push the data to.|
|AccessToken|[string](https://docs.microsoft.com/en-us/dotnet/api/system.string)|Gets or sets the Bearer access token to use when pushing the data. Will get from the environment variable `ACCESS_TOKEN` if missing.|
|PushFormat|[Format](#format-enum) |Gets or sets the format to use when pushing the data to the Bing for Commerce endpoint.|
|MaxBatchCount|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets or sets the maximum number of records per push request.|
|MaxRequestSize|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets or sets the maximum size of the request sent to the Bing API in bytes.|
|MaxConcurrentRequests|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|gets or sets the maximum number of concurrent requests to use when pushing.|
|RequestLog|[RequestLogLevel](#requestloglevel-enum) |Gets or sets the desired level for logging the full requests.|
|RequestLogLocation|[string](https://docs.microsoft.com/en-us/dotnet/api/system.string)|Gets or sets the directory you wish to store the full requests.|
|TrackingInterval|[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)?|Gets or sets the interval on which to track the push update requests.|
|RetryCount|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets or sets number of retries before deadlettering the request.|
|MaxBufferWaitMs|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|Gets or sets the maximum wait in-between events to wait for before pushing (For the buffered pusher).|

## Format `enum`

### Summary

The format to use when pushing to the Bing for Commerce endpoint.

### Values

|Name|Summary|
|----|-------|
|JsonArray|A json array of objects.|
|NDJson|New-line Delimited JSon objects.|
|Csv|Comma-Separated Values.|
|Tsv|Tab-Separated Values.|

## RequestLogLevel `enum`

### Summary

The requested level for logging full requests..

### Values

|Name|Summary|
|----|-------|
|None|No request logging is requested.|
|DeadletterOnly|Only log failed requests / records.|
|All|Log everything.|
