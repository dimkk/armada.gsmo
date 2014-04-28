Add-PSSnapin "Microsoft.SharePoint.PowerShell"

# URL of the Site
$web = Get-SPWeb -Identity "http://gs.msk.mosreg.ru"
 
$manager = $web.Site.WorkFlowManager
 
# Name of the list
$list = $web.Lists["Поручения"]
 
# Name of the Workflow
$assoc = $list.WorkflowAssociations.GetAssociationByName("Вычисление полей поручения","en-US")
 
$data = $assoc.AssociationData
$items = $list.Items
foreach($item in $items)
{
    $wf = $manager.StartWorkFlow($item, $assoc, $data, "RunOptions.SynchronousAllowPostpone")
}
  
$manager.Dispose()
$web.Dispose()