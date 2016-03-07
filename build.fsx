// include Fake lib
//#r @"D:/fake/tools/FakeLib.dll"
#I @"D:/fake/tools/"
#r @"FakeLib.dll"

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



let pkgProject pkgDir =
    let useConfig = getBuildParamEnsure "useConfig"
    
    let setParams defaults =
        {
            defaults with
                Verbosity = Some(Quiet)
                Targets = ["Build"]
                Properties =
                    [
                        "DeployOnBuild", "True"
                        "Configuration", useConfig
                        "PackageLocation", pkgDir
                    ]
        }
    
    (getBuildParamEnsure "csProjFile") |> build setParams

    
let deploy() =
    RestoreMSSolutionPackages (fun p -> p) slnFile

    let pkgDir = getBuildParamEnsure "pkgDir"

    pkgProject pkgDir

    let deployUser = getBuildParamEnsure "deployUser" // 系统自身配置
    let deployPwd = getBuildParamEnsure "deployPwd"   // 系统自身配置
    
    let msDeployUrl = getBuildParamEnsure "msDeployUrl"
    let iisSiteName = getBuildParamEnsure "iisSiteName"

    let exitCode = ExecProcess (fun info ->
                    info.FileName <- pkgDir + "\Uoko.FireProj.WebSite.deploy.cmd"
                    info.Arguments <- sprintf "/Y /U:%s /P:%s /A:Basic -allowUntrusted \"-setParam:name='IIS Web Application Name',value='%s'\" \"/M:%s?site=%s" 
                     deployUser deployPwd iisSiteName msDeployUrl iisSiteName) (TimeSpan.FromMinutes 1.0)
    if exitCode <> 0 then failwithf "deploy cmd failed with a non-zero exit code %d."  exitCode


let ensureOnBranch branchNeeded =
    gitCommand null (sprintf "checkout %s"  branchNeeded)
    let branchName = getBranchName null
    if branchName <> branchNeeded then failwithf "you need do this only on [%s] branch,but now you are on [%s]" branchNeeded branchName
        
let ffMergeAndDeploy onBranch =
    let mergeFromBranch = getBuildParamEnsure "mergeFromBranch"

    merge null "--ff-only" ("origin/" + mergeFromBranch)

    if onBranch = "master" then
        let onlineDate = System.DateTime.Today.Date.ToString("yyyy-MM-dd")
        let tagName = getBuildParamEnsure "onlineTagName"
        gitCommand null (sprintf "tag -a v-%s-%s -m \"deploy %s to %s\"" tagName onlineDate mergeFromBranch onBranch)
                
    deploy()

    let useRunnerAccountUrl = System.Text.RegularExpressions.Regex.Replace(environVar "CI_BUILD_REPO", @".*(@.+?)(:\d+)?/(.*)", "git$1:$3"); 

    gitCommand null (sprintf "remote set-url --push origin %s" useRunnerAccountUrl)

    gitCommand null "push --follow-tags"


Target "Deploy-To-PRE" (fun _ ->
    let branchPre = "pre"
    ensureOnBranch branchPre
        
    // 保证 pre 和 master 永远保持最新,即：在上pre这个过程里面，没有人越过上线。否则需要人为合并这部分数据过来 pre 上。
    merge null "--ff-only" "origin/master"
    
    ffMergeAndDeploy branchPre
)


Target "Online" (fun _ ->
    let branchMaster = "master"
    ensureOnBranch branchMaster    
    ffMergeAndDeploy branchMaster
)

Target "Deploy-To-IOC" (fun _ ->
    deploy()
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
            
    RestoreMSSolutionPackages (fun p -> p) slnFile
    build setParams slnFile
)

RunTargetOrDefault "BuildSolution"
