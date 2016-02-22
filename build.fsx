// include Fake lib
#r @"FakeLib.dll"
open Fake

let srcDir = "./Uoko.FireProj/"
let slnFilePath = "./Uoko.FireProj/Uoko.FireProj.sln"
let buildDir = "./testBuildDir/"

let slnFile = getBuildParam "slnFile"
let csProjFile = getBuildParam "csProjFile"
let iisSiteName = getBuildParam "iisSiteName"
let pkgDir = getBuildParam "pkgDir"
let msDeployUrl = getBuildParam "msDeployUrl"

Target "RestorePkg" (fun _ ->
    slnFilePath
    |> RestoreMSSolutionPackages (fun p ->
    {
        p with
            OutputPath = srcDir + "/packages"
    })
)

Target "Clean" (fun _ ->
    CleanDir buildDir;
)

Target "Build" (fun _ ->
    !! "./**/*.sln"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

// Default target
Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// Default target
Target "Test" (fun _ ->
    trace ("Test from FAKE => " + testParam + testParam1) 
)


"Clean"
    ==> "RestorePkg"
    ==> "Build"
    ==> "Default"

// start build
RunTargetOrDefault "Test"