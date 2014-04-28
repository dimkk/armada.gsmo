#Simplple WSP redeplyment script from SharePointJack.com
#http://sharepointjack.com/2013/a-simple-powershell-script-for-redeploying-a-sharepoint-wsp-solution-file/
#first we add the SP snap in so that this can be launched from "normal" powershell
Add-PSSnapin Microsoft.SharePoint.PowerShell –ErrorAction SilentlyContinue

function main {

	$path = get-location # gets the location this script is running - it is assumed you have the WSP in the same directory
	$wsp = "nameofyour.wsp" #name of the WSP

    #this switch statement lets you assign the webapp URL based on the machine it's run from - in this way you can use the same script for Dev, stage and production.
	switch($env:computername)
	{
	 "DevelopmentServerName" {$webapp = "http://dev-url.yoursite.com"}
	 "StageServerName" {$webapp = "https://stg-url.yoursite.com"}
	 "ProdServerName" {$webapp = "https://prod-url.yorusite.com"}
          default {write-host "This should be run from the central admin box - Press any key to exit this script"; read-host; exit}
	 }

	write-host "retracting $wsp from $webapp"
	uninstall-spsolution $wsp -WebApplication $webapp -confirm:$false
	wait4timer($wsp)
	write-host "Removing solution $wsp from the farm"
	remove-spsolution $wsp -Confirm:$false
	write-host "Done Removing"
	sleep 2
	write-host "Adding solution $path\$wsp to the farm"
	add-spsolution -literalpath "$path\$wsp"
	write-host "Deploying solution $wsp to $webapp"
	install-spsolution -Identity $wsp -WebApplication $webapp -gacdeployment
	wait4timer($wsp)
	write-host "Done adding"
	write-host "press any key to continue"
	$a = read-host
} #end main

#I grabbed this right from Nik Patel's script - there are some operations that you cant do in quick succession - the system needs time to copy to each node and let the timer job run it's course.
# one way to deal with this, is to use a sleep timer, but that's not exact, I liked Nik's approach of checking the solution's job status - well done Nik!
function wait4timer($webapp) {
	$solution = Get-SPSolution | where-object {$_.Name -eq $wsp}
	if ($solution -ne $null) 
	{
		$counter = 1   

		Write-Host "Waiting to finish soultion timer job"
		while( ($solution.JobExists -eq $true ) -and ( $counter -lt 50 ) ) 
		{   
			Write-Host "Please wait..."
			sleep 2
			$counter++   
		}

		Write-Host "Finished the solution timer job" 		
	}
} #end wait4timer

main