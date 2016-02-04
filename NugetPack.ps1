[string[]] $buildFiles = 
  '.\src\Buddy.Database\Buddy.Database40.csproj',
  '.\src\Buddy.Database\Buddy.Database45.csproj',
  '.\src\Buddy.Database\Buddy.Database451csproj',
  '.\src\Buddy.Database\Buddy.Database452.csproj',
  '.\src\Buddy.Database.PostgreSql\Buddy.Database.PostgreSql40.csproj',
  '.\src\Buddy.Database.PostgreSql\Buddy.Database.PostgreSql45.csproj',
  '.\src\Buddy.Database.PostgreSql\Buddy.Database.PostgreSql451.csproj',
  '.\src\Buddy.Database.PostgreSql\Buddy.Database.PostgreSql452.csproj'
[string[]] $nuspecFiles = 
  '.\nuspec\Buddy.Database.nuspec',
  '.\nuspec\Buddy.Database.PostgreSql.nuspec'
  
$versionFile = '.\src\SharedAssemblyInfo.cs'

$buildConfiguration = 'Release'
$outputPath = Join-Path $HOME "Dropbox\Packages"

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
