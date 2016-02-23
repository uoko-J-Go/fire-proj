// include Fake lib
#r @"F1akeLib.dll"
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

let setParams defaults =
    {
        defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Configuration","Release"
                ]
    }


Target "BuildSolution" (fun _ ->
    !! "./**/*.sln"
    |> Seq.toList
    |> List.head
    |> build setParams
)



"RestorePkg"
    ==> "BuildSolution"

// start build
RunTargetOrDefault "BuildSolution"
