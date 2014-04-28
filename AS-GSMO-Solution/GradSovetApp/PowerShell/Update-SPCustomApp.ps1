param
(
    [string]$Web = $(throw '- Need a SharePoint web site URL (e.g. "http://mysite/")'),
	[string]$Source = "ObjectModel"
)

Write-Host -ForegroundColor White "------------------"
Write-Host -ForegroundColor White "|  App Upgrader  |"
Write-Host -ForegroundColor White "------------------"
Write-Host -ForegroundColor White "- "

#Global vars
$AppPackageName = "GradSovetApp.app";
$AppName = "Система ГрадСовета"

#Loads powershell settings
Write-Host -ForegroundColor White "- Load Powershell context.."
$0 = $myInvocation.MyCommand.Definition
$dp0 = [System.IO.Path]::GetDirectoryName($0)

#Loads the SharePoint snapin
Write-Host -ForegroundColor White "- Load SharePoint context.."
$ver = $host | select version
if ($ver.Version.Major -gt 1) {$host.Runspace.ThreadOptions = "ReuseThread"} 
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) {
    Add-PSSnapin "Microsoft.SharePoint.PowerShell";
}
[void][System.Reflection.Assembly]::Load("Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c") 

#Imports the App package
Write-Host -ForegroundColor White "- Import app package '$AppPackageName'..."
$appPath = $dp0 + "\" + $AppPackageName;
if ($Source.Equals("ObjectModel", [System.StringComparison]::InvariantCultureIgnoreCase)) {
	$sourceApp = ([microsoft.sharepoint.administration.spappsource]::ObjectModel);
}
elseif ($Source.Equals("Marketplace", [System.StringComparison]::InvariantCultureIgnoreCase)) {
	$sourceApp = ([microsoft.sharepoint.administration.spappsource]::Marketplace);
}
elseif ($Source.Equals("CorporateCatalog", [System.StringComparison]::InvariantCultureIgnoreCase)) {
	$sourceApp = ([microsoft.sharepoint.administration.spappsource]::CorporateCatalog);
}
elseif ($Source.Equals("DeveloperSite", [System.StringComparison]::InvariantCultureIgnoreCase)) {
	$sourceApp = ([microsoft.sharepoint.administration.spappsource]::DeveloperSite);
}
elseif ($Source.Equals("RemoteObjectModel", [System.StringComparison]::InvariantCultureIgnoreCase)) {
	$sourceApp = ([microsoft.sharepoint.administration.spappsource]::RemoteObjectModel);
}

$spapp = Import-SPAppPackage -Path "$appPath" -Site $Web -Source $sourceApp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
if ($err -or ($spapp -eq $null)) 
{
	Write-Host -ForegroundColor Yellow "- An error occured during app import !"
	throw $err;
}
Write-Host -ForegroundColor White "- Package imported with success."

#Installs the App
Write-Host -ForegroundColor White "- Updating the APP in web site..."
$appInstance = Get-SPAppInstance -Web $Web | where-object {$_.Title -eq $AppName};
$app = Update-SPAppInstance -Identity $appInstance -App $spapp -Confirm:$false -ErrorAction SilentlyContinue -ErrorVariable err;
if ($err) {
	Write-Host -ForegroundColor Yellow "- An error occured during app update !"
	throw $err;
}
$AppName = $app.Title;
Write-Host -ForegroundColor White "- App registered, please wait during update..."
$appInstance = Get-SPAppInstance -Web $Web | where-object {$_.Title -eq $AppName};
$counter = 1;
$maximum = 150;
$sleeptime = 2;
Write-Host -ForegroundColor White "- Please wait..." -NoNewline;
while (($appInstance.Status -eq ([Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Upgrading)) -and ($counter -lt $maximum))
{
	Write-Host -ForegroundColor White "." -NoNewline;
	sleep $sleeptime;
	$counter++;
	$appInstance = Get-SPAppInstance -Web $Web | where-object {$_.Title -eq $AppName}	
}
Write-Host -ForegroundColor White ".";

if ($appInstance.Status -eq [Microsoft.SharePoint.Administration.SPAppInstanceStatus]::Installed) {
	Write-Host -ForegroundColor White "- The App was successfully upgrade.";
	$appUrl = $appInstance.AppWebFullUrl;
	Write-Host -ForegroundColor White "- The App is available at '$appUrl'.";
	Write-Host -ForegroundColor White "- "
}
else {
	Write-Host -ForegroundColor Yellow "- An unknown error has occured during app update. Read SharePoint log for more information.";
}
