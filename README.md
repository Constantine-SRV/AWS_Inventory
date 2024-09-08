# AWS Inventory Tool

This program queries all AWS regions and generates XLS files containing information about the discovered objects. AWS credentials should be saved in a JSON file, and the path to this file should be passed to the program as a parameter.

## AWS Credentials JSON Format

The JSON file format should be as follows:

```json
{
  "AccessKeyId": "AWS_ACCESS_KEY",
  "SecretAccessKey": "AWS_SECRET_KEY"
}
```

## Linux 
wget -O AWS_Inventory_linux https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_linux

chmod +x AWS_Inventory_linux

nano aws.json

./AWS_Inventory_linux aws.json

## MacOS 
curl -L -o AWS_Inventory_mac https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_mac

chmod +x AWS_Inventory_mac

nano aws.json

./AWS_Inventory_mac aws.json

## Windows
Download the Windows binary from the following link: https://github.com/Constantine-SRV/AWS_Inventory/releases/download/latest_release/AWS_Inventory_win.exe

Create an aws.json file with the AWS credentials in the same directory as the executable:

AWS_Inventory_win.exe aws.json
