<#  遍历 nuget restore

gci .\source -Recurse "packages.config" |% {
	"Restoring " + $_.FullName
	.\source\.nuget\nuget.exe i $_.FullName -o .\source\packages
    
    if ($LastExitCode -ne 0) {
        exit $LastExitCode
    }
}

#>
;
# .\Uoko.FireProj\.nuget\NuGet.exe install "FAKE" -o .\Uoko.FireProj\packages -ExcludeVersion
.\Uoko.FireProj\packages\FAKE\tools\FAKE.exe build.fsx