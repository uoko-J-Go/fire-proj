// include Fake lib
#r @"Uoko.FireProj/packages/FAKE/tools/FakeLib.dll"
open Fake

// Default target
Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// start build
RunTargetOrDefault "Default"