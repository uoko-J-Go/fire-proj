// include Fake lib
#r @"FakeLib.dll"
//#r @"D:/fake/tools/FakeLib.dll"

open Fake
open System


let slnFile = getBuildParam "slnFileDefault" |> getBuildParamOrDefault "slnFile" 
let csProjFile = getBuildParam "csProjFile"
let iisSiteName = getBuildParam "iisSiteName"
let pkgDir = getBuildParam "pkgDir"
let msDeployUrl = getBuildParam "msDeployUrl"
let publishConfiguration =  getBuildParam "publishConfiguration"

let onlyBuild = isNullOrEmpty msDeployUrl

Target "RestorePkg" (fun _ ->
    slnFile
    |> RestoreMSSolutionPackages (fun p -> p)
)



Target "BuildSolution" (fun _ ->
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
        
    !! "./**/*.sln"
    |> Seq.toList
    |> List.head
    |> build setParams
)


///     let result = ExecProcess (fun info ->  
///                       info.FileName <- "c:/MyProc.exe"
///                       info.WorkingDirectory <- "c:/workingDirectory"
///                       info.Arguments <- "-v") (TimeSpan.FromMinutes 5.0)
///     
///     if result <> 0 then failwithf "MyProc.exe returned with a non-zero exit code"


Target "PackageProject" (fun _ ->
    let setParams defaults =
        {
            defaults with
                Verbosity = Some(Minimal)
                Targets = ["Build"]
                Properties =
                    [
                        "Configuration",publishConfiguration
                        "DeployOnBuild","True"
                        "PackageLocation",pkgDir
                    ]
        }

    build setParams csProjFile
)

Target "Deploy" (fun _ ->
    let exitCode = ExecProcess (fun info ->
                    info.FileName <- "D:\deploy-pkg\Uoko.FireProj.WebSite.deploy.cmd"
                    info.Arguments <- sprintf "/Y /U:simple-deploy /P:deploy /A:Basic -allowUntrusted \"-setParam:name='IIS Web Application Name',value='%s'\" \"/M:%s?site=%s" iisSiteName msDeployUrl iisSiteName) (TimeSpan.FromMinutes 1.0)
    if exitCode <> 0 then failwithf "packageProject cmd failed with a non-zero exit code %d."  exitCode
)

"RestorePkg"
    ==> "BuildSolution"

"PackageProject"
    ==> "Deploy"


if onlyBuild then
    // start build
    RunTargetOrDefault "BuildSolution"
else
    RunTargetOrDefault "Deploy"