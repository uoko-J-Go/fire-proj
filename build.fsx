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

Target "BuildSolution" (fun _ ->
    !! "./**/*.sln"
    |> MSBuildWithDefaults "Build"
    |> Log "AppBuild-Output: "
)


"Clean"
    ==> "RestorePkg"
    ==> "BuildSolution"

// start build
RunTargetOrDefault "BuildSolution"