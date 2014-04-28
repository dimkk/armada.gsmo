param
(
    [string]$Web = $(throw '- Need a SharePoint web site URL (e.g. "http://mysite/")'),
	[string]$AppWeb = $(throw '- Need a SharePoint app web URL')
)

Write-Host -ForegroundColor White "-------------------------"
Write-Host -ForegroundColor White "|  App groups creating  |"
Write-Host -ForegroundColor White "-------------------------"
Write-Host -ForegroundColor White "- "

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

$groups = @(
    @("ГрадСовет - Докладчики",		"Reader"), 
    @("ГрадСовет - Читатели",		"Reader"), 
    @("ГрадСовет - Администраторы", "Administrator"), 
    @("ГрадСовет - Редакторы",		"Contributor"));

$appWebLists = @(
    @("Вложения вопроса повестки",		"ГрадСовет - Докладчики", "Contributor"), 
    @("Вложения отчета по поручению",	"ГрадСовет - Докладчики", "Contributor"), 
    @("Вопросы повестки заседания",		"ГрадСовет - Докладчики", "Contributor"), 
    @("Записи журналов поручений",		"ГрадСовет - Докладчики", "Contributor"),
    @("Отчеты по поручениям",			"ГрадСовет - Докладчики", "Contributor"));


#create groups if not exists
Write-Host -ForegroundColor White "- ";
$website = Get-SPWeb $Web;
foreach($groupName in $groups) 
{
    try 
    {
        $groupExists = $website.SiteGroups.GetByName($groupName[0]);
    }
    catch [Exception] 
    {
        $website.SiteGroups.Add($groupName[0], $webSite.SiteUsers["i:0#.w|rkis\infoworker"], $webSite.SiteUsers["i:0#.w|rkis\infoworker"], "Автоматически созданная группа для приложения ГрадСовета");

        $group = $website.SiteGroups[$groupName[0]];
        $roleAssignment = New-Object Microsoft.Sharepoint.SPRoleAssignment($group);
        $roleDefinition = $website.RoleDefinitions.GetByType($groupName[1]);
        $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
        $website.RoleAssignments.Add($roleAssignment);
        $website.Update();

        Write-Host -ForegroundColor White "Группа " $groupName[0] " создана";    
        continue;
    }

    Write-Host -ForegroundColor White "Группа " $groupName[0] " уже существует";    
}

#break inheritance where needed in appWeb
Write-Host -ForegroundColor White "- ";
$appWebSite = Get-SPWeb $AppWeb;
foreach($listName in $appWebLists) 
{
    $list = $appWebSite.Lists[$listName[0]];
    if($list -eq $null) 
    {
        Write-Host -ForegroundColor White "Список " $listName[0] " не найден";    
        continue;
    }

    $list.BreakRoleInheritance($true);
    $roleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($appWebSite.SiteGroups[$listname[1]]);
    $roleDefinition = $appWebSite.RoleDefinitions.GetByType($listname[2]);
    $roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
    $list.RoleAssignments.Add($roleAssignment);
    $list.Update();

    Write-Host -ForegroundColor White "Для списка " $listName[0] " установлены права " $listName[2] " группе " $listName[1];    
}
