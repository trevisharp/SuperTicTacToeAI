clear
"Building library..."
cd ST3AI
$output = dotnet run
cd ..
clear
if ($output -ne $null)
{
    $output
}
else
{
    "Executing Windows Forms app..."
    cd ST3UI
    $output = dotnet run
    clear
    if ($output -ne $null)
    {
        $output
    }
    cd ..
}