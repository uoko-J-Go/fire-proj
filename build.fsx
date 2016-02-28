// include Fake lib
#r @"FakeLib.dll"
//#r @"D:/fake/tools/FakeLib.dll"

open Fake
open Fake.Git
open System


let getBuildParamEnsure name =
    let value = environVar name
    if isNullOrWhiteSpace value then failwithf "environVar of %s is null or whitespace" name
    else value

let slnFile = 
    !! "./**/*.sln"
    |> Seq.toList
    |> List.head
    |> getBuildParamOrDefault "slnFile"

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
            
    RestoreMSSolutionPackages (fun p -> p) slnFile
    build setParams slnFile
)

let pkgProject pkgDir =
    let useConfig = getBuildParamEnsure "useConfig"
    
    let setParams defaults =
        {
            defaults with
                Verbosity = Some(Minimal)
                Targets = ["Build"]
                Properties =
                    [
                        "DeployOnBuild", "True"
                        "Configuration", useConfig
                        "PackageLocation", pkgDir
                    ]
        }
    
    (getBuildParamEnsure "csProjFile") |> build setParams

    
let deploy =
    RestoreMSSolutionPackages (fun p -> p) slnFile

    let pkgDir = getBuildParamEnsure "pkgDir"

    pkgProject pkgDir

    let msDeployUrl = getBuildParamEnsure "msDeployUrl"
    let iisSiteName = getBuildParamEnsure "iisSiteName"
    let deployUser = getBuildParamEnsure "deployUser"
    let deployPwd = getBuildParamEnsure "deployPwd"
    

    let exitCode = ExecProcess (fun info ->
                    info.FileName <- pkgDir + "\Uoko.FireProj.WebSite.deploy.cmd"
                    info.Arguments <- sprintf "/Y /U:%s /P:%s /A:Basic -allowUntrusted \"-setParam:name='IIS Web Application Name',value='%s'\" \"/M:%s?site=%s" 
                     deployUser deployPwd iisSiteName msDeployUrl iisSiteName) (TimeSpan.FromMinutes 1.0)
    if exitCode <> 0 then failwithf "deploy cmd failed with a non-zero exit code %d."  exitCode


Target "Deploy-To-IOC" (fun _ ->
    deploy
)

let ensureOnBranch branchNeeded =
    let branchName = getBranchName null
    if branchName <> branchNeeded then failwithf "you need do this only on [%s] branch,but now you are on [%s]" branchNeeded branchName
        
let ffMergeAndDeploy onBranch =
    let mergeFromBranch = getBuildParamEnsure "mergeFromBranch"

    merge null "--ff-only" mergeFromBranch

    if onBranch = "master" then
        let onlineDate = System.DateTime.Today.Date.ToString("yyyy-MM-dd")
        let tagName = getBuildParamEnsure "onlineTagName"
        gitCommand null (sprintf "tag -a v-%s-%s -m 'deploy %s to %s'" tagName onlineDate mergeFromBranch onBranch)
    else
        gitCommand null (sprintf "tag -a %s-to-%s -m 'deploy %s to %s'"  mergeFromBranch onBranch mergeFromBranch onBranch)
                
    deploy

    gitCommand null "push --follow-tags"


(*
    做 合并 git pull origin branch-xxx --ff-only
    deploy
    tag & push  git tag -a xxx-to-pre -m "deploy xxx to pre" & git push --follow-tags
*)
Target "Deoply-To-PRE" (fun _ ->
    let branchPre = "pre"
    ensureOnBranch branchPre
    ffMergeAndDeploy branchPre
)

(*
    做 合并 git pull origin pre --ff-only
    deploy
    tag & push  git tag -a xxx-to-master -m "deploy xxx to online" & git push --follow-tags
*)
Target "Online" (fun _ ->
    let branchMaster = "master"
    ensureOnBranch branchMaster
    ffMergeAndDeploy branchMaster
)


RunTargetOrDefault "BuildSolution"
