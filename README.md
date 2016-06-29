# 如何集成使用系统
#### 系统简介
> 该项目是基于GitLab、GitLabCI打造的一款持续集成和持续交付的系统，通过该系统可以规范我们现有的上线流程，并可以保证其正确性...

## 应用步骤
### 一、代码迁移至GitLab，建立必要分支
    使用该系统必须先将代码管理工具改为GitLab，并建立分支master（默认不用建立）、pre分支(必须建立，上线集成所用)，
    其次大家根据自己的需要，构建相应的开发分支（建议开发分支的模型为feature-branch模型）
### 二、项目中添加对应的CI文件
> 在Git项目的根目录下添加 .gitlab-ci.yml 和 build.fsx 文件，文件可完全从 fire 项目中 copy，如果需要额外定制功能，可执行扩展。 
  
  - .gitlab-ci.yml文件内容
  
<pre><code>    
    variables:
    #  slnFile: Uoko.FireProj.sln # 如果有多个则可以进行指定

    before_script:
    - 'chcp 65001'

    # 每次 package 包都放入缓存中，这样方便快速还原，这个维度是以 branch 为基础的  
    cache:
    key: "$CI_BUILD_REF_NAME"
    paths:
        - packages/
    
    stages:
    - build

    execFakeScript:
    stage: build
    script:
        - Fake.exe build.fsx
</code></pre>         

   - build.fsx文件内容   
   
<pre><code>
        // include Fake lib
//#r @"D:/fake/tools/FakeLib.dll"
#I @"D:/fake/tools/"
#r @"FakeLib.dll"

open Fake
open Fake.Git
open System
open System.IO

let msdeployPath = getBuildParamOrDefault "msdeployPath" @"C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"

let getBuildParamEnsure name =
    let value = environVar name
    if isNullOrWhiteSpace value then failwithf "environVar of %s is null or whitespace" name
    else value

let slnFile = 
    !! "./**/*.sln"
    |> Seq.toList
    |> List.head
    |> getBuildParamOrDefault "slnFile"

let nugetPkgOutputPath = (FileInfo slnFile).Directory.FullName @@ "/packages"

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
    RestoreMSSolutionPackages (fun p -> {
        p with
            OutputPath = nugetPkgOutputPath
    }) slnFile

    let pkgDir = getBuildParamEnsure "pkgDir"
    let iisSiteName = getBuildParamEnsure "iisSiteName"

    let pkgFullPath = sprintf "%s/%s.zip" pkgDir iisSiteName
    let setParametersFile = sprintf "%s/%s.SetParameters.xml" pkgDir iisSiteName
    
    pkgProject pkgFullPath

    let deployUser = getBuildParamEnsure "deployUser" // 系统自身配置
    let deployPwd = getBuildParamEnsure "deployPwd"   // 系统自身配置
    let msDeployUrl = getBuildParamEnsure "msDeployUrl"

    let msdeployArgs = sprintf @"-source:package=""%s"" -dest:auto,computerName=""%s?site=%s"",userName=""%s"",password=""%s"",authtype=""Basic"",includeAcls=""False"" -verb:sync 
    -disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension 
    -setParamFile:""%s"" -allowUntrusted -enableRule:AppOffline -setParam:name=""IIS Web Application Name"",value=""%s""" pkgFullPath msDeployUrl iisSiteName deployUser deployPwd setParametersFile iisSiteName
    
    let tracing = ProcessHelper.enableProcessTracing
    ProcessHelper.enableProcessTracing <- false
    let exitCode = ExecProcess (fun info ->
                    info.FileName <- msdeployPath
                    info.Arguments <- msdeployArgs) (TimeSpan.FromMinutes 5.0)
    if exitCode <> 0 then failwithf "deploy cmd failed with a non-zero exit code %d."  exitCode
    ProcessHelper.enableProcessTracing <- tracing


let backup onlineVersion =
    let pkgDir = getBuildParamEnsure "pkgDir"
    let iisSiteName = getBuildParamEnsure "iisSiteName"
    let deployUser = getBuildParamEnsure "deployUser" // 系统自身配置
    let deployPwd = getBuildParamEnsure "deployPwd"   // 系统自身配置
    let msDeployUrl = getBuildParamEnsure "msDeployUrl"
    
    let backupPath = sprintf "%s/backups/%s-before-%s.zip" pkgDir iisSiteName onlineVersion
    
    let sourceArg = sprintf @"-source:iisapp=""%s"",computerName=""%s?site=%s"",userName=""%s"",password=""%s"",authtype=""Basic"",includeAcls=""False""" iisSiteName msDeployUrl iisSiteName deployUser deployPwd
    let destArg = sprintf @"-dest:package=""%s"",computerName=""%s?site=%s"",userName=""%s"",password=""%s"",authtype=""Basic"",includeAcls=""False""" backupPath msDeployUrl iisSiteName deployUser deployPwd

    let backupArgs = sprintf @"-verb:sync -allowUntrusted %s %s" sourceArg destArg

    let tracing = ProcessHelper.enableProcessTracing
    ProcessHelper.enableProcessTracing <- false
    let exitCode = ExecProcess (fun info ->
                    info.FileName <- msdeployPath
                    info.Arguments <- backupArgs) (TimeSpan.FromMinutes 5.0)
                    
    if exitCode <> 0 then failwithf "backup failed with a non-zero exit code %d."  exitCode
    ProcessHelper.enableProcessTracing <- tracing


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
        gitCommand null (sprintf "tag -f -a v-%s-%s -m \"deploy %s to %s\"" tagName onlineDate mergeFromBranch onBranch)
        backup tagName
                
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
                
    RestoreMSSolutionPackages (fun p -> {
        p with
            OutputPath = nugetPkgOutputPath
    }) slnFile
    build setParams slnFile
)


Target "Rollback" (fun _ ->
    let invokeTime = getBuildParamEnsure "invokeTime" |> System.DateTime.Parse 
    let times = System.DateTime.Now - invokeTime
    if (System.DateTime.Now - invokeTime) > (System.TimeSpan.FromMinutes 1.0) then failwithf "该次操作已经失效,截止 %s,请重新发起操作" (invokeTime.AddMinutes(1.0).ToString "yyyy-MM-dd HH:mm:ss")
    
    let rollbackVersion = getBuildParamEnsure "rollbackVersion"
    let pkgDir = getBuildParamEnsure "pkgDir"
    let iisSiteName = getBuildParamEnsure "iisSiteName"
    let deployUser = getBuildParamEnsure "deployUser" // 系统自身配置
    let deployPwd = getBuildParamEnsure "deployPwd"   // 系统自身配置
    let msDeployUrl = getBuildParamEnsure "msDeployUrl"
    let backupPath = sprintf "%s/backups/%s-before-%s.zip" pkgDir iisSiteName rollbackVersion
    
    let sourceArg = sprintf @"-source:package=""%s"",computerName=""%s?site=%s"",userName=""%s"",password=""%s"",authtype=""Basic"",includeAcls=""False""" backupPath msDeployUrl iisSiteName deployUser deployPwd
    let destArg = sprintf @"-dest:iisapp=""%s"",computerName=""%s?site=%s"",userName=""%s"",password=""%s"",authtype=""Basic"",includeAcls=""False""" iisSiteName msDeployUrl iisSiteName deployUser deployPwd

    let rollbackArgs = sprintf @"-verb:sync -allowUntrusted %s %s" sourceArg destArg

    let trace = ProcessHelper.enableProcessTracing
    ProcessHelper.enableProcessTracing <- false
    let exitCode = ExecProcess (fun info ->
                    info.FileName <- msdeployPath
                    info.Arguments <- rollbackArgs) (TimeSpan.FromMinutes 5.0)
    ProcessHelper.enableProcessTracing <- trace
    if exitCode <> 0 then failwithf "rollback cmd failed with a non-zero exit code %d."  exitCode
)


Target "test" (fun _ ->
    printfn "before test------------>%b" ProcessHelper.enableProcessTracing
    printfn "after test------------>%b" ProcessHelper.enableProcessTracing
)


RunTargetOrDefault "BuildSolution"

</code></pre>      


### GitLab 配置相关信息（此操作只有项目Owner才能配置）

#### 配置WebHook(Settings=>Web Hooks)
 
 1. 在 URL中 添加 地址  http://fire.uoko.com/api/WebHookApi/BuildCallback  
 2. 在Trigger中只选择 Build events
 3. SSL verification 默认选中
 4. 点击 按钮 Add Web Hook
 
#### 配置Runner(Settings=>Runners)

 此项目暂时不用配置 采用 共享的Runner

#### 配置Triggers(Settings=>Triggers)
 
 检查Triggers下面是否已经含有一条Trigger记录  如果没有 就直接点击 按钮 Add Trigger 会自动生成一条记录
 
## asp.net Web站点多环境配置问题

> 解决方案: [Configuration Transform](https://visualstudiogallery.msdn.microsoft.com/579d3a78-3bdd-497c-bc21-aa6e6abbc859)   

### 如何使用Web.Config文件的Transformations 
   [中文地址](http://www.cnblogs.com/TomXu/archive/2011/11/25/2263089.html)      
   [英文地址](http://www.asp.net/mvc/overview/older-versions/deployment-to-a-hosting-provider/deployment-to-a-hosting-provider-web-config-file-transformations-3-of-12) 
   
    特别说明  
      
    我们统一约定 使用域名来建立 配置转换文件 比如你要发布的站点域名是 ids.uoko.ioc 你就在站点下建立一个Web.ids.uoko.ioc.Config 文件，然后在这个文件中配置相应的转换信息