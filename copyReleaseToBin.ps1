$destinationPath = "C:\Users\bryan\Documents\bin\phoneImageMover\"
$backupPath = "C:\Users\bryan\Documents\bin\phoneImageMover\$(Get-Date -Format 'yyyy_MM_dd_HH_mm')"
$artifactPath = ".\phoneImageMover\bin\Release\net6.0\"
New-Item -ItemType Directory -Path $backupPath
Move-Item "$($destinationPath)*.*" $backupPath
Copy-Item "$($artifactPath)*.exe" $destinationPath
Copy-Item "$($artifactPath)*.dll" $destinationPath
Copy-Item "$($artifactPath)*.json" $destinationPath
Copy-Item .\*.ps1 $destinationPath