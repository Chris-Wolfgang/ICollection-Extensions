type: internal

`dotnet pack` now validates the package against the last published version (0.3.1), so an unintended breaking change or dropped target framework fails the release build.
