$ErrorActionPreference = 'Stop'
$root = 'E:\bbplusbot\MilkItem'
$files = Get-ChildItem -Path $root -Recurse -Filter *.cs | Where-Object {
    $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -notmatch '^Trim'
}
$rxVar = [regex]'\b[A-Za-z_][A-Za-z0-9_]*\b'

foreach ($f in $files) {
    $content = [System.IO.File]::ReadAllText($f.FullName)
    $len = $content.Length
    $changedCount = 0
    $i = 0
    while ($i -lt $len) {
        $ci = $content.IndexOf('catch', $i, [System.StringComparison]::Ordinal)
        if ($ci -lt 0) { break }
        # 找 '(' 
        $open = $content.IndexOf('(', $ci, [System.StringComparison]::Ordinal)
        if ($open -lt 0) { break }
        # 找匹配 ')'
        $close = $open
        $depth = 0
        while ($close -lt $len) {
            $c = $content[$close]
            if ($c -eq '(') { $depth++ }
            elseif ($c -eq ')') { $depth--; if ($depth -eq 0) { break } }
            $close++
        }
        if ($close -ge $len) { break }
        $param = $content.Substring($open + 1, $close - $open - 1)
        # 参数里的最后一个标识符=异常变量名
        $m = $rxVar.Matches($param)
        if ($m.Count -eq 0) { $i = $close + 1; continue }
        $var = $m[$m.Count - 1].Value
        # 找 catch 体的 '{'
        $b = $close + 1
        while ($b -lt $len -and [char]::IsWhiteSpace($content[$b])) { $b++ }
        if ($b -ge $len -or $content[$b] -ne '{') { $i = $close + 1; continue }
        # 配对大括号（感知字符串）
        $depth = 0
        $inStr = $null
        $e = $b
        while ($e -lt $len) {
            $cc = $content[$e]
            if ($null -ne $inStr) {
                if ($cc -eq '\') { $e += 2; continue }
                if ($cc -eq $inStr) { $inStr = $null }
            } else {
                if ($cc -eq '"' -or $cc -eq "'") { $inStr = $cc }
                elseif ($cc -eq '{') { $depth++ }
                elseif ($cc -eq '}') { $depth--; if ($depth -eq 0) { break } }
            }
            $e++
        }
        if ($e -ge $len) { $i = $close + 1; continue }
        $block = $content.Substring($b + 1, $e - $b - 1)
        $uses = $rxVar.Matches($block) | Where-Object { $_.Value -eq $var } | Measure-Object
        if ($uses.Count -eq 0) {
            # 未引用：去掉参数里的变量名（保留类型）。直接移除该变量名 token。
            $nameIdx = $param.LastIndexOf($var, [System.StringComparison]::Ordinal)
            $newParam = $param.Substring(0, $nameIdx).TrimEnd() + $param.Substring($nameIdx + $var.Length)
            # 清理可能产生的多余空白/ "," 默认构造；若变空则给注释占位
            if ([string]::IsNullOrWhiteSpace($newParam)) { $newParam = 'System.Exception' }
            $content = $content.Substring(0, $open + 1) + $newParam + $content.Substring($close)
            $len = $content.Length
            $changedCount++
            $i = $open + 1
        } else {
            $i = $e + 1
        }
    }
    if ($changedCount -gt 0) {
        [System.IO.File]::WriteAllText($f.FullName + '.warnbak', [System.IO.File]::ReadAllText($f.FullName))
        [System.IO.File]::WriteAllText($f.FullName, $content)
        Write-Output ("{0}: trimmed {1} unused catch vars" -f $f.Name, $changedCount)
    }
}
Write-Output "DONE"