# CHANGELOG
https://keepachangelog.com/en/1.0.0/

## [1.16.12] - 2022-12-30
- Project is updated to C# 11

## [1.16.11] - 2022-12-30
- Simplified the implementation of all codefixes

## [1.16.10] - 2022-12-30
- `AccessingTaskResultWithoutAwait`: Analyzer is rewritten to use `IOperation`

## [1.16.9] - 2022-12-30
- `InstanceFieldWithThreadStatic`: Simplified the `IsStatic` check

## [1.16.8] - 2022-12-30
- `OnPropertyChangedWithoutNameOf`: Analyzer is rewritten to use `IOperation`

## [1.16.7] - 2022-12-30
- `ThrowNull`: Correctly triggers when a constant or casted `null` value is being thrown

## [1.16.6] - 2022-12-30
- `ThreadStaticWithInitializer` and `InstanceFieldWithThreadStatic`: Analyzer is rewritten to use `IOperation`
- Removed unused test project dependencies
- Split up Test and Build workflows

## [1.16.5] - 2022-12-29
- `DateTimeNow`: No longer incorrectly triggers for `nameof(DateTime.Now)` invocations
- `PointlessCollectionToString`: Correctly handles longer chains with nullable annotations, e.g. `SomeClass.SomeCollection?.ToString()`
- `NewGuid`, `DateTimeNow`, `HttpClientInstantiatedDirectly`, `HttpContextStoredInField`, `ThrowNull`, `PointlessCollectionToString`, `MultipleFromBodyParameters`, `LoopedRandomInstantiation` and `ElementaryMethodsOfTypeInCollectionNotOverridden`: Analyzer is rewritten to use `IOperation`

## [1.16.4] - 2022-12-29
- `ParameterAssignedInConstructor`: Analyzer is rewritten to use `IOperation`

## [1.16.3] - 2022-12-29
- `AsyncMethodWithVoidReturnType` and `AttributeMustSpecifyAttributeUsage`: Analyzer is rewritten to use `IOperation`

## [1.16.2] - 2022-12-29
- `SwitchDoesNotHandleAllEnumOptions`: Analyzer is rewritten to use `IOperation`

## [1.16.1] - 2022-12-28
- `StaticInitializerAccessedBeforeInitialization`: Complete rewrite of the analyzer to use `IOperation`. No functional difference but might be more performant

## [1.16.0] - 2022-12-27
- `PointlessCollectionToString`: `.ToString()` was called on a collection which results in impractical output. Considering using `string.Join()` to display the values instead.

## [1.15.0] - 2022-12-25
- `ThreadStaticWithInitializer`: A field is marked as `[ThreadStatic]` so it cannot contain an initializer. The field initializer is only executed for the first thread.
- `StaticInitializerAccessedBeforeInitialization`: When a reference is part of a lambda expression we no longer incorrectly mark it as an error

## [1.14.1] - 2022-10-16
- `AccessingTaskResultWithoutAwait`: Now also works for top-level functions
- `AccessingTaskResultWithoutAwait`: In null-conditional access scenarios such as `file?.ReadAsync().Result`, invalid code will no longer be suggested by the code fix

## [1.14.0] - 2022-10-16
- `LockingOnMutableReference`: A lock was obtained on a mutable field which can lead to deadlocks when a new value is assigned. Mark the field as `readonly` to prevent re-assignment after a lock is taken.
- `ComparingStringsWithoutStringComparison`: Only suggest one code fix at a time
- `UnusedResultOnImmutableObject`: Don't trigger for custom extension methods on the `string` type

## [1.13.1] - 2022-10-1
- `AsyncOverloadsAvailable`: Correctly suggests passing through a `CancellationToken` if the sync overload accepts one as well

## [1.13.0] - 2022-10-1
- `AsyncOverloadsAvailable`: Now passes through a `CancellationToken` if there is one available in the current context
- `AttributeMustSpecifyAttributeUsage`: Takes definitions on base classes into account
- `ElementaryMethodsOfTypeInCollectionNotOverridden`: Supports `HashSet.Add()` and `Dictionary.Add()`

## [1.12.0] - 2022-09-25
- `ParameterAssignedInConstructor`: A parameter was assigned in a constructor

## [1.11.2] - 2022-09-25
- Fixed an issue where in some scenarios, necessary `using` statements were not getting added
- `StaticInitializerAccessedBeforeInitialization`: no longer triggers when passing a method reference

## [1.11.1] - 2022-09-25
- `SwitchIsMissingDefaultLabel`: code fix now works in top-level statements
- `AttributeMustSpecifyAttributeUsage`: correctly fires when the type is defined in the netstandard assembly

## [1.11.0] - 2022-09-24
- `ComparingStringsWithoutStringComparison`: A `string` is being compared through allocating a new `string`, e.g. using `ToLower()` or `ToUpperInvariant()`. Use a case-insensitive comparison instead which does not allocate.
- `UnnecessaryEnumerableMaterialization`: supports `!.` operator
- `ElementaryMethodsOfTypeInCollectionNotOverridden`: supports `?.` and `!.` operators
- `StaticInitializerAccessedBeforeInitialization`: no longer triggers when referencing itself

## [1.10.1] - 2022-09-16
- `StaticInitializerAccessedBeforeInitialization`: supports implicit object creation expressions
- `NewGuid`: supports implicit object creation expressions
- `HttpClientInstantiatedDirectly`: supports implicit object creation expressions

## [1.10.0] - 2022-09-15
- All analyzers and code fixes now have help codes that link back to the individual documentation
- `StaticInitializerAccessedBeforeInitialization`: don't trigger if the referenced field is marked as `const`

## [1.9.4] - 2022-09-13
- `StaticInitializerAccessedBeforeInitialization`: no longer triggers for `Lazy<T>` invocations when a method is passed as argument
- Added documentation for all analyzers to the repo

## [1.9.3] - 2022-09-12
- Internal code cleanup: all warning messages in the tests are now hardcoded

## [1.9.2] - 2022-09-12
- `StaticInitializerAccessedBeforeInitialization`: now takes `nameof()` usage into account
- `StaticInitializerAccessedBeforeInitialization`: no longer triggers for invocations of `static` functions
- `StaticInitializerAccessedBeforeInitialization`: no longer triggers when the field is of type `Action` or `Func`

## [1.9.1] - 2022-09-12
- Internal code cleanup to remove -Async suffixes on tests

## [1.9.0] - 2022-09-11
- `LinqTraversalBeforeFilter`: An `IEnumerable` extension method was used to traverse the collection and subsequently filtered using `Where()`. If the `Where()` filter is executed first, the traversal will have to iterate over fewer items which will result in better performance.
- `LockingOnDiscouragedObject`: A `lock` was taken using an instance of a discouraged type. `System.String`, `System.Type` and `this` references can all lead to deadlocks and should be replaced with a `System.Object` instance instead.

## [1.8.0] - 2022-09-08
- `StaticInitializerAccessedBeforeInitialization`: A `static` field relies on the value of another `static` field which is defined in the same type. `static` fields are initialized in order of appearance.
- `UnboundedStackalloc`: An array is stack allocated without checking whether the length is within reasonable bounds. This can result in performance degradations and security risks

## [1.7.2] - 2022-09-07
- `AttributeMustSpecifyAttributeUsage`: correctly identify when the attribute has been added so it doesn't continue suggesting the change

## [1.7.1] - 2022-09-06
- `StructWithoutElementaryMethodsOverridden`: take `partial struct` definitions into account where the methods are implemented across separate files
- `TestMethodWithoutTestAttribute`: more accurately exclude `Dispose()` methods

## [1.7.0] - 2022-09-05
- `FlagsEnumValuesAreNotPowersOfTwo` has been rewritten to reduce the scope of its warning. Now it will only warn if a non-negative decimal literal is found which is not a power of two. A code fix will be available if a binary OR expression can be constructed with other enum members
- `FlagsEnumValuesDontFit` will no longer fire as this was inaccurate and already covered by the default CA analyzers
- `FlagsEnumValuesAreNotPowersOfTwo` will now mention the enum member that triggered the violation

## [1.6.0] - 2022-09-04
- `AttributeMustSpecifyAttributeUsage`: warn when an attribute is defined without specifying the `[AttributeUsage]`
- All internal code now uses nullable reference types

## [1.5.0] - 2022-09-04
- `MultipleFromBodyParameters`: warn when an API was defined with multiple `[FromBody]` parameters that attempt to deserialize the request body
- Include README in nuget package

## [1.4.2] - 2022-09-04
- CI will ensure the version has been updated appropriately before releasing a new package
- CI will run its `dotnet format` check much faster

## [1.4.1] - 2022-09-03
- `TestMethodWithoutTestAttribute`: improved the accuracy of discovering `TestClass` and `TestFixture` attributes

## [1.4.0] - 2022-09-03
- `InstanceFieldWithThreadStatic`: warn when `[ThreadStatic]` is applied to an instance field
- Removed `StructShouldNotMutateSelf`
- Restructured the diagnostic categories into _Performance_, _ApiDesign_ and _Correctness_

## [1.3.1] - 2022-09-03
- `ThreadSleepInAsyncMethod` does not suggest a no-op refactor if the method is not marked as `async`

## [1.3.0] - 2022-09-02
- `ElementaryMethodsOfTypeInCollectionNotOverridden` is more targeted and only warns if it finds an actual lookup that will be problematic
- `ExceptionThrownFromProhibitedContext` doesn't crash when encountering empty `throw` statements
- `ExceptionThrownFromProhibitedContext` doesn't crash when encountering `throw` statements that reference properties
- `AsyncOverloadsAvailable` will no longer suggest to use an overload if that overload is the current surrounding method
- `AsyncOverloadsAvailable` now works inside lambda expressions as well
- `UnusedResultOnImmutableObject` doesn't trigger on `CopyTo` and `TryCopyTo`

## [1.2.4] - 2022-08-31
- Fixed: `AsyncOverloadsAvailable` supports methods that return `ValueTask`
- Fixed: `AccessingTaskResultWithoutAwait` supports methods that return `ValueTask`
- Fixed: `ThreadSleepInAsyncMethod` supports methods that return `ValueTask`
- `AsyncMethodWithVoidReturnType` now also works for top-level function declarations and local functions
- `ThreadSleepInAsyncMethod` now also works for top-level function declarations and local functions

## [1.2.3] - 2022-08-29
- Fixed: `GetHashCodeRefersToMutableMember` correctly handles `partial` classes
- Fixed: `EqualsAndGetHashcodeNotImplemented` correctly handles `partial` classes

## [1.2.2] - 2022-08-29
- Fixed: `AsyncOverloadsAvailable` wraps the `await` expression with parentheses when the function return value is accessed inline
- Fixed: `AsyncOverloadsAvailable` no longer suggests a change if it would result in invalid code
- Fixed: `AsyncOverloadsAvailable` now also reports improvements when using top-level statements
- Fixed: `AsyncOverloadsAvailable` takes nullable reference types into account when selecting an overload
- `EqualsAndGetHashcodeNotImplementedTogether` now mentions the class name in the diagnostic message

## [1.2.1] - 2022-08-29
- Fixed: `ElementaryMethodsOfTypeInCollectionNotOverridden` triggers for external types
- Fixed: `ExceptionThrownFromProhibitedContext` will no longer trigger for `NotSupportedException` and `NotImplementedException`
- Fixed: `TestMethodWithoutTestAttribute` no longer crashes when encountering a `record`
- Fixed: `TestMethodWithoutTestAttribute` no longer triggers for `Dispose()` methods

## [1.2.0] - 2022-08-28
- Implemented `UnnecessaryEnumerableMaterialization`: An `IEnumerable` was materialized before a deferred execution call
- `SwitchDoesNotHandleAllEnumOptions` produces more accurate code when static imports cause enum members to conflict
- SharpSource its unit tests now run on .NET 6.0

## [1.1.0] - 2022-08-28
- Implemented `UnusedResultOnImmutableObject`: The result of an operation on an immutable object is unused

## [1.0.0] - 2022-08-28
- Implemented `EnumWithoutDefaultValue`: An enum should specify a default value
- Changed the categories of `ExplicitEnumValues`, `FlagsEnumValuesAreNotPowersOfTwo` and `FlagsEnumValuesDontFit`
- Improved messaging for `DateTimeNow`
- Added documentation

## [0.9.0] - 2022-08-26
- Implemented `HttpContextStoredInField`: show a warning when `HttpContext` was stored in a field. Use `IHttpContextAccessor` instead
- Fixed DiagnosticID of `HttpClientInstantiatedDirectly`

## [0.8.0] - 2022-08-24
- Implemented `HttpClientInstantiatedDirectly`: show a warning when `HttpClient` is instantiated. Use `IHttpClientFactory` instead

## [0.7.0] - 2022-08-23
- Implemented `ExplicitEnumValues`: show a warning when an enum does not explicitly specify its value

## [0.6.0] - 2022-08-15
- Automatically publish updates to Github Packages

## [0.5.0] - 2022-08-15
- Automatically publish updates to the VSIX marketplace

## [0.4.0] - 2022-08-08
- Don't trigger `ElementaryMethodsOfTypeInCollectionNotOverridden` for enums
- Don't trigger `ElementaryMethodsOfTypeInCollectionNotOverridden` for arrays
- `DateTimeNow` now shows the correct code fix title action