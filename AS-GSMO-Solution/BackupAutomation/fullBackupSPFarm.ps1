$backupfolder = "\\minstroy-0\backup";
$ftpUrl = "ftp://gradsovetsogibackup:aAXQ6zmpQD@ftp.armd.ru/";
$backupRootFileName = "spbrtoc.xml";


# ����� �����
Add-PSSnapin "Microsoft.SharePoint.PowerShell"
Set-ExecutionPolicy -ExecutionPolicy "Unrestricted" -Force
Backup-SPFarm -BackupMethod Full -Directory $backupfolder

# ����������� ���������� ������ � ��������
$thelatest = Get-ChildItem -Directory $backupfolder | sort CreationTime -Descending | select -First 1;
if (!$thelatest) {
    Write-Host "��������� ����� �� ������";
    exit 1;
}

# ������������� �������� � �������
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

# ���������� ftp � ������ �����
"������ ���� $ftp ���� �� ����������"
$ftprequest = [System.Net.FtpWebRequest]::Create($ftp);
$ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile;
$ftprequest.GetResponse();

# �������� ������
"�������� $file ..."

$webClient.UploadFile($uri, $fullFileName);

"�������� ���������"

# �������� ������ � ���������� �����
if (Test-Path $fullFileName) {
    Remove-Item $fullFileName;
}

# ����������� �� ftp xml ����� � ����������� � �������
# �� ������� ������ ���������
$ftprequest = [System.Net.FtpWebRequest]::Create($ftpUrl + $backupRootFileName);
$ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile;
$ftprequest.GetResponse();
# �����������
$uri = New-Object System.Uri($ftpUrl + $backupRootFileName);
$webClient.UploadFile($uri, $backupfolder + "\" + $backupRootFileName);

"������ ����� ����� ��������"



