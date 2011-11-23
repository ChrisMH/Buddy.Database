$srcRoot = '.\src'                     # relative to script directory
$versionFile = 'SharedAssemblyInfo.cs' # relative to $srcRoot
$outputPath = "$home\Dropbox\Packages"

Import-Module NugetUtilities

New-Path $outputPath

$version = Get-Version (Join-Path $srcRoot $versionFile)

Pack-Project Utility.Database $srcRoot $version $outputPath
