# Version 1.0.1.1 (2020-07-07)

## Minor Changes
### Tweaks
* Updated C# bridge interface ([bd7066](https://bitbucket.org/voxon-photonics/c-bridge/commit/bd70663e5b6e74978209413258b0e20324824de2))

# Version 0 - 1.0.1.0 (2020-07-07)

## Features
*  Added automatic versioning and signing to pipeline
* Implemented Touch Screen menu creation functions
* Added functions for set/get Emulator Angle and Distance
* Added Implementation for Color and Menu Options (Dual Color Mode currently disabled)
* Added DrawVoxels and settings for Helix mode
* Added Helix support (including new tests)
* Added DLL / SDK version functions
* Added Space Nav Support
* Added ToString for Point3d

## Deprecations
* Removed unused IRuntime

## Tweaks
* Updated C# bridge interface
* Reimplemented Dual Colour; fixed not moving into correct color mode
* Updated voxiewindow_t structure to include asprmin, and should be able to handle duplicate slashes in paths for dll
* Updated project to reflect new name for C# Bridge Interface
* Added current interface DLL (for quick building), renamed project to reflect role as C# bridge rather than Unity based
* Converted RuntimePromise to an Interface, updated all function calls in Runtime based on this, reordered priority to take local dll first.
* Made poltex_t serializable for preBuild compression
* Updated Voxon SDK Path

