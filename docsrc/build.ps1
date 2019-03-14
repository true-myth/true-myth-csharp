[CmdletBinding()]
Param(
    [Parameter(Position=0,Mandatory=$True)]
    $DocFxVersion,
    [switch]
    $Safe
)

Function New-TemporaryDirectory {
    $parent = [System.IO.Path]::GetTempPath()
    $name = [System.IO.Path]::GetRandomFileName()
    New-Item -ItemType Directory -Path (Join-Path $parent $name)
}

$tmpPath = New-TemporaryDirectory
$commit = git rev-parse HEAD

Write-Host "[docfx] Building documentation into $tmpPath for $commit"

Write-Host "[docfx] Generate metadata..."
&"docfx.console.$DocFxVersion/tools/docfx.exe" metadata docsrc\docfx.json
if ($LASTEXITCODE -ne 0) {
    Write-Error "DocFx Error"
}

Write-Host "[docfx] Build content..."
&"docfx.console.$DocFxVersion/tools/docfx.exe" build docsrc\docfx.json -o $tmpPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "DocFx Error"
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
#git push

Write-Host "Done."
