$ErrorActionPreference = 'Stop'

$assemblyPath = Join-Path $PSScriptRoot '..\DataLayer\bin\Debug\DataLayer.dll'
if (-not (Test-Path -LiteralPath $assemblyPath)) {
    throw 'Build the Debug solution before running this check.'
}

[void][System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $assemblyPath).Path)

$password = 'sample-password-for-a-smoke-check'
$hashOne = [DataLayer.PasswordHash]::Create($password)
$hashTwo = [DataLayer.PasswordHash]::Create($password)

if (-not [DataLayer.PasswordHash]::Verify($password, $hashOne)) {
    throw 'The correct password was rejected.'
}
if ([DataLayer.PasswordHash]::Verify('wrong-password', $hashOne)) {
    throw 'An incorrect password was accepted.'
}
if ($hashOne -eq $hashTwo) {
    throw 'Separate hashes reused the same salt.'
}
if ([DataLayer.PasswordHash]::Verify($password, 'pbkdf2-sha256:bad:input:hash')) {
    throw 'A malformed hash was accepted.'
}

Write-Host 'Password hashing smoke checks passed.'
