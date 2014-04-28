$backupfolder = "\\minstroy-0\backup";
$ftpUrl = "ftp://gradsovetsogibackup:aAXQ6zmpQD@ftp.armd.ru/";
$siteIdentity = "http://gs.msk.mosreg.ru";
$fileprefix = "gs_sitecollection_";

# уникальное имя файла
$date = date;
$filename = $fileprefix + $date.Day.ToString() + $date.Month.ToString() + $date.Year.ToString() + "_" + $date.Hour.ToString() + $date.Minute.ToString() + $date.Second.ToString() + ".bak";
$backupPath = $backupfolder + "\" + $filename;

# Бэкап sc
Add-PSSnapin "Microsoft.SharePoint.PowerShell"
Set-ExecutionPolicy -ExecutionPolicy "Unrestricted" -Force
Backup-SPSite -Identity $siteIdentity -Path $backupPath -Force

$ftp = $ftpUrl + $filename;
"ftp url: $ftp"

$webClient = New-Object System.Net.WebClient;
$uri = New-Object System.Uri($ftp);

# Подготовка ftp к приему файла
"Удалим файл $ftp если он существует"
$ftprequest = [System.Net.FtpWebRequest]::Create($ftp);
$ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile;
$ftprequest.GetResponse();

# Загрузка бэкапа
"Загрузка $filename ..."

$webClient.UploadFile($uri, $backupPath);

"Загрузка завершена"

# Удаление старых бэкапов, оставляем только 2 последних
$searchmask = $backupfolder + "\" + $fileprefix + "*.bak";
Get-ChildItem -File $searchmask | sort CreationTime -Descending | select -Skip 2 | foreach ($_) { Remove-Item $_.FullName };

"Полный бэкап коллекции сайтов завершен"



