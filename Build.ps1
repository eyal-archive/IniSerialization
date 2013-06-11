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
Task Compile -Depends Clean, Nuget, Default-Build
Task Compile-And-Test -Depends Compile, Unit-Test

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

# Copies the artifacts to the build directory
Task Artifacts {
    New-Item $artifactsPath -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

    $artifactList = New-Object System.Collections.ArrayList

    $artifactList.AddRange($artifacts)

    foreach($projInfo in Get-Projects-Info)
    {
        foreach($artifact in $artifacts)
        {
            $assemblyPath = "$($projInfo.Output)$artifact"

            if ([System.IO.File]::Exists($assemblyPath) -and $artifactList.Contains($artifact))
		    {
                Write-Host "Copy file '$artifact' to '$artifactsPath'."
    
                gci "$assemblyPath" | copy -Destination "$artifactsPath"
                   
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

TaskTearDown {
    Write-Host ""
}

# NuGet
#######################
   
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

# MSBuild
#######################

# Builds all projects within the directory of the solution
Task Default-Build {
	foreach($projInfo in Get-Projects-Info)
	{
		$projFile = $projInfo.File
		
		try
		{
			exec { msbuild /nologo /v:$msbuildVerbosity /m:$msbuildCpuCount /p:BuildInParralel=$msbuildParralel /p:Configuration="$buildConfig" /p:Platform="AnyCPU" /p:OutDir="$($projInfo.Output)" "$projFile" }
		}
		catch
		{
            Exit-Build "Failed to build project '$projFile'"
		}
	}
}

Task Debug-Build -Depends Clean, Nuget {
    foreach($projInfo in Get-Projects-Info)
	{
		$projFile = $projInfo.File
		
		try
		{
			exec { msbuild /nologo /v:$msbuildVerbosity /m:$msbuildCpuCount /p:BuildInParralel=$msbuildParralel /p:Configuration="Debug" /p:Platform="AnyCPU" /p:OutDir="$artifactsPath\" "$projFile" }
		}
		catch
		{
            Exit-Build "Failed to build project '$projFile'"
		}
	}
}

# xUnit
#######################

# Finds tests and execute them using xUnit
Task Unit-Test {
	$tools = @("xunit.console", "xunit.console.x86", "xunit.console.clr4", "xunit.console.clr4.x86")

    $outputRegex = New-Object Regex -ArgumentList @("xUnit\.net console test runner.+?Could not load file or assembly.+?(?:cannot be loaded.|incorrect format.)")

    $failureRegex = New-Object Regex -ArgumentList @(",\s+[1-9]\d*\s+failed,")

    $successRegex = New-Object Regex -ArgumentList @("Test assembly:.+seconds")

	foreach($projInfo in Get-Projects-Info)
	{
		$projDocument = New-Object XML
		
		$projDocument.Load($projInfo.File)
		
        # Gets the assembly's name out of the project's file
		$assemblyName = ($projDocument.Project.PropertyGroup | where { $_.AssemblyName -ne $null }).AssemblyName
		
		$assemblyPath = "$($projInfo.Output)$assemblyName.dll"
		
		if ([System.IO.File]::Exists($assemblyPath))
		{
            # Checks that xUnit is referenced so we can determine whether it's a project that may contain tests
			$isTestProject = ($projDocument.Project.ItemGroup.Reference.Include | where { $_ -eq "xunit" }) -ne $null

			if ($isTestProject)
			{
                # Iterates over xUnit executables and attempt to find a candidate to execute the assembly
				foreach($tool in $tools)
				{
					$output = & $(Get-Tool $tool) $assemblyPath /silent

					if (-not $outputRegex.IsMatch($output))
					{
                        if ($failureRegex.IsMatch($output))
                        {
                            Exit-Build "One or more tests failed"
                        }
                        else
                        {
                            $outputMatch = $successRegex.Match($output)

                            if ($outputMatch.Success)
                            {
                                echo $outputMatch.Value
                            }

                            break
                        }
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

    Write-Host
	Write-Host $([String]::Format("Exiting build because task [{0}] failed.", $psake.context.Peek().currentTaskName)) -ForegroundColor Red
    Write-Host "->    $Message." -ForegroundColor Red
	Write-Host

    Exit
}

# Tries to find a given tool in the solution directory and get its path
function Get-Tool
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)][String]$ToolFile
    )

	return @(gci "$baseDir" -Recurse -File -Include "$ToolFile.exe") | select -First 1
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
    $projFiles = @(gci "$baseDir" -Recurse -File -Include "*.$ProjectExt")

	foreach($file in $projFiles)
	{
		$projDir = $file.Directory
		
		$bin = "$projDir\bin\$buildConfig\"
		
		$paths.Add(@{
			File = $file;
			Directory = $projDir;
			Output = $bin
		})
	}
	
	return $paths;
}