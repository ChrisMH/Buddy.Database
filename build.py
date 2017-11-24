"""Project Build Tools

Usage:
    build.py [-c] [-b] [-p] [-s]
    build.py (-h | --help)
    build.py (-v | --version)

Options:
    -h --help       Show this screen.
    -v --version    Show version.
    -c              Clean output files.
    -b              Build the projects.
    -p              Package the projects.
    -s              Push the packages to the repository.

Examples:
    py build.py -bps    Build, package, and push.
    py build.py -c      Clean outputs.
"""
if __name__ != "__main__":
    exit()

srcRoot = r"."
outputPath = r"out"
packagesPath = r"packages"
versionFile = r"SharedAssemblyInfo.cs"
repository = "https://www.nuget.org/api/v2/package"

solutionFile = r"Buddy.Database.sln"

projectFiles = [
    r"Buddy.Database45\Buddy.Database45.csproj",
    r"Buddy.Database451\Buddy.Database451.csproj",
    r"Buddy.Database452\Buddy.Database452.csproj",
    r"Buddy.Database46\Buddy.Database46.csproj",
    r"Buddy.Database461\Buddy.Database461.csproj",
    r"Buddy.Database.PostgreSql45\Buddy.Database.PostgreSql45.csproj",
    r"Buddy.Database.PostgreSql451\Buddy.Database.PostgreSql451.csproj",
    r"Buddy.Database.PostgreSql452\Buddy.Database.PostgreSql452.csproj",
    r"Buddy.Database.PostgreSql46\Buddy.Database.PostgreSql46.csproj",
    r"Buddy.Database.PostgreSql461\Buddy.Database.PostgreSql461.csproj"
]

testProjectFiles = [
    r"Buddy.Database45.Test\Buddy.Database45.Test.csproj",
    r"Buddy.Database.PostgreSql45.Test\Buddy.Database.PostgreSql45.Test.csproj"
]

packageFiles = [
    r"nuspec\Buddy.Database.nuspec",
    r"nuspec\Buddy.Database.PostgreSql.nuspec"
]


import os
import re
import shutil
import subprocess
from docopt import docopt


buildCmd = os.path.join(os.environ["WINDIR"], r"Microsoft.NET\Framework\v4.0.30319\MSBuild.exe")
nugetCmd = os.path.join(os.environ["PROGRAMFILES(X86)"], r"nuget\nuget.exe")
version = ""

def clean():
    """Cleans up all build and pack outputs"""

    print("Cleaning outputs...")

    for projectFile in projectFiles + testProjectFiles:
        projectPath = os.path.dirname(projectFile)

        path = os.path.join(projectPath, "bin")
        if os.path.exists(path):            
            shutil.rmtree(path)
            print("  Removed '{}'".format(path))

        path = os.path.join(projectPath, "obj")
        if os.path.exists(path):
            shutil.rmtree(path)
            print("  Removed '{}'".format(path))

    if os.path.exists(outputPath):
        shutil.rmtree(outputPath)
        print("  Removed '{}'".format(outputPath))

    print("  Done cleaning outputs.\n")


def build():
    """Builds all projects"""

    print("Restoring packages for {}...".format(solutionFile))
    sp = subprocess.Popen([nugetCmd, "restore", solutionFile], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    output = sp.communicate()[0]
    for line in [line.decode("utf-8").strip() for line in output.splitlines()]:
        print("  ", line)    
    print("  Done restoring packages for {}.\n".format(solutionFile))
    
    for projectFile in projectFiles:
        print("Building '{}'...".format(projectFile))
                
        sp = subprocess.Popen([buildCmd, "/p:Configuration=Release", "/t:rebuild"], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        output = sp.communicate()[0]
        buildFailed = False
        for line in [line.decode("utf-8").strip() for line in output.splitlines()]:
            print("  ", line)
            if "Build FAILED." in line:
                buildFailed = True
        
        if buildFailed:            
            raise Exception("Build failed for '{}'".format(projectFile))

        print("  Done building '{}'.\n".format(projectFile))

def pack():
    """Packs output files for nuget"""
    
    print("Packing to '{}'...".format(outputPath))

    if os.path.exists(outputPath):
        shutil.rmtree(outputPath)
    
    os.makedirs(outputPath)

    for packageFile in packageFiles:
        sp = subprocess.Popen([nugetCmd, "pack", packageFile, "-Version", version, "-OutputDirectory", outputPath], 
                              stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        output = sp.communicate()[0]
        for line in [line.decode("utf-8").strip() for line in output.splitlines()]:
            print("  ", line)

    print("  Done packing to '{}'.\n".format(outputPath))


def push():
    """Pushes packaged files to a repository"""

    print("Pushing to '{}'...".format(repository))    
    for packedFile in [os.path.join(outputPath, packedFile) for packedFile in os.listdir(outputPath) if os.path.isfile(os.path.join(outputPath, packedFile))]:
        sp = subprocess.Popen([nugetCmd, "push", packedFile, "-Source", repository], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        output = sp.communicate()[0]
        for line in [line.decode("utf-8").strip() for line in output.splitlines()]:
            print("  ", line)    
    print("  Done pushing to '{}'.\n".format(repository))

try:

    arguments = docopt(__doc__, version="Project Build Tools 1.0")


    srcRoot = os.path.abspath(srcRoot)
    outputPath = os.path.abspath(os.path.join(srcRoot, outputPath))
    packagesPath = os.path.abspath(os.path.join(srcRoot, packagesPath))
    versionFile = os.path.abspath(os.path.join(srcRoot, versionFile))

    solutionFile = os.path.abspath(os.path.join(srcRoot, solutionFile))

    projectFiles = [os.path.abspath(os.path.join(srcRoot, projectFile)) for projectFile in projectFiles]
    testProjectFiles = [os.path.abspath(os.path.join(srcRoot, projectFile)) for projectFile in testProjectFiles]

    packageFiles = [os.path.abspath(os.path.join(srcRoot, packageFile)) for packageFile in packageFiles]

    if not os.path.exists(versionFile):
        raise Exception("Version file not found: '{}'".format(versionFile))

    if not os.path.exists(solutionFile):
        raise Exception("Solution file not found: '{}'".format(solutionFile))
    
    for projectFile in projectFiles:
        if not os.path.exists(projectFile):
            raise Exception("Project file not found: '{}'".format(projectFile))

    for testProjectFile in testProjectFiles:
        if not os.path.exists(testProjectFile):
            raise Exception("Test project file not found: '{}'".format(testProjectFile))

    for packageFile in packageFiles:
        if not os.path.exists(packageFile):
            raise Exception("Package file not found: '{}'".format(packageFile))

    if not os.path.exists(nugetCmd):
        raise Exception("Nuget command not found: '{}'".format(nugetCmd))

    if not os.path.exists(buildCmd):
        raise Exception("Build command not found: '{}'".format(buildCmd))
    
    regexVersion = re.compile(r"\[\s*assembly:\s*AssemblyInformationalVersion\s*\(\s*\"(?P<version>[\d\.]+)\s*\"\s*\)\s*\]")
    versionFileContents = open(versionFile, "r").read()
    match = regexVersion.search(open(versionFile, "r").read())
    if not match:
        raise Exception("Version not found in version file '{}'".format(versionFile))
    version = match.group("version")

    if arguments["-c"]:
        clean()

    if arguments["-b"]:
        build()

    if arguments["-p"]:
        pack()

    if arguments["-s"]:
        push()

except Exception as ex:
    print("\nERROR: {}".format(ex))
