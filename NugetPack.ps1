[string[]] $buildFiles = 
  '.\src\Utility.Database\Utility.Database.csproj',
  '.\src\Utility.Database.Mock\Utility.Database.Mock.csproj',
  '.\src\Utility.Database.PostgreSql\Utility.Database.PostgreSql.csproj'
[string[]] $nuspecFiles = 
  '.\nuspec\Utility.Database.nuspec',
  '.\nuspec\Utility.Database.Mock.nuspec',
  '.\nuspec\Utility.Database.PostgreSql.nuspec'
  
$versionFile = '.\src\SharedAssemblyInfo.cs'

$buildConfiguration = 'Release'
$outputPath = "c:\Users\chogan\Dropbox\Packages"

Import-Module BuildUtilities

$version = Get-Version (Resolve-Path $versionFile)
  
New-Path $outputPath

#foreach($buildFile in $buildFiles)
#{
#  Invoke-Build (Resolve-Path $buildFile) $buildConfiguration
#}

foreach($nuspecFile in $nuspecFiles)
{
  New-Package (Resolve-Path $nuspecFile) $version $outputPath
}

Remove-Module BuildUtilities
