$backupfolder = "\\minstroy-0\backup";
$ftpUrl = "ftp://gradsovetsogibackup:aAXQ6zmpQD@ftp.armd.ru/";
$backupRootFileName = "spbrtoc.xml";


# Бэкап фермы
Add-PSSnapin "Microsoft.SharePoint.PowerShell"
Set-ExecutionPolicy -ExecutionPolicy "Unrestricted" -Force
Backup-SPFarm -BackupMethod Full -Directory $backupfolder

# Определение последнего бэкапа в каталоге
$thelatest = Get-ChildItem -Directory $backupfolder | sort CreationTime -Descending | select -First 1;
if (!$thelatest) {
    Write-Host "Последний бэкап не найден";
    exit 1;
}

# Архивирование каталога с бэкапом
."c:\Scripts\ZipFolder.ps1"
$fullFileName = $thelatest.FullName + ".zip";
if (Test-Path $fullFileName) {
    Remove-Item ($fullFileName);
}

ZipFolder $thelatest;

$file = $thelatest.Name + ".zip";
$ftp = $ftpUrl + $file;

"ftp url: $ftp"

$webClient = New-Object System.Net.WebClient;
$uri = New-Object System.Uri($ftp);

# Подготовка ftp к приему файла
"Удалим файл $ftp если он существует"
$ftprequest = [System.Net.FtpWebRequest]::Create($ftp);
$ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile;
$ftprequest.GetResponse();

# Загрузка бэкапа
"Загрузка $file ..."

$webClient.UploadFile($uri, $fullFileName);

"Загрузка завершена"

# Удаление архива с локального диска
if (Test-Path $fullFileName) {
    Remove-Item $fullFileName;
}

# Копирование на ftp xml файла с метаданными о бэкапах
# но сначала удалим имеющийся
$ftprequest = [System.Net.FtpWebRequest]::Create($ftpUrl + $backupRootFileName);
$ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile;
$ftprequest.GetResponse();
# копирование
$uri = New-Object System.Uri($ftpUrl + $backupRootFileName);
$webClient.UploadFile($uri, $backupfolder + "\" + $backupRootFileName);

"Полный бэкап фермы завершен"



