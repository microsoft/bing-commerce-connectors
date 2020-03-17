# Utility classes

# Table Of Contents

*  ### [BackgroundTaskScheduler](#backgroundtaskscheduler-type)
*  ### [BingCommerceIngestionWithRetry](#bingcommerceingestionwithretry-type)
*  ### [Require](#require-type)
*  ### [RetryStrategy](#retrystrategy-type)

# Classes  

## BackgroundTaskScheduler `type`

### Summary

 A utility class that provides a way to schedule a recurring task with a specific interval.

### Methods

#### ctor(TimeSpan interval)

Initializes a new instance of the BackgroundTaskScheduler class.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| interval | [TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan) | The interval to run the task at. |

#### StartAsync(Action action, CancellationToken cancellation = default(CancellationToken)) `method`

##### Summary

Start the scheduler run.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| action | [Action](https://docs.microsoft.com/en-us/dotnet/api/system.action) | The action to perform on the scheduler's stops. |
|cancellation|[CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)|(optional): the cancellation token that you can use to stop the run.|

##### Returns

The running task for the scheduler.

## BingCommerceIngestionWithRetry `type`

### Summary

A wrapper around the Bing for Commerce Ingestion client in with added retry option. 

### Methods

#### ctor(IBingCommerceIngestion internalClient, uint maxAttempts, uint initialBackoffMs)

Initializes a new instance of the BingCommerceIngestionWithRetry class, which wraps the Bing for Commerce client with added retry logic.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalClient | IBingCommerceIngestion | The wrapped bing client for added retry option. |
| maxAttempts | [uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types) | The maximum number of attempts to call an API before failing the request. |
| initialBackoffMs | [uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types) |The initial backoff wait time between attempts in milliseconds. |

### Remarks

For more details about the specific Methods and Properties for BingCommerceIngestion client, please check the Bing for Commerce documentation.

## Require `type`

### Summary

A utility to validate arguments, state and others that uses the fluent pattern.

### Methods

#### IsNotNull(object argument, string name) `method`

##### Summary

Validates the given argument is not null, otherwise throws an ArgumentNullException.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| argument | [object](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types) |The argument to validate. |
| name | [string](https://docs.microsoft.com/en-us/dotnet/api/system.string) | The name of the argument. |

##### Returns

this, to use with the fluent pattern.

#### IsTrue(bool condition, string message) `method`

##### Summary

Validates the given condition is true, otherwise throws an ArgumentException.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| condition | [bool](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool) | The condition to validate. |
| message | [string](https://docs.microsoft.com/en-us/dotnet/api/system.string) | The message to give to the exception in case the condition is false. |

##### Returns

this, to use with the fluent pattern.

#### State(bool condition, string message) `method`

##### Summary

Validates the given condition is true, otherwise throws an InvalidOperationException.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| condition | [bool](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool) | The condition to validate. |
| name | [string](https://docs.microsoft.com/en-us/dotnet/api/system.string) | The name of the argument. |

##### Returns

this, to use with the fluent pattern.

#### OrDefault&lt;T>(ref T value, bool condition, T defaultVal) `method`

##### Summary

Assigns a default to a given object in case a certain condition is met.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | ref T | The object to validate and assign to default if a certain condition is met. |
| condition | [bool](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool) | The condition that the value would be set to default in case it's met. |
| defaultVal | T | The default value to set the object to. |

##### Returns

this, to use with the fluent pattern.

#### OrDefault&lt;T>(ref T value, T inCase, T defaultVal) `method`

##### Summary

Assigns a default to a given object in case it's a certain value.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | ref T | The object to validate and assign to default if it's a certain value. |
| inCase | T | The value to compare the object to.
| defaultVal | T | The default value to set the object to. |

##### Returns

this, to use with the fluent pattern.

### Properties

|Name|Type|Summary|
|----|---|-------|
|Instance|Require|Returns an instance of the Require utility class.|

## RetryStrategy `type`

### Summary

A static utility class to help us retry operations in case of failure.

### Methods

#### RetryAsync&lt;T>(Func<Task<T>> func, uint maxAttempts, uint initialBackoffMs, Action<Exception> exceptionCallback = null) `method`

##### Summary

Attempt to invoke the given function until it succeeds, or the maximum number of attempts have been reached.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
|func|[Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)&lt;T>>|The function to attempt to invoke.|
|maxAttempts|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|The maximum number of attempts to invoke the given function. Has to be greater than zero.|
|initialBackoffMs|[uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)|The initial backoff time wait between attempts in milliseconds. Has to be greater than zero.|
|exceptionCallback|[Action](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)>|(Optional): A callback to be invoked when the operation fails at any attempt.|

##### Returns

Forwards the return value from the given function.
