$ErrorActionPreference = 'Stop'
$root = 'E:\bbplusbot\MilkItem'
$files = Get-ChildItem -Path $root -Recurse -Filter *.cs | Where-Object {
    $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -ne 'TrimLogs.ps1'
}
$marker = [regex]'(?:Plugin\.)?Log\?\.Log(?:Info|Warning|Error|Debug|Fatal)\('

foreach ($f in $files) {
    $raw = [System.IO.File]::ReadAllText($f.FullName)   # 保留原编码/BOM
    $preCount = $marker.Matches($raw).Count
    if ($preCount -eq 0) { continue }

    $content = $raw
    $removed = 0
    $len = $content.Length
    for (;;) {
        $m = $marker.Match($content)
        if (-not $m.Success) { break }
        $start = $m.Index
        $afterParen = $start + $m.Length               # 紧贴 '(' 之后

        # 从 '(' 内开始，括号深度扫描（感知字符串），直到配对 ')' 的下一字符
        $depth = 1
        $inStr = $null
        $k = $afterParen
        while ($k -lt $len -and $depth -gt 0) {
            $c = $content[$k]
            if ($null -ne $inStr) {
                if ($c -eq '\') { $k += 2; continue }
                if ($c -eq $inStr) { $inStr = $null }
            } else {
                if ($c -eq '"' -or $c -eq "'") { $inStr = $c }
                elseif ($c -eq '(') { $depth++ }
                elseif ($c -eq ')') { $depth-- }
            }
            $k++
        }
        if ($depth -ne 0 -or $k -gt $len) {
            # 括号无法闭合（很可能命中注释里的字样）——跳过本次调用继续往后扫
            $content = $content.Substring(0, $m.Index) + $content.Substring($k)
            $len = $content.Length
            continue
        }
        # $k 指向配对 ')' 之后；跳过空白找本语句的分号 ';'
        $semi = $k
        while ($semi -lt $len -and ([char]::IsWhiteSpace($content[$semi]))) { $semi++ }
        if ($semi -ge $len -or $content[$semi] -ne ';') {
            # 日志语句后无紧邻分号（罕见）——保守跳过本次
            $content = $content.Substring(0, $m.Index) + $content.Substring($k)
            $len = $content.Length
            continue
        }
        # 删除 [start, semi+1)（含日志自己的分号）：内联 `{ Log?; return; }` 会变成 `{ return; }`
        $content = $content.Substring(0, $start) + $content.Substring($semi + 1)
        $len = $content.Length
        $removed++
    }

    if ($removed -eq 0) { continue }
    [System.IO.File]::WriteAllText($f.FullName + '.logbak', $raw)
    [System.IO.File]::WriteAllText($f.FullName, $content)
    Write-Output ("{0}: removed {1} log calls" -f $f.Name, $removed)
}
Write-Output "DONE"