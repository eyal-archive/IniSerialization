# Psake file.

if (-not $PSVersionTable.PSVersion -or $PSVersionTable.PSVersion.Major -lt 3)
{
    Write-Host "To run the script you need to use PowerShell 3." -ForegroundColor Red
    Write-Host

    throw
}

Properties {
	# Solution
	$baseDir = Resolve-Path .
	$buildConfig = "Release"
	# MSBuild
	$msbuildVerbosity = "normal"
	$msbuildCpuCount = [System.Environment]::ProcessorCount / 2
	$msbuildParralel = $true
	# NuGet
	$nugetPackageDir = ".packages"
    $nugetPackagePath = "$baseDir\$nugetPackageDir"
    # Release
    $artifactsPath = "$baseDir\.build"
    $artifacts = @("Ace.Ini.dll")
}

# Tasks
#######################

Task Default -Depends Compile-And-Test, Artifacts
Task Compile -Depends Clean, Nuget, Release
Task Compile-And-Test -Depends Compile, Unit-Test

# Setup
#######################

# Deletes the output directories
Task Clean {
    function Delete([String]$Path) { del "$Path" -Force -Recurse -ErrorAction SilentlyContinue }

	Delete "$nugetPackagePath"

    Delete "$artifactsPath"

	foreach($projInfo in Get-Projects-Info)
	{
		$projDir = $projInfo.Directory;
		
		Delete "$projDir\bin"
		Delete "$projDir\obj"
	}
}

# Downloads the required packages using NuGet
Task Nuget {
	foreach($projInfo in Get-Projects-Info)
	{
		$configPath = "$($projInfo.Directory)\packages.config"
		
		if ([System.IO.File]::Exists($configPath))
		{
			& $(Get-Tool "NuGet") install $configPath -o "$nugetPackagePath"
		}
	}

    # Creates or updates the 'nuget.config' file
   	$xml = [XML]("<settings><repositoryPath>.\" + $nugetPackageDir + "</repositoryPath></settings>")
	
	$xml.Save("$baseDir\nuget.config");
}

# Copies artifacts to the build directory
Task Artifacts {
    New-Item $artifactsPath -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

    $artifactList = New-Object System.Collections.ArrayList

    $artifactList.AddRange($artifacts)

    foreach($projInfo in Get-Projects-Info)
    {
        foreach($artifact in $artifacts)
        {
            $assemblyPath = "$($projInfo.ProjectOutput)$artifact"

            if ([System.IO.File]::Exists($assemblyPath) -and $artifactList -contains $artifact)
		    {
                Write-Host "Copy file '$artifact' to '$artifactsPath'."
    
                copy $assemblyPath "$artifactsPath"
                   
                $artifactList.Remove($artifact)
            }
        }
    }

    if ($artifactList.Count -gt 0)
    {
        $notFoundArtifacts = [String]::Join(", ", $artifactList.ToArray())

        Exit-Build "Cannot find the following artifacts '$notFoundArtifacts'"
    }
}

# Prints an empty line after each task
TaskTearDown {
    Write-Host ""
}

# Compilation
#######################

# Builds all projects within the directory of the solution
Task Release {
	foreach($projInfo in Get-Projects-Info)
	{
		$projFile = $projInfo.FullName
		
		try
		{
			exec { msbuild /nologo /v:$msbuildVerbosity /m:$msbuildCpuCount /p:BuildInParralel=$msbuildParralel /p:Configuration="$buildConfig" /p:Platform="AnyCPU" /p:OutDir="$($projInfo.ProjectOutput)" "$projFile" }
		}
		catch
		{
            Exit-Build "Failed to build project '$projFile'"
		}
	}
}

# Tests
#######################

# Finds tests and execute them using xUnit
Task Unit-Test {
	$tools = @("xunit.console", "xunit.console.x86", "xunit.console.clr4", "xunit.console.clr4.x86")

    $badOutputPattern = "xUnit\.net console test runner.+?Could not load file or assembly.+?(?:cannot be loaded.|incorrect format.)"

    $failurePattern = ",\s+[1-9]\d*\s+failed,"

    $successPattern = "Test assembly:.+seconds"

	foreach($projInfo in Get-Projects-Info)
	{
        if ($projInfo.IsTestProject)
        {
            $assemblyPath = "$($projInfo.ProjectOutput + $projInfo.AssemblyName).dll"

            # Iterates over xUnit executables in attempt to find the correct version of the tool to load the assembly
		    foreach($tool in $tools)
		    {
			    $output = "$(& $(Get-Tool $tool) $assemblyPath /silent)"

			    if ($output -notmatch $badOutputPattern)
			    {
                    if ($output -match $failurePattern)
                    {
                        Exit-Build "One or more tests failed"
                    }
                    else
                    {
                        $matched = $output -match $successPattern

                        if ($matched)
                        {
                            echo $($matches[0])
                        }

                        break
                    }
			    }
		    }
        }
	}           
}

# Utility Functions
#######################

# Exits the script when an error occurs and prints a message to the user
function Exit-Build
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)][String]$Message
    )

	Write-Host $("`nExiting build because task [{0}] failed.`n->`t$Message.`n" -f $psake.context.Peek().currentTaskName) -ForegroundColor Red

    Exit
}

# Tries to find a given tool in the solution directory and get its path
function Get-Tool
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)][String]$ToolFile
    )

	$tool = (@(gci "$baseDir" -Recurse -File -Filter "$ToolFile.exe") | select -First 1)

    return $tool.FullName
}

# Gets information about projects in the current directory of the solution
function Get-Projects-Info
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $false)][String]$ProjectExt = "csproj"
    )

	$paths = New-Object System.Collections.Generic.List[PSObject]

    # Gets the project files based on a given file's extension
    $projFiles = @(gci "$baseDir" -Recurse -File -Filter "*.$ProjectExt")

	foreach($file in $projFiles)
	{
		$projDir = $file.Directory

        $fullName = $file.FullName

        $bin = "$projDir\bin\$buildConfig\"

        $projDocument = New-Object XML

		$projDocument.Load($fullName)

        # Checks that xUnit is referenced so we can determine whether it's a project that may contain tests
		$isTestProject = ($projDocument.Project.ItemGroup.Reference.Include | where { $_ -eq "xunit" }) -ne $null

        # Gets the assembly's name out of the project's file
		$assemblyName = ($projDocument.Project.PropertyGroup | where { $_.AssemblyName -ne $null }).AssemblyName

		$paths.Add(@{
			File = $file;
			Directory = $projDir;
            FullName = $fullName;
            ProjectOutput = $bin;
			ReleaseOutput = "$artifactsPath\";
            AssemblyName = $assemblyName;
            IsTestProject = $isTestProject
		})
	}
	
	return $paths;
}