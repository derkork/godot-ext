# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2022-07-08
### Added
- Support for multiple .net targets. Now targets `netstandard2.1` in addition to `net472`.
- Documentation for `FindAllDescendants` and `FindClosestDescendants`.

## [0.1.0] - 2022-06-26
### Added
- More documentation was added to several functions.
- The package is now built and published to the NuGet repository automatically.

### Changed
- `Sleep` is now renamed to `SleepSeconds` to indicate that the parameter takes seconds (not milliseconds). The `Sleep` method is still available, but it is deprecated and will be removed in a future version.

### Fixed
- `ScreenToWorld` and `WorldToScreen` are now public and no longer internal.
- `FindBelowInGroup` will no longer crash if the group contains nodes of different types.


## [0.0.1]

- Initial release