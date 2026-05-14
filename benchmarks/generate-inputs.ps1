param(
    [string] $OutputDirectory = "$PSScriptRoot\data"
)

$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$sizes = @(1000, 10000, 100000, 1000000, 10000000)
$tradeTemplates = @(
    "2000000 Private 12/29/2025",
    "400000 Public 07/01/2020",
    "5000000 Public 01/02/2024",
    "3000000 Public 10/26/2023"
)

foreach ($size in $sizes) {
    $path = Join-Path $OutputDirectory "input_$size.txt"
    $writer = [System.IO.StreamWriter]::new($path, $false, [System.Text.Encoding]::UTF8)

    try {
        $writer.WriteLine("12/11/2020")
        $writer.WriteLine($size)

        for ($i = 0; $i -lt $size; $i++) {
            $writer.WriteLine($tradeTemplates[$i % $tradeTemplates.Count])
        }
    }
    finally {
        $writer.Dispose()
    }
}
