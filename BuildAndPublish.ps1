#
# Flags
#
# -Build 
#   Build the projects
#
# -Pack 
#   Package the projects 
#
# -Publish
#   Publish the packages to the repository
#
# No flags specified is equivalent to calling with all flags (-Build -Pack -Publish)
#
if($args.Length -eq 0)
{
    $args += "-Build"
    $args += "-Pack"
    $args += "-Publish"
}
$ErrorActionPreference = "Stop"

$solution = "Buddy.Database.sln"         #relative to $srcRoot

[string[]] $projects =          #relative to $srcRoot
  "Buddy.Database40\Buddy.Database40.csproj",
  "Buddy.Database45\Buddy.Database45.csproj",
  "Buddy.Database451\Buddy.Database451.csproj",
  "Buddy.Database452\Buddy.Database452.csproj",
  "Buddy.Database46\Buddy.Database46.csproj",
  "Buddy.Database461\Buddy.Database461.csproj",
  "Buddy.Database.PostgreSql40\Buddy.Database.PostgreSql40.csproj",
  "Buddy.Database.PostgreSql45\Buddy.Database.PostgreSql45.csproj",
  "Buddy.Database.PostgreSql451\Buddy.Database.PostgreSql451.csproj",
  "Buddy.Database.PostgreSql452\Buddy.Database.PostgreSql452.csproj",
  "Buddy.Database.PostgreSql46\Buddy.Database.PostgreSql46.csproj",
  "Buddy.Database.PostgreSql461\Buddy.Database.PostgreSql461.csproj"

[string[]] $packages = 
  "Buddy.Database",
  "Buddy.Database.PostgreSql"

$srcRoot = ".\src"                      # relative to script directory
$outputPath = ".\out"                   # relative to script directory
$nuspecPath = ".\nuspec"                # relative to script directory
$versionFile = 'SharedAssemblyInfo.cs'  # relative to $srcRoot

$repository = "http://nuget.hogancode.com/Hogan/nuget"
$apiKey = "Chris051010"


$buildCmd = "$env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$nugetCmd = "${Env:ProgramFiles(x86)}\nuget\nuget.exe"


# Create the output path if it does not exist
if((Test-Path $outputPath) -eq $False) { New-Item -Path $outputPath -ItemType directory | Out-Null }

# Get version information from AssemblyInformationalVersion key in the assembly info file
$matchPattern = "^\[\s*assembly:\s*AssemblyInformationalVersion\s*\(\s*\""(?<version>.*)\""\s*\)\s*\]"
Get-Content (Join-Path $srcRoot $versionFile -Resolve) | Where-Object { $_ -match $matchPattern } | out-null
$version = $matches.version

# Build all projects
if($args -contains "-Build")
{
    $solutionFile = Join-Path $srcRoot $solution
    if((Test-Path $solutionFile) -eq $False) { throw "Could not find solution file for '$solution'" }
    
    #restore nuget packages before building
    Write-Host "`nRestoring packages for $solutionFile...`n" -ForegroundColor Green
    &$nugetCmd restore $solutionFile

    Write-Host "`nBuilding all projects...`n" -ForegroundColor Green

    foreach($project in $projects)
    {
        $projectFile = Join-Path $srcRoot $project
        if((Test-Path $projectFile) -eq $False) { throw "Could not find project file for '$project'" }

        Write-Host "`nBuilding $project from $projectFile`n" -ForegroundColor Green

        &$buildCmd $projectFile /p:Configuration=Release /t:clean 
        if($LastExitCode) {throw "Build Failed"}
        &$buildCmd $projectFile /p:Configuration=Release /t:rebuild 
        if($LastExitCode) {throw "Build Failed"}
    }
}


# Package all projects
if($args -contains "-Pack")
{   
    Write-Host "`nPackaging to $outputPath...`n" -ForegroundColor Green

    foreach($package in $packages)
    {
        $nuspecFile = Join-Path $nuspecPath "$package.nuspec"
        if((Test-Path $nuspecFile) -eq $False) { throw "Could not find nuspec file for '$package'" }
        
        Write-Host "`nPackaging $nuspecFile`n" -ForegroundColor Green
        
        &$nugetCmd pack $nuspecFile -Version $version -OutputDirectory $outputPath
    }
}


# Publish to repository
if($args -contains "-Publish")
{
    Write-Host "`nPublishing to $repository...`n" -ForegroundColor Green
    
    &$nugetCmd setApiKey $apiKey -Source $repository

    foreach($package in $packages)
    {
        $packageFile = Join-Path $outputPath "$package.$version.nupkg"
        if((Test-Path $packageFile) -eq $False) { throw "Could not find package file for '$package'" }

        Write-Host "`nPublishing $packageFile...`n" -ForegroundColor Green

        &$nugetCmd push $packageFile -Source $repository
    }
}


