$ErrorActionPreference = 'Stop'

$assemblyPath = Join-Path $PSScriptRoot '..\Revayat.Web\bin\Revayat.Web.dll'
if (-not (Test-Path -LiteralPath $assemblyPath)) {
    throw 'Build the Debug solution before running this check.'
}

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Web
[void][System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $assemblyPath).Path)

Add-Type -ReferencedAssemblies @('System.Web', 'System') -TypeDefinition @'
using System;
using System.IO;
using System.Web;

public sealed class TestPostedFile : HttpPostedFileBase
{
    private readonly byte[] content;
    private readonly string name;

    public TestPostedFile(string name, byte[] content)
    {
        this.name = name;
        this.content = content;
    }

    public override string FileName { get { return name; } }
    public override int ContentLength { get { return content.Length; } }
    public override Stream InputStream { get { return new MemoryStream(content); } }
}
'@

$tempRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
$directory = [System.IO.Path]::Combine($tempRoot, 'revayat-image-test-' + [guid]::NewGuid().ToString('N'))
try {
    $bitmap = New-Object System.Drawing.Bitmap 4, 4
    $stream = New-Object System.IO.MemoryStream
    try {
        $bitmap.Save($stream, [System.Drawing.Imaging.ImageFormat]::Png)
        $content = $stream.ToArray()
    }
    finally {
        $bitmap.Dispose()
        $stream.Dispose()
    }

    [string]$fileName = $null
    [string]$errorMessage = $null
    $badUpload = [TestPostedFile]::new('script.aspx', $content)
    if ([Revayat.Web.Classes.PageImageStorage]::TrySave($badUpload, $directory, [ref]$fileName, [ref]$errorMessage)) {
        throw 'A script extension was accepted.'
    }

    $goodUpload = [TestPostedFile]::new('picture.png', $content)
    if (-not [Revayat.Web.Classes.PageImageStorage]::TrySave($goodUpload, $directory, [ref]$fileName, [ref]$errorMessage)) {
        throw "A valid image was rejected: $errorMessage"
    }
    if (-not (Test-Path -LiteralPath (Join-Path $directory $fileName))) {
        throw 'The accepted image was not saved.'
    }

    [Revayat.Web.Classes.PageImageStorage]::DeleteIfOwned($directory, $fileName)
    if (Test-Path -LiteralPath (Join-Path $directory $fileName)) {
        throw 'The generated image was not deleted.'
    }

    Write-Host 'Image upload smoke checks passed.'
}
finally {
    $resolved = [System.IO.Path]::GetFullPath($directory)
    if ($resolved.StartsWith($tempRoot, [System.StringComparison]::OrdinalIgnoreCase) -and
        (Test-Path -LiteralPath $resolved)) {
        Remove-Item -LiteralPath $resolved -Recurse -Force
    }
}
