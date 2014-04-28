#
# Script.ps1
#

function PopulateList($SiteUrl, $ListTitle, [int]$NumberOfItems, $fields)
{
$web = Get-SPWeb $SiteUrl
$list = $web.Lists[$ListTitle]
Write-Host "---Retrieved list" $list.Title

for ($i=1; $i -le $NumberOfItems; $i++)
{
Write-Host "-Adding item # " $i
$newItem = $list.Items.Add()
foreach ($field in $fields)
{
Write-Host "--Updating" $field.Title " for item # " $i
if($field.Type -eq "Text")
{
$newItem[$field.Title] = "lorem ipsum " * (Get-Random -Minimum 1 -Maximum 8)
}
if($field.Type -eq "Note") {$newItem[$field.Title] = "lorem ipsum " * (Get-Random -Minimum 1 -Maximum 100)}
}
if($field.Type -eq "DateTime")
{
$newItem[$field.Title] = (Get-Date).AddDays((Get-Random -Minimum -1000 -Maximum 1000))
}
if($field.Type -eq "Number")
{
$newItem[$field.Title] = (Get-Random -Minimum 1 -Maximum 200)
}
if($field.Type -eq "Boolean")
{
$newItem[$field.Title] = $true
}
$newItem.Update();
}
} 

function InitApplication() { Write-Host "Initialize Script" "-----------------" 
# check to ensure Microsoft.SharePoint.PowerShell is loaded 
$snapin = Get-PSSnapin | Where-Object Name -eq 'Microsoft.SharePoint.Powershell'
if ($snapin -eq $null) {
Write-Host "Loading SharePoint Powershell Snapin"
Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
}

try
{
[xml]$SiteStructure = Get-Content "PopulateList.xml"
}
catch
{
Write-Host "Failed to load site structure from file, make sure file exists."

return
}

$SiteUrl = $SiteStructure.Setup.SiteUrl -replace "/$", ""
$ListTitle = $SiteStructure.Setup.ListTitle -replace "/$", ""
$NumberOfItems = $SiteStructure.Setup.NumberOfItems -replace "/$", ""
$fieldsList = $SiteStructure.SelectNodes("/Setup/Fields/Field")

# initialize applicaiton, load relevant snap-ins and assemblies.
InitApplication

# deploy solutions
PopulateList $SiteUrl $ListTitle $NumberOfItems $fieldsList