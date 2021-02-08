
param( 
    [Parameter( Mandatory = $true, HelpMessage = "Please enter value")]    
    [string] $mq = "nothing",
    [Parameter( Mandatory = $true, HelpMessage = "Please enter value")]
    [int] $workers = 2,
    [Parameter( Mandatory = $true, HelpMessage = "Please enter value")]
    [int] $batches = 2,
    [Parameter( Mandatory = $true, HelpMessage = "Please enter value")]
    [int] $batchSize = 20 
)

Write-Host "type      : $mq"
Write-Host "workers   : $workers"
Write-Host "batches   : $batches"
Write-Host "batch size: $batchSize"

$Env:batches = $batches
$Env:batchSize = $batchSize

if($mq -eq "AZURE" ){
    $Env:queuelib = "AZURE"
    $Env:mqConnectionString = "Endpoint=sb://your azure service bus connection string here"
}

if( $mq -eq "NATS") {
    $Env:queuelib = "NATS"
    $Env:mqConnectionString ="nats://ruser:T0pS3cr3t@localhost:4222"
}

    1..$workers | ForEach-Object {
        Start-Process -FilePath dotnet  -ArgumentList ".\DP.MessageHandler\bin\Debug\netcoreapp2.1\DP.MessageHandler.dll"
    }
    
    Start-Sleep 5
    
    # Start-Process -FilePath dotnet  -ArgumentList ".\DP.MessagePublisher\bin\Debug\netcoreapp2.1\DP.MessagePublisher.dll"




