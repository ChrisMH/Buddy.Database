[string[]] $buildFiles = 
  '.\src\Buddy.Database40\Buddy.Database40.csproj',
  '.\src\Buddy.Database45\Buddy.Database45.csproj',
  '.\src\Buddy.Database451\Buddy.Database451.csproj',
  '.\src\Buddy.Database452\Buddy.Database452.csproj',
  '.\src\Buddy.Database.PostgreSql40\Buddy.Database.PostgreSql40.csproj',
  '.\src\Buddy.Database.PostgreSql45\Buddy.Database.PostgreSql45.csproj',
  '.\src\Buddy.Database.PostgreSql451\Buddy.Database.PostgreSql451.csproj',
  '.\src\Buddy.Database.PostgreSql452\Buddy.Database.PostgreSql452.csproj'
  
$versionFile = '.\src\SharedAssemblyInfo.cs'

$outputPath = Join-Path $HOME "Dropbox\Packages"

Import-Module BuildUtilities

$version = Get-Version (Resolve-Path $versionFile)
  
New-Path $outputPath

foreach($buildFile in $buildFiles)
{
  Invoke-Build (Resolve-Path $buildFile) 'Debug'
  Invoke-Build (Resolve-Path $buildFile) 'Release'
}


New-Package (Resolve-Path '.\nuspec\Buddy.Database.nuspec') $version $outputPath
New-Package (Resolve-Path '.\nuspec\Buddy.Database.Debug.nuspec') "$version-debug" $outputPath
New-Package (Resolve-Path '.\nuspec\Buddy.Database.PostgreSql.nuspec') $version $outputPath
New-Package (Resolve-Path '.\nuspec\Buddy.Database.PostgreSql.Debug.nuspec') "$version-debug" $outputPath

Remove-Module BuildUtilities
