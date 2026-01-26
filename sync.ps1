$gstatus = git status --porcelain

if ($gstatus.Length -ne 0) {
    git add --all
    git commit -m "Automated snaptop: $gstatus"
    git pull --rebase
    git push
}
