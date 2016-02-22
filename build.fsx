// include Fake lib
#r @"FakeLib.dll"
open Fake

let slnFile = getBuildParam "slnFile"
let csProjFile = getBuildParam "csProjFile"
let iisSiteName = getBuildParam "iisSiteName"
let pkgDir = getBuildParam "pkgDir"
let msDeployUrl = getBuildParam "msDeployUrl"
let publishConfiguration =  getBuildParam "publishConfiguration"

Target "RestorePkg" (fun _ ->
    slnFile
    |> RestoreMSSolutionPackages (fun p -> p)
)



/// 这里调整 verbosity
///  let setParams defaults =
///             { defaults with
///                 Verbosity = Some(Quiet)
///                 Targets = ["Build"]
///                 Properties =
///                     [
///                         "Optimize", "True"
///                         "DebugSymbols", "True"
///                         "Configuration", buildMode
///                     ]
///              }
///     build setParams "./MySolution.sln"

Target "BuildSolution" (fun _ ->
    !! "./**/*.sln"
    |> MSBuildWithDefaults "Build"
    |> Log "AppBuild-Output: "
)


"RestorePkg"
    ==> "BuildSolution"

// start build
RunTargetOrDefault "BuildSolution"