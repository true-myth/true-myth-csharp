[CmdletBinding()]
Param(
    [Parameter(Position=0,Mandatory=$True)]
    $DocFxVersion,
    [switch]
    $Safe
)

$PSVersionTable | Out-String | Write-Host 

$scriptPath = Split-Path $MyInvocation.MyCommand.Path -Parent
$docfx = Resolve-Path "$scriptPath/../docfx.console.$DocFxVersion/tools/docfx.exe"

Function New-TemporaryDirectory {
    $parent = [System.IO.Path]::GetTempPath()
    $name = [System.IO.Path]::GetRandomFileName()
    New-Item -ItemType Directory -Path (Join-Path $parent $name)
}

Function Run-Docfx {
    if ($PSVersionTable.Platform -match 'Win*') {
        Write-Verbose "Executing $docfx $args"
        &$docfx @args
    } else {
        Write-Verbose "Executing mono $docfx $args"
        mono $docfx @args
    }
}

Write-Verbose "Set docfx path to $docfx"
if (-not (Test-Path $docfx)) {
    Write-Error "Failed to locate docfx.exe"
}

git config --global user.name "TrueMyth CI"
git config --global user.email '<>'

$tmpPath = New-TemporaryDirectory
$commit = git rev-parse HEAD

Write-Host "[docfx] Building documentation into $tmpPath for $commit"
Write-Host "[docfx] Generate metadata..."
$docfxJson = Resolve-Path "$scriptPath\docfx.json"
Run-DocFx metadata $docfxJson
if ($LASTEXITCODE -ne 0) {
    Write-Error "DocFx Error: exit code $LASTEXITCODE"
}

Write-Host "[docfx] Build content..."
Run-DocFx build $docfxJson -o $tmpPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "DocFx Error: exit code $LASTEXITCODE"
}

if ($Safe) {
    Write-Host "[ps] safe mode; exiting."
    exit
}

Write-Host "[git] Checking out gh-pages ..."
git checkout gh-pages
if ($LASTEXITCODE -ne 0) {
    Write-Error "Git Error: LASTEXITCODE=$LASTEXITCODE"
}

Write-Host "[git] hard reset ..."
git reset --hard
if ($LASTEXITCODE -ne 0) {
    Write-Error "Git Error"
}

Write-Host "[git] clean ..."
git clean -df
if ($LASTEXITCODE -ne 0) {
    Write-Error "Git Error"
}

Write-Host "[ps] Copying documentation ..."
Copy-Item -rec -force $tmpPath\_site\* .

Write-Host "[git] commit updates ..."
git add .
git commit -m "CI Update on gh-pages for $commit"

Write-Host "[git] pushing back to origin"
git push

Write-Host "Done."
