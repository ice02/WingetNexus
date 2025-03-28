// create a powershell to dwnload a file and appli ConvertFrom-Json mothod on the content
$uri = "https://jsonplaceholder.typicode.com/posts/1"
$output = "C:\Users\Public\Downloads\output.json"
Invoke-WebRequest -Uri $uri -OutFile $output
$content = Get-Content $output
$json = $content | ConvertFrom-Json
$json
