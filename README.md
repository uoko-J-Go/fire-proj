# 如何集成使用系统
#### 系统简介
> 该项目是基于GitLab、GitLabCI打造的一款持续集成和持续交付的系统，通过该系统可以规范我们现有的上线流程，并可以保证其正确性...

## 应用步骤
### 一、代码迁移至GitLab，建立必要分支
    使用该系统必须先将代码管理工具改为GitLab，并建立分支master（默认不用建立）、pre分支(必须建立，上线集成所用)，
    其次大家根据自己的需要，构建相应的开发分支（建议开发分支的模型为feature-branch模型）
### 二、项目中添加对应的CI文件
> 在Git项目的根目录下添加 .gitlab-ci.yml 和 build.fsx 文件，文件可完全从 fire 项目中 copy，如果需要额外定制功能，可执行扩展。 

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